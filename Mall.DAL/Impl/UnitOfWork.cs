using WS.DAL.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

using System.Threading.Tasks;


namespace WS.DAL.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        public IServiceProvider Services { get; set; }

        public  MallDbContext DataContext { get;set;}

        public UnitOfWork(IServiceProvider service)
        {
            //Id = Guid.NewGuid();
            this.Services = service;
            DataContext = this.Services.GetService(typeof(MallDbContext)) as MallDbContext;
        }

        public bool IsUnitSubmit { get; set; }

        //public Guid Id { get; set; }

        //private MallDbContext _mallDbContext;
        //private string _connectionString;
        ///// <summary>
        ///// 获取或设置 当前使用的数据访问上下文对象
        ///// </summary>
        //public MallDbContext DataContext
        //{
        //    get
        //    {
        //        if (_mallDbContext == null)
        //        {
        //            if (_connectionString == null)
        //            {
        //                _mallDbContext = MallContextFactory.GetCurrentDbContext();
        //            }
        //            else
        //            {
        //                _mallDbContext = MallContextFactory.GetCurrentDbContext(_connectionString);

        //            }
        //        }
        //        return _mallDbContext;
        //    }
        //}

        //public MallDbContext GetNewDataContext()
        //{

        //    if (_connectionString == null)
        //    {
        //        _mallDbContext = new MallDbContext();
        //    }
        //    else
        //    {
        //        _mallDbContext = new MallDbContext(_connectionString);

        //    }

        //    return _mallDbContext;
        //}



        //public MallDbContext SetConnectionString(string connectionString)
        //{
        //    _connectionString = connectionString;
        //    return _mallDbContext;
        //}

        public int Submit()
        {
            try
            {

                int i = DataContext.SaveChanges();
                IsUnitSubmit = false;
                return i;

            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.InnerException is SqlException)
                {
                    SqlException sqlEx = e.InnerException.InnerException as SqlException;
                    // string msg = DataHelper.GetSqlExceptionMessage(sqlEx.Number);
                    // throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + msg, sqlEx);
                }
                throw;
            }
        }

        public async Task<int> SubmitAsync()
        {

            try
            {

                int i = await DataContext.SaveChangesAsync();
                IsUnitSubmit = false;
                return i;

            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.InnerException is SqlException)
                {
                    SqlException sqlEx = e.InnerException.InnerException as SqlException;
                    // string msg = DataHelper.GetSqlExceptionMessage(sqlEx.Number);
                    // throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + msg, sqlEx);
                }
                throw;
            }
        }
    }
}
