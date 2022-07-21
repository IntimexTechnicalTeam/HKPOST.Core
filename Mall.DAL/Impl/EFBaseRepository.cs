
using WS.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace WS.DAL.Impl
{
    public class EFBaseRepository : BaseRepository
    {
        public IUnitOfWork UnitWork { get; set; }

        public IServiceProvider Services { get; set; }

        public EFBaseRepository(IServiceProvider service)
        {
            this.Services = service;
            UnitWork = Services.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
        }

        #region 同步

        protected int SubmitChanges()
        {
            return !UnitWork.IsUnitSubmit ? UnitWork.Submit() : 0;
        }

        public override void Insert<T>(T t)
        {
            UnitWork.DataContext.Set<T>().Add(t);
            SubmitChanges();
        }

        public override void Insert<T>(List<T> list)
        {

            UnitWork.DataContext.Set<T>().AddRange(list);
            SubmitChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public override void Update<T>(T t)
        {
            //if (t == null) throw new Exception("t is null");
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Attach(t);//将数据附加到上下文，支持实体修改和新实体，重置为UnChanged
                UnitWork.DataContext.Entry<T>(t).State = EntityState.Modified;
                SubmitChanges(); //保存 然后重置为UnChanged
            }
        }

        /// <summary>
        /// 批量更新，跟据表的主键批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要更新的集合</param>
        public override void Update<T>(List<T> tList)
        {
            foreach (var t in tList)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
                UnitWork.DataContext.Entry<T>(t).State = EntityState.Modified;
            }
            SubmitChanges();
        }

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere">lamada表达式作条件</param>
        public override void Update<T>(Expression<Func<T, bool>> funcWhere)
        {
            T t = UnitWork.DataContext.Set<T>().Where(funcWhere).FirstOrDefault();
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
                UnitWork.DataContext.Entry<T>(t).State = EntityState.Modified;
                SubmitChanges();
            }
        }

        /// <summary>
        /// 先附加 再删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public override void Delete<T>(T t)
        {
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
                UnitWork.DataContext.Set<T>().Remove(t);
                SubmitChanges();
            }
        }

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        public override void Delete<T>(Expression<Func<T, bool>> funcWhere)
        {
            T t = UnitWork.DataContext.Set<T>().FirstOrDefault(funcWhere);
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Remove(t);
                SubmitChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        public override void Delete<T>(int Id)
        {
            T t = this.UnitWork.DataContext.Set<T>().Find(Id);//也可以附加
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Remove(t);
                SubmitChanges();
            }
        }

        /// <summary>
        /// 批量删除，根据表的主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要删除的实体集合</param>
        public override void Delete<T>(List<T> tList)
        {
            foreach (var t in tList)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
            }
            UnitWork.DataContext.Set<T>().RemoveRange(tList);
            SubmitChanges();
        }

        /// <summary>
        /// 根据主键获取一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        public override T GetModelById<T>(object Id)
        {
            var model = UnitWork.DataContext.Set<T>().Find(Id);
            return model;
        }

        public override T GetModel<T>(Expression<Func<T, bool>> funcWhere)
        {
            var model = UnitWork.DataContext.Set<T>().FirstOrDefault(funcWhere);
            return model;
        }

        public override IQueryable<T> GetList<T>()
        {
            var result = UnitWork.DataContext.Set<T>();
            return result;
        }

        public override IQueryable<T> GetList<T>(Expression<Func<T, bool>> funcWhere)
        {
            var result = GetList<T>().Where(funcWhere);
            return result;
        }

        public override IQueryable<T> GetList<T>(string sql, params object[] parameters)
        {
            var result =  UnitWork.DataContext.Set<T>().FromSqlRaw(sql, parameters).AsQueryable();
            return result;
        }

        public override bool Any<T>(Expression<Func<T, bool>> funcWhere)
        {
            bool flag = UnitWork.DataContext.Set<T>().Any(funcWhere);
            return flag;
        }

        #endregion

        #region 异步

        protected async Task<int> SubmitChangesAsync()
        {
            return !UnitWork.IsUnitSubmit ? await UnitWork.SubmitAsync() : 0;
        }

        public override async Task InsertAsync<T>(T t)
        {
            await UnitWork.DataContext.Set<T>().AddAsync(t);
            await SubmitChangesAsync();
        }

        public override async Task InsertAsync<T>(List<T> list)
        {
            await UnitWork.DataContext.Set<T>().AddRangeAsync(list);
            await SubmitChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public override async Task UpdateAsync<T>(T t)
        {
            //if (t == null) throw new Exception("t is null");
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Attach(t);//将数据附加到上下文，支持实体修改和新实体，重置为UnChanged
                UnitWork.DataContext.Entry<T>(t).State = EntityState.Modified;
                await SubmitChangesAsync(); //保存 然后重置为UnChanged
            }
        }

        /// <summary>
        /// 批量更新，跟据表的主键批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要更新的集合</param>
        public override async Task UpdateAsync<T>(List<T> tList)
        {
            foreach (var t in tList)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
                UnitWork.DataContext.Entry<T>(t).State = EntityState.Modified;
            }
            await SubmitChangesAsync();
        }

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere">lamada表达式作条件</param>
        public override async Task UpdateAsync<T>(Expression<Func<T, bool>> funcWhere)
        {
            T t = UnitWork.DataContext.Set<T>().Where(funcWhere).FirstOrDefault();
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
                UnitWork.DataContext.Entry<T>(t).State = EntityState.Modified;
                await SubmitChangesAsync();
            }
        }

        /// <summary>
        /// 先附加 再删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public override async Task DeleteAsync<T>(T t)
        {
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
                UnitWork.DataContext.Set<T>().Remove(t);
                await SubmitChangesAsync();
            }
        }

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        public override async Task DeleteAsync<T>(Expression<Func<T, bool>> funcWhere)
        {
            T t = UnitWork.DataContext.Set<T>().FirstOrDefault(funcWhere);
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Remove(t);
                await SubmitChangesAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        public override async Task DeleteAsync<T>(int Id)
        {
            T t = this.UnitWork.DataContext.Set<T>().Find(Id);//也可以附加
            if (t != null)
            {
                UnitWork.DataContext.Set<T>().Remove(t);
                await SubmitChangesAsync();
            }
        }

        /// <summary>
        /// 批量删除，根据表的主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList">要删除的实体集合</param>
        public override async Task DeleteAsync<T>(List<T> tList)
        {
            foreach (var t in tList)
            {
                UnitWork.DataContext.Set<T>().Attach(t);
            }
            UnitWork.DataContext.Set<T>().RemoveRange(tList);
            await SubmitChangesAsync();
        }

      

        /// <summary>
        /// 根据主键获取一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        public override async Task<T> GetModelByIdAsync<T>(object Id)
        {
            var model = await UnitWork.DataContext.Set<T>().FindAsync(Id);
            return model;
        }

        public override async Task<T> GetModelAsync<T>(Expression<Func<T, bool>> funcWhere)
        {
            var model = await UnitWork.DataContext.Set<T>().FirstOrDefaultAsync(funcWhere);
            return model;
        }

        public override async Task<IQueryable<T>> GetListAsync<T>()
        {
            var result = await UnitWork.DataContext.Set<T>().ToArrayAsync();
            return result.AsQueryable();
        }

        public override async Task<IQueryable<T>> GetListAsync<T>(Expression<Func<T, bool>> funcWhere)
        {
            var result = await UnitWork.DataContext.Set<T>().Where(funcWhere).ToArrayAsync();
            return result.AsQueryable();
        }

        public override async Task<IQueryable<T>> GetListAsync<T>(string sql, params object[] parameters)
        {
            var result = await UnitWork.DataContext.Set<T>().FromSqlRaw(sql, parameters).ToArrayAsync();
            return result.AsQueryable();
        }

        public override async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> funcWhere)
        {
            bool flag = await UnitWork.DataContext.Set<T>().AnyAsync(funcWhere);
            return flag;
        }

        #endregion

        public override IQueryable<T> GetList<T>(string sql, List<SqlParameter> parameters)
        {
            var result =  UnitWork.DataContext.Set<T>().FromSqlRaw(sql, parameters);
            return result;
        }

        public override async Task<IQueryable<T>> GetListAsync<T>(string sql, List<SqlParameter> parameters)
        {
            var result = await UnitWork.DataContext.Set<T>().FromSqlRaw(sql, parameters).ToArrayAsync();
            return result.AsQueryable();
        }

        public override async Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> param)
        {           
           return await UnitWork.DataContext.Database.ExecuteSqlRawAsync(sql, param);
        }

        public override async Task<int> ExecuteSqlRawAsync(string sql, params object[] param)
        {
            return await UnitWork.DataContext.Database.ExecuteSqlRawAsync(sql, param);
        }

        public override int ExecuteSqlCommand(string sql, IEnumerable<object> param)
        {
            return UnitWork.DataContext.Database.ExecuteSqlRaw(sql, param);
        }

        public override IDbContextTransaction CreateTransation()
        {
            return UnitWork.DataContext.Database.BeginTransaction();
        }
    }
}
