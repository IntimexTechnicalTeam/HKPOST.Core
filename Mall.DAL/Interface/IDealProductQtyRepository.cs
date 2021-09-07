using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;

namespace WS.DAL.Interface
{
    public interface IDealProductQtyRepository : IDependency
    {
        Task<int> UpdateQtyWhenPurchasing(Guid SkuId, int InvtActualQty, int SalesQty,Guid Id);

        Task<int> UpdateQtyWhenAddToCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenDeleteCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenModifyCart(Guid SkuId, int InvtHoldQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenPay(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenOrderComplate(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id);

        Task<int> UpdateQtyWhenOrderCancel(Guid SkuId, int InvtReservedQty, int SalesQty, Guid Id);
    }
}
