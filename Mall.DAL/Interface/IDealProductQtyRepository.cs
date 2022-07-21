using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;
using WS.Domain.PreHeat;

namespace WS.DAL.Interface
{
    public interface IDealProductQtyRepository : IDependency
    {
        /// <summary>
        /// 采购入库，更新数据库的Qty
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtActualQty"></param>
        /// <param name="SalesQty"></param>
        /// <param name="Id">PushMessage.Id</param>
        /// <returns></returns>
        Task<int> UpdateQtyWhenPurchasing(Guid SkuId, int InvtActualQty, int SalesQty, Guid Id);

        /// <summary>
        /// 销售退回或发货退回或采购退回，更新数据库的Qty
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtActualQty"></param>
        /// <param name="SalesQty"></param>
        /// <param name="Id">PushMessage.Id</param>
        /// <returns></returns>
        Task<int> UpdateQtyWhenReturn(Guid SkuId, int InvtActualQty, int SalesQty, Guid Id);

        /// <summary>
        /// 加入购物车，更新数据库的Qty
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <param name="Id">PushMessage.Id</param>
        /// <returns></returns>
        Task<int> UpdateQtyWhenAddToCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        /// <summary>
        /// 删除购物车，更新数据库的Qty
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="InvtHoldQty"></param>
        /// <param name="SalesQty"></param>
        /// <param name="Id">PushMessage.Id</param>
        /// <returns></returns>
        Task<int> UpdateQtyWhenDeleteCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenModifyCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenPay(Guid SkuId, int InvtReservedQty, int SalesQty, int InvtHoldQty, Guid Id);

        Task<int> UpdateQtyWhenDeliveryArranged(Guid SkuId, int InvtReservedQty, int InvtActualQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenOrderCancel(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenPayTimeOut(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        Task<int> UpdateProductyQty(IEnumerable<PreProductQty> dataSource);
    }
}
