
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;
using WS.DAL.Impl;
using WS.DAL.Interface;
using WS.Domain.Enums;
using WS.Model;
using WS.Model.MQMessage;

namespace WS.BLL
{
    /// <summary>
    /// 消费者服务类
    /// </summary>
    public class DealProductQtyBll : IDealProductQtyBll
    {
        public EFBaseRepository baseRepository { get; set; }

        public IDealProductQtyRepository ProductQtyRepository { get; set; }

        public Dictionary<QtyType, Func<TmpProductQty, Task<int>>> dicQtyMethord = new Dictionary<QtyType, Func<TmpProductQty, Task<int>>>();

        public DealProductQtyBll()
        {
            dicQtyMethord.Add(QtyType.WhenPurchasing, UpdateQtyWhenPurchasing);
            dicQtyMethord.Add(QtyType.WhenAddToCart, UpdateQtyWhenAddToCart);
            dicQtyMethord.Add(QtyType.WhenDeleteCart, UpdateQtyWhenDeleteCart);
            dicQtyMethord.Add(QtyType.WhenModifyCart, UpdateQtyWhenModifyCart);
            dicQtyMethord.Add(QtyType.WhenPay, UpdateQtyWhenPay);
            dicQtyMethord.Add(QtyType.WhenOrderComplate, UpdateQtyWhenOrderComplate);
            dicQtyMethord.Add(QtyType.WhenOrderCancel, UpdateQtyWhenOrderCancel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">PushMessage.Id</param>
        /// <returns></returns>
        public async Task<SystemResult> DealProductQty(Guid Id)
        {
            var result = new SystemResult() { Succeeded = false };
            var record = await baseRepository.GetModelByIdAsync<PushMessage>(Id);

            if (record == null)
            {
                result.Message = $"找不到记录{Id}";
                return result;
            }

            if (record.State == MQState.Deal)
            {
                result.Message = $"记录{Id}已处理";
                return result;
            }
          
            var tmpProductQty = JsonUtil.ToObject<TmpProductQty>(record.MsgContent);
            tmpProductQty.Id = Id;
            int doFlag = await dicQtyMethord[tmpProductQty.QtyType].Invoke(tmpProductQty);

            result.Succeeded = doFlag > 0 ? true : false;

            return result;
        }

        private async Task<int> UpdateQtyWhenPurchasing(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenPurchasing(tmpProductQty.SkuId, tmpProductQty.InvtActualQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }

        private async Task<int> UpdateQtyWhenAddToCart(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenAddToCart(tmpProductQty.SkuId, tmpProductQty.InvtHoldQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }

        private async Task<int> UpdateQtyWhenDeleteCart(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenDeleteCart(tmpProductQty.SkuId, tmpProductQty.InvtHoldQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }

        private async Task<int> UpdateQtyWhenModifyCart(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenModifyCart(tmpProductQty.SkuId, tmpProductQty.InvtHoldQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }

        private async Task<int> UpdateQtyWhenPay(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenPay(tmpProductQty.SkuId, tmpProductQty.InvtReservedQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }

        private async Task<int> UpdateQtyWhenOrderComplate(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenOrderComplate(tmpProductQty.SkuId, tmpProductQty.InvtReservedQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }

        private async Task<int> UpdateQtyWhenOrderCancel(TmpProductQty tmpProductQty)
        {
            return await ProductQtyRepository.UpdateQtyWhenOrderCancel(tmpProductQty.SkuId, tmpProductQty.InvtReservedQty, tmpProductQty.SalesQty, tmpProductQty.Id);
        }


    }
}
