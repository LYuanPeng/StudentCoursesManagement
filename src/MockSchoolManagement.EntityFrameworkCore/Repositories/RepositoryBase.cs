using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 默认的仓储通用功能实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRepository"></typeparam>
    public class RepositoryBase<TEntity, TRepository> : IRepository<TEntity, TRepository> where TEntity : class
    {
        protected readonly AppDbContext _dbContext;

        /// <summary>
        /// 通过泛型，从数据库上下文中获取领域模型
        /// </summary>
        public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public RepositoryBase(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 查询
        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();

        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        #endregion


        #region 单体查询
        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await GetAll().FirstOrDefaultAsync(predicate);
            return entity;
        }


        #endregion


        #region 添加

        public TEntity Insert(TEntity entity)
        {
            var newEntity = Table.Add(entity).Entity;
            Save();
            return newEntity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entityEntry = await Table.AddAsync(entity);
            await SaveAsync();
            return entityEntry.Entity;
        }

        #endregion


        #region 更新

        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            Save();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
            return entity;
        }

        #endregion


        #region 删除

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            Save();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            await SaveAsync();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                await DeleteAsync(entity);
            }
        }

        #endregion


        #region 计算总和

        public int Count()
        {
            return GetAll().Count();
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).CountAsync();
        }


        public long LongCount()
        {
            return GetAll().LongCount();

        }

        public async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).LongCountAsync();
        }

        #endregion



        #region 检查是否跟踪
        /// <summary>
        /// 检查实体是否处于跟踪状态
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);

            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }

        #endregion

        #region 保存数据
        protected void Save()
        {
            //调用数据库上下文保存数据
            _dbContext.SaveChanges();
        }

        protected async Task SaveAsync()
        {
            //调用数据库上下文保存数据的异步方法
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}
