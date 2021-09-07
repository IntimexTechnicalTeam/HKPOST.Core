using WS.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.DAL.Impl
{
    /// <summary>
    /// 这里只做数据同步，一切与Redis的数据为准，MQ从Redis中获取数据，回写数据库
    /// </summary>
    public class DealProductQtyRepository : IDealProductQtyRepository
    {
        public EFBaseRepository baseRepository { get; set; }

        /// <summary>
        /// 采购入库成功，更新库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenPurchasing(Guid SkuId, int InvtActualQty,int SalesQty,Guid Id)
        {
            string sql = $"update ProductQties set InvtActualQty =@InvtActualQty ,SalesQty = @SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtActualQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessage set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.UnitWork.CreateTransation())
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
        public async Task<int> UpdateQtyWhenAddToCart(Guid SkuId, int InvtHoldQty,int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtHoldQty =@InvtHoldQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            return await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
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
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessage set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.UnitWork.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();              
            }

            return  flag;
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
            param.Add(new SqlParameter { ParameterName = "@InvtActualQty", Value = InvtHoldQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessage set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.UnitWork.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当付款时
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenPay(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtReservedQty=@InvtReservedQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtReservedQty", Value = InvtReservedQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessage set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.UnitWork.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当订单完成时
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtReservedQty"></param>
        /// <param name="SalesQty"></param>
        /// <returns></returns>
        public async Task<int> UpdateQtyWhenOrderComplate(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id)
        {
            string sql = $"update ProductQties set InvtReservedQty=@InvtReservedQty ,SalesQty=@SalesQty,UpdateDate=GETDATE() where SkuId = @SkuId";
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter { ParameterName = "@InvtReservedQty", Value = InvtReservedQty });
            param.Add(new SqlParameter { ParameterName = "@SalesQty", Value = SalesQty });
            param.Add(new SqlParameter { ParameterName = "@SkuId", Value = SkuId });

            string sql2 = $"update PushMessage set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.UnitWork.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

        /// <summary>
        /// 当订单取消时
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

            string sql2 = $"update PushMessage set State =2 where  Id = @Id";
            var param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter { ParameterName = "@Id", Value = Id });

            int flag = 0;
            using (var tran = baseRepository.UnitWork.CreateTransation())
            {
                flag += await baseRepository.ExecuteSqlCommandAsync(sql, param.ToArray());
                flag += await baseRepository.ExecuteSqlCommandAsync(sql2, param2.ToArray());
                tran.Commit();
            }

            return flag;
        }

    }
}
