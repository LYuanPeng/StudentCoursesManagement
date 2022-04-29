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
            
            //使用SQLServer数据库，通过IConfiguration访问去获取，自定义名称的
            //MockStudentDBConnection作为我们的连接字符串
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));

            //配置ASP.NET Core Identity服务
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
                //验证邮箱
                options.SignIn.RequireConfirmedEmail = true;
            });
            // 注入AddHttpContextAccessor
            services.AddHttpContextAccessor();
            // 添加策略（角色、声明）
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            // 配置Cookie信息
            services.ConfigureApplicationCookie(options =>
            {
                //更改默认的拒绝访问路由
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                //同一系统全局的Cookie名称
                options.Cookie.Name = "MockSchoolCookieName";
            });

            //// 配置微软Azure第三方登录
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
                //因为PagedResultDto<>在  //MockSchoolManagement.Application类库中，所以通过PagedResultDto<>获取程序集信息
            };
            //自动注入服务到依赖注入容器
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
            //将获取到的程序集信息注册到我们的依赖注入容器中
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
            


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //数据初始化
            app.UseDataInitializer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //否则显示用户友好的错误页面
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            {
                //用于处理错误异常
                //app.UseStatusCodePages();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            //使用纯静态文件支持的中间件，而不使用带有终端的中间件
            app.UseStaticFiles();
            
            //主要作用就是启用路由
            app.UseRouting();
            //添加验证中间件
            app.UseAuthentication();
            //授权中间件
            app.UseAuthorization();


            //app.UseMvcWithDefaultRoute();
            //app.UseMvc();

            //终结点路由
            //UseEndpoints验证路由服务和EndpointRouting()中间件是否启用和注册到管道中
            //注册完毕后，改变中间件的配置，将路由规则运用到应用程序中
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

        //// 授权访问
        //private bool AuthorizeAccess(AuthorizationHandlerContext context)
        //{
        //    return context.User.IsInRole("Admin") &&
        //           context.User.HasClaim(claim => claim.Type == "Eidt Role" && claim.Value == "true") ||
        //           context.User.IsInRole("Super Admin");
        //}
    }

    
}
