using WS.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Domain.PreHeat;

namespace WS.DAL.Impl
{
    /// <summary>
    /// 这里只做数据同步，一切与Redis的数据为准，MQ从Redis中获取数据，回写数据库
    /// </summary>
    public class DealProductQtyRepository : IDealProductQtyRepository
    {
        public IBaseRepository baseRepository;

        public IServiceProvider service;

        public DealProductQtyRepository(IServiceProvider services)
        {
            this.service = services;
            baseRepository = service.GetService(typeof(IBaseRepository)) as IBaseRepository;
        }

        /// <summary>
        /// 采购入库成功，更新库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenPurchasing(Guid SkuId, int InvtActualQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtActualQty =@InvtActualQty ,SalesQty = @SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtActualQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtActualQty"></param>
        /// <param name="SalesQty"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenReturn(Guid SkuId, int InvtActualQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtActualQty =@InvtActualQty ,SalesQty = @SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtActualQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 加入购物车，进行Hold货时
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenAddToCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtHoldQty =@InvtHoldQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtHoldQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当移除购物车上的物品时
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenDeleteCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtHoldQty =@InvtHoldQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtHoldQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当修改购物车上的物品数量时
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenModifyCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtHoldQty =@InvtHoldQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtHoldQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当订单状态变为PaymentConfirmed
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenPay(Guid SkuId, int InvtReservedQty, int SalesQty, int InvtHoldQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtReservedQty=@InvtReservedQty ,SalesQty=@SalesQty,InvtHoldQty=@InvtHoldQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtReservedQty", Value = InvtReservedQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@InvtHoldQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当订单状态变为已安排发货
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtReservedQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenDeliveryArranged(Guid SkuId, int InvtReservedQty, int InvtActualQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtReservedQty=@InvtReservedQty,InvtActualQty=@InvtActualQty,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtReservedQty", Value = InvtReservedQty });
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtActualQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当订单取消时，一般为订单为已付款，处理中，已安排发货时的取消订单
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtReservedQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenOrderCancel(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtReservedQty=@InvtReservedQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtReservedQty", Value = InvtReservedQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当支付超时，回滚Hold数， 一般为订单已创建了，订单是待付款状态（后台可在待付款状态时取消订单）或者支付超时取消订单
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenPayTimeOut(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtHoldQty =@InvtHoldQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtHoldQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessages set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        public async Task<int> UpdateProductyQty(IEnumerable<PreProductQty> dataSource)
        {
            StringBuilder sb = new StringBuilder();
            var param = new List<SqlParameter>();

            int PerPage = 500;                             //一次多少条
            int Total = dataSource.Count();
            //int HowMuch = Total % PerPage > 0 ? Total / PerPage+1 : Total / PerPage ;   //执行多少次
            //int HowMuch = ((Total - 1) / PerPage) + 1;
            int HowMuch = (int)Math.Ceiling(Total / (decimal)PerPage);

            int flag = 0;

            for (int i = 0; i < HowMuch; i++)
            {
                var query = dataSource.ToList().Skip(i * PerPage).Take(PerPage);
                foreach (var item in query)
                {
                    sb.Append(@$"update ProductQties set InvtReservedQty={item.InvtReservedQty},InvtActualQty={item.InvtActualQty},SalesQty={item.SaleQty}" +
                                     $",InvtHoldQty ={item.InvtHoldQty} ,UpdateDate=GETDATE() where SkuId = '{item.SkuId}';");
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    using (var tran = baseRepository.CreateTransation())
                    {
                        flag = await baseRepository.ExecuteSqlCommandAsync(sb.ToString(), param.ToArray());
                        tran.Commit();
                        sb.Clear();
                    }
                }
                await Task.Delay(3000);
            }
            return flag;
        }

    }
}
