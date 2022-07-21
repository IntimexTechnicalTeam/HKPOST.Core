using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Linq;
using Web.Framework;
using Microsoft.EntityFrameworkCore.Storage;

namespace WS.DAL.Interface
{

    public abstract class BaseRepository : IBaseRepository
    {
        #region 同步

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public abstract void Insert<T>(T t) where T : class;

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public abstract void Insert<T>(List<T> list) where T : class;

        /// <summary>
        /// 更新單個
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public abstract void Update<T>(T t) where T : class;

        /// <summary>
        /// 批量更新，跟据表的主键批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要更新的集合</param>
        public abstract void Update<T>(List<T> tList) where T : class;

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere">lamada表达式作条件</param>
        public abstract void Update<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 先附加 再删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public abstract void Delete<T>(T t) where T : class;

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        public abstract void Delete<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        public abstract void Delete<T>(int Id) where T : class;

        /// <summary>
        /// 批量删除，根据表的主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要删除的实体集合</param>
        public abstract void Delete<T>(List<T> tList) where T : class;

        public abstract T GetModelById<T>(object Id) where T : class;

        public abstract T GetModel<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        public abstract bool Any<T>(Expression<Func<T, bool>> funcWhere) where T : class;
        public abstract IQueryable<T> GetList<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        public abstract IQueryable<T> GetList<T>() where T : class;

        public abstract IQueryable<T> GetList<T>(string sql, params object[] parameters) where T : class;

        public abstract IQueryable<T> GetList<T>(string sql, List<SqlParameter> parameters) where T : class;

        #endregion

        #region 异步

        public abstract Task InsertAsync<T>(T t) where T : class;
        public abstract Task InsertAsync<T>(List<T> list) where T : class;
        public abstract Task UpdateAsync<T>(T t) where T : class;
        public abstract Task UpdateAsync<T>(List<T> tList) where T : class;
        public abstract Task UpdateAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;
        public abstract Task DeleteAsync<T>(T t) where T : class;
        public abstract Task DeleteAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;
        public abstract Task DeleteAsync<T>(int Id) where T : class;
        public abstract Task DeleteAsync<T>(List<T> tList) where T : class;
        public abstract Task<T> GetModelByIdAsync<T>(object Id) where T : class;
        public abstract Task<T> GetModelAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;
        public abstract Task<bool> AnyAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;
        public abstract Task<IQueryable<T>> GetListAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;
        public abstract Task<IQueryable<T>> GetListAsync<T>() where T : class;

        public abstract Task<IQueryable<T>> GetListAsync<T>(string sql, params object[] parameters) where T : class;

        public abstract Task<IQueryable<T>> GetListAsync<T>(string sql, List<SqlParameter> parameters) where T : class;

        #endregion

        public abstract Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> param);

        public abstract Task<int> ExecuteSqlRawAsync(string sql, params object[] param);

        public abstract int ExecuteSqlCommand(string sql, IEnumerable<object> param);

        public abstract IDbContextTransaction CreateTransation();
    }

    public interface IBaseRepository : IDependency
    {
        #region 同步

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void Insert<T>(T t) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        void Insert<T>(List<T> list) where T : class;

        /// <summary>
        ///  更新單個
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void Update<T>(T t) where T : class;

        /// <summary>
        /// 批量更新，跟据表的主键批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要更新的集合</param>
        void Update<T>(List<T> tList) where T : class;

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere">lamada表达式作条件</param>
        void Update<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 按實體刪除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void Delete<T>(T t) where T : class;

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        void Delete<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        ///  根據主鍵刪除一條數據
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        void Delete<T>(int Id) where T : class;

        /// <summary>
        /// 批量删除，根据表的主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要删除的实体集合</param>
        void Delete<T>(List<T> tList) where T : class;

        /// <summary>
        /// 按主键查找第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        T GetModelById<T>(object Id) where T : class;

        T GetModel<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 判断集合中是否有一条以上数据，有就true，没有就false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        bool Any<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        IQueryable<T> GetList<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        IQueryable<T> GetList<T>() where T : class;

        IQueryable<T> GetList<T>(string sql, params object[] parameters) where T : class;

        IQueryable<T> GetList<T>(string sql, List<SqlParameter> parameters) where T : class;

        #endregion

        #region 异步

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        Task InsertAsync<T>(T t) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        Task InsertAsync<T>(List<T> list) where T : class;

        /// <summary>
        ///  更新單個
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        Task UpdateAsync<T>(T t) where T : class;

        /// <summary>
        /// 批量更新，跟据表的主键批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要更新的集合</param>
        Task UpdateAsync<T>(List<T> tList) where T : class;

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere">lamada表达式作条件</param>
        Task UpdateAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 按實體刪除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        Task DeleteAsync<T>(T t) where T : class;

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        Task DeleteAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        ///  根據主鍵刪除一條數據
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        Task DeleteAsync<T>(int Id) where T : class;

        /// <summary>
        /// 批量删除，根据表的主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要删除的实体集合</param>
        Task DeleteAsync<T>(List<T> tList) where T : class;

        /// <summary>
        /// 按主键查找第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetModelByIdAsync<T>(object Id) where T : class;

        Task<T> GetModelAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 判断集合中是否有一条以上数据，有就true，没有就false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        Task<IQueryable<T>> GetListAsync<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        Task<IQueryable<T>> GetListAsync<T>() where T : class;

        Task<IQueryable<T>> GetListAsync<T>(string sql, params object[] parameters) where T : class;

        Task<IQueryable<T>> GetListAsync<T>(string sql, List<SqlParameter> parameters) where T : class;

        #endregion

        Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> param);

        Task<int> ExecuteSqlRawAsync(string sql, params object[] param);

        int ExecuteSqlCommand(string sql, IEnumerable<object> param);

        IDbContextTransaction CreateTransation();
    }
}
