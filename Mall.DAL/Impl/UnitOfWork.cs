using WS.DAL.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace WS.DAL.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public bool IsUnitSubmit { get; set; }

        private MallDbContext _mallDbContext;
        private string _connectionString;
        /// <summary>
        /// 获取或设置 当前使用的数据访问上下文对象
        /// </summary>
        public MallDbContext DataContext
        {
            get
            {
                _mallDbContext = MallContextFactory.GetCurrentDbContext();
                return _mallDbContext;
            }
        }

        public MallDbContext GetNewDataContext()
        {

            if (_connectionString == null)
            {
                _mallDbContext = new MallDbContext();
            }
          
            return _mallDbContext;
        }

    
        public MallDbContext SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return _mallDbContext;
        }

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

        public IDbContextTransaction CreateTransation()
        {
            return this.DataContext.Database.BeginTransaction();
        }
    }
}
