using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 所有仓储的接口约定
    /// </summary>
    /// <typeparam name="TEntity">当前传入仓储的实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">传入仓储的主键类型</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region 查询
        /// <summary>
        /// 获取用于从整个表中检索实体的IQueryable
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 用于获取所有实体
        /// </summary>
        /// <returns>所有的实体列表</returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 用于获取所有实体的异步实现
        /// </summary>
        /// <returns>所有的实体列表</returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 用于获取传入本方法的所偶实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有的实体列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 用于获取传入本方法的所偶实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有的实体列表</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);


        /// <summary>
        /// 通过传入的筛选条件查询实体信息，如果没有找到，则返回null
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件查询实体信息，如果没有找到，则返回null
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);


        #endregion

        #region Insert

        /// <summary>
        /// 添加一个实体信息
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 添加一个实体信息
        /// </summary>
        /// <param name="entity"></param>
        Task<TEntity> InsertAsync(TEntity entity);

        #endregion

        #region Update

        /// <summary>
        /// 更新所有实体
        /// </summary>
        /// <param name="entity">实体</param>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更新所有实体
        /// </summary>
        /// <param name="entity">实体</param>
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion

        #region Delete

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// 按传入的条件可以删除多个实体
        /// </summary>
        /// <param name="predicate"></param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 按传入的条件可以删除多个实体
        /// </summary>
        /// <param name="predicate"></param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region 综合计算

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        int Count();

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync();

        /// <summary>
        /// 按条件筛选计算仓储中的实体总和
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>实体的总数</returns>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 按条件筛选计算仓储中的实体总和
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>实体的总数</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取此仓储中的所有实体的综合，如果数目很大的话
        /// </summary>
        /// <returns>实体的总数</returns>
        long LongCount();

        /// <summary>
        /// 获取此仓储中的所有实体的综合，如果数目很大的话
        /// </summary>
        /// <returns>实体的总数</returns>
        Task<long> LongCountAsync();

        /// <summary>
        /// 获取此仓储中的所有实体的综合，如果数目很大的话
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>实体的总数</returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取此仓储中的所有实体的综合，如果数目很大的话
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);



        #endregion

    }
}
