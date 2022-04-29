using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Students;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Infrastructure.Data;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.Security;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MockSchoolManagement
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc(a => a.EnableEndpointRouting = false);
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
            
            //ʹ��SQLServer���ݿ⣬ͨ��IConfiguration����ȥ��ȡ���Զ������Ƶ�
            //MockStudentDBConnection��Ϊ���ǵ������ַ���
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));

            //����ASP.NET Core Identity����
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                //��֤����
                options.SignIn.RequireConfirmedEmail = true;
            });
            // ע��AddHttpContextAccessor
            services.AddHttpContextAccessor();
            // ��Ӳ��ԣ���ɫ��������
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            // ����Cookie��Ϣ
            services.ConfigureApplicationCookie(options =>
            {
                //����Ĭ�ϵľܾ�����·��
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                //ͬһϵͳȫ�ֵ�Cookie����
                options.Cookie.Name = "MockSchoolCookieName";
            });

            //// ����΢��Azure��������¼
            //services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            //{
            //    microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
            //    microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
            //});

            services.AddScoped<IStudentRepository, SQLStudentRepository>();
            services.AddTransient(typeof(IRepository<,>), typeof(RepositoryBase<,>));

            //services.AddScoped<IStudentService, StudentService>();
            //services.AddScoped<ICourseService, CourseService>();



            var assembliesToScan = new[]
            {  
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(PagedResultDto<>)),
                //��ΪPagedResultDto<>��  //MockSchoolManagement.Application����У�����ͨ��PagedResultDto<>��ȡ������Ϣ
            };
            //�Զ�ע���������ע������
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
            //����ȡ���ĳ�����Ϣע�ᵽ���ǵ�����ע��������
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
            


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //���ݳ�ʼ��
            app.UseDataInitializer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //������ʾ�û��ѺõĴ���ҳ��
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            {
                //���ڴ�������쳣
                //app.UseStatusCodePages();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            //ʹ�ô���̬�ļ�֧�ֵ��м��������ʹ�ô����ն˵��м��
            app.UseStaticFiles();
            
            //��Ҫ���þ�������·��
            app.UseRouting();
            //�����֤�м��
            app.UseAuthentication();
            //��Ȩ�м��
            app.UseAuthorization();


            //app.UseMvcWithDefaultRoute();
            //app.UseMvc();

            //�ս��·��
            //UseEndpoints��֤·�ɷ����EndpointRouting()�м���Ƿ����ú�ע�ᵽ�ܵ���
            //ע����Ϻ󣬸ı��м�������ã���·�ɹ������õ�Ӧ�ó�����
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern: "{controller=Home}/{action=Index}");
            });

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello World");
            //});
        }

        //// ��Ȩ����
        //private bool AuthorizeAccess(AuthorizationHandlerContext context)
        //{
        //    return context.User.IsInRole("Admin") &&
        //           context.User.HasClaim(claim => claim.Type == "Eidt Role" && claim.Value == "true") ||
        //           context.User.IsInRole("Super Admin");
        //}
    }

    
}
