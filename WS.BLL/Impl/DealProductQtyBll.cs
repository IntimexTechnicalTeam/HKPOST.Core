
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;
using Web.MQ;
using WS.DAL.Impl;
using WS.DAL.Interface;
using WS.Domain.Enums;
using WS.Domain.PreHeat;
using WS.Model;
using WS.Model.MQMessage;

namespace WS.BLL
{
    /// <summary>
    /// 消费者服务类
    /// </summary>
    public class DealProductQtyBll : ServiceBase, IDealProductQtyBll
    {
        IServiceProvider _service;

        public IDealProductQtyRepository ProductQtyRepository;

        public Dictionary<MQType, Func<TmpProductQty, Task<int>>> dicQtyMethord = new Dictionary<MQType, Func<TmpProductQty, Task<int>>>();

        public DealProductQtyBll(IServiceProvider services) : base(services)
        {
            this._service = services;

            dicQtyMethord.Add(MQType.WhenPurchasing, UpdateQtyWhenPurchasing);
            dicQtyMethord.Add(MQType.WhenReturn, UpdateQtyWhenReturn);
            dicQtyMethord.Add(MQType.WhenAddToCart, UpdateQtyWhenAddToCart);
            dicQtyMethord.Add(MQType.WhenDeleteCart, UpdateQtyWhenDeleteCart);
            dicQtyMethord.Add(MQType.WhenModifyCart, UpdateQtyWhenModifyCart);
            dicQtyMethord.Add(MQType.WhenPay, UpdateQtyWhenPay);
            dicQtyMethord.Add(MQType.WhenDeliveryArranged, UpdateQtyWhenDeliveryArranged);
            dicQtyMethord.Add(MQType.WhenOrderCancel, UpdateQtyWhenOrderCancel);
            dicQtyMethord.Add(MQType.WhenPayTimeOut, UpdateQtyWhenPayTimeOut);
            dicQtyMethord.Add(MQType.WhenPurchasingReturn, UpdateQtyWhenPurchasingReturn);

            ProductQtyRepository = this.Services.GetService(typeof(IDealProductQtyRepository)) as IDealProductQtyRepository;
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
            tmpProductQty.MsgId = Id;
            int doFlag = await dicQtyMethord[tmpProductQty.MsgType].Invoke(tmpProductQty);

            result.Succeeded = doFlag > 0 ? true : false;

            return result;
        }

        #region 计算逻辑

        /// <summary>
        /// 采购入库成功，计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenPurchasing(Guid SkuId, int Qty)
        {         
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            if (InvtHoldQty < 0) InvtHoldQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //设置实际库存数
            var InvtActualQty = await RedisHelper.ZIncrByAsync(InvtActualQtyKey, SkuId.ToString(), Qty);

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId));

            var productQty = new ProductQty { InvtActualQty = (int)InvtActualQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        /// <summary>
        /// 采购退回，计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty">退回数量</param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenPurchaseReturn(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            if (InvtHoldQty < 0) InvtHoldQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //设置实际库存数       
            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            InvtActualQty = InvtActualQty - Qty < 0 ? 0 : InvtActualQty - Qty;
            await RedisHelper.ZAddAsync(InvtActualQtyKey, (InvtActualQty, SkuId));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId));

            var productQty = new ProductQty { InvtActualQty = (int)InvtActualQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        /// <summary>
        /// 銷售退回或發貨退回，计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty">退回数量</param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenReturn(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            if (InvtHoldQty < 0) InvtHoldQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //设置实际库存数
            var InvtActualQty = await RedisHelper.ZIncrByAsync(InvtActualQtyKey, SkuId.ToString(), Qty);

            //设置可销售库存数           
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;

            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId.ToString()));

            var productQty = new ProductQty { InvtActualQty = (int)InvtActualQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        /// <summary>
        /// 加入购物车，进行Hold货时,计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenAddToCart(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            if (InvtActualQty < 0) InvtActualQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //设置Hold货数量
            var InvtHoldQty = await RedisHelper.ZIncrByAsync(InvtHoldQtyKey, SkuId.ToString(), Qty);

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId));

            var productQty = new ProductQty { InvtHoldQty = (int)InvtHoldQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        /// <summary>
        /// 当移除购物车上的物品时,计算缓存中的库存,释放Hold货数
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenDeleteCart(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            if (InvtActualQty < 0) InvtActualQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //获取Hold货数
            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            //计算Hold货数
            InvtHoldQty = InvtHoldQty - Qty < 0 ? 0 : InvtHoldQty - Qty;
            //设置Hold货数
            await RedisHelper.ZAddAsync(InvtHoldQtyKey, (InvtHoldQty, SkuId.ToString()));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;

            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId.ToString()));

            var productQty = new ProductQty { InvtHoldQty = (int)InvtHoldQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        /// <summary>
        /// 当修改购物车上的物品数量时,计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenModifyCart(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            if (InvtActualQty < 0) InvtActualQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //计算Hold货数
            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            InvtHoldQty = InvtHoldQty + Qty;
            //设置Hold货数量
            await RedisHelper.ZAddAsync(InvtHoldQtyKey, (InvtHoldQty, SkuId.ToString()));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId.ToString()));

            var productQty = new ProductQty { InvtHoldQty = (int)InvtHoldQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        /// <summary>
        /// 当订单状态变更为PaymentConfirmed时,计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenPaymentConfirmed(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            if (InvtActualQty < 0) InvtActualQty = 0;

            //设置预留数
            var InvtReservedQty = await RedisHelper.ZIncrByAsync(InvtReservedQtyKey, SkuId.ToString(), Qty);

            //获取Hold数
            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            //计算Hold数
            InvtHoldQty = InvtHoldQty - Qty < 0 ? 0 : InvtHoldQty - Qty;
            //设置Hold数
            await RedisHelper.ZAddAsync(InvtHoldQtyKey, (InvtHoldQty, SkuId));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId));

            var productQty = new ProductQty { InvtReservedQty = (int)InvtReservedQty, SalesQty = (int)SalesQty, SkuId = SkuId, InvtHoldQty = (int)InvtHoldQty };
            return productQty;
        }

        /// <summary>
        /// 当订单状态变更为DeliveryArranged,已安排发货,计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>  ---not ok
        async Task<ProductQty> CalculateQtyWhenOrderDeliveryArranged(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            if (InvtHoldQty < 0) InvtHoldQty = 0;

            //取出实际库存
            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            //计算实际库存
            InvtActualQty = InvtActualQty - Qty < 0 ? 0 : InvtActualQty - Qty;
            //设置实际库存
            await RedisHelper.ZAddAsync(InvtActualQtyKey, (InvtActualQty, SkuId));

            //取出预留数
            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            //计算预留数
            InvtReservedQty = InvtReservedQty - Qty < 0 ? 0 : InvtReservedQty - Qty;
            //设置预留数
            await RedisHelper.ZAddAsync(InvtReservedQtyKey, (InvtReservedQty, SkuId));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;

            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId.ToString()));

            var productQty = new ProductQty { InvtReservedQty = (int)InvtReservedQty, InvtActualQty = (int)InvtActualQty, SalesQty = (int)SalesQty, SkuId = SkuId, };

            return productQty;
        }

        /// <summary>
        /// 当订单取消时(支付后取消)，回滚预留数，计算缓存中的库存
        /// </summary>
        /// <param name="SkuId"></param>       
        /// <param name="Qty"></param>
        /// <returns></returns>  
        async Task<ProductQty> CalculateWhenOrderCancel(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            if (InvtActualQty < 0) InvtActualQty = 0;

            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            if (InvtHoldQty < 0) InvtHoldQty = 0;

            //获取预留数
            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            //计算预留数
            InvtReservedQty = InvtReservedQty - Qty < 0 ? 0 : InvtReservedQty - Qty;
            //设置预留数并返回更新后的预留数
            await RedisHelper.ZAddAsync(InvtReservedQtyKey, (InvtReservedQty, SkuId));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId));

            var productQty = new ProductQty { InvtReservedQty = (int)InvtReservedQty, SalesQty = (int)SalesQty, SkuId = SkuId, };
            return productQty;
        }

        /// <summary>
        /// 支付超时或支付前取消，恢复Hold货数量，计算缓存中的库存
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        async Task<ProductQty> CalculateQtyWhenPayTimeOut(Guid SkuId, int Qty)
        {
            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";

            var InvtActualQty = await RedisHelper.ZScoreAsync(InvtActualQtyKey, SkuId) ?? 0;
            if (InvtActualQty < 0) InvtActualQty = 0;

            var InvtReservedQty = await RedisHelper.ZScoreAsync(InvtReservedQtyKey, SkuId) ?? 0;
            if (InvtReservedQty < 0) InvtReservedQty = 0;

            //获取Hold数
            var InvtHoldQty = await RedisHelper.ZScoreAsync(InvtHoldQtyKey, SkuId) ?? 0;
            //计算Hold数
            InvtHoldQty = InvtHoldQty - Qty < 0 ? 0 : InvtHoldQty - Qty;
            //设置Hold数
            await RedisHelper.ZAddAsync(InvtHoldQtyKey, (InvtHoldQty, SkuId));

            //设置可销售库存数
            var SalesQty = InvtActualQty - InvtReservedQty - InvtHoldQty;
            if (SalesQty < 0) SalesQty = 0;
            await RedisHelper.ZAddAsync(SalesQtyKey, (SalesQty, SkuId));

            var productQty = new ProductQty { InvtHoldQty = (int)InvtHoldQty, SalesQty = (int)SalesQty, SkuId = SkuId };
            return productQty;
        }

        #endregion

        #region 更新数据库逻辑


        /// <summary>
        /// 采购入库
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenPurchasing(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenPurchasing(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenPurchasing(result.SkuId, result.InvtActualQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 采购退回
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenPurchasingReturn(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenPurchaseReturn(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenReturn(result.SkuId, result.InvtActualQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 销售退回或发货退回
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenReturn(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenReturn(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenReturn(result.SkuId, result.InvtActualQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenAddToCart(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenAddToCart(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenAddToCart(result.SkuId, result.InvtHoldQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenDeleteCart(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenDeleteCart(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenDeleteCart(result.SkuId, result.InvtHoldQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 修改购物车数量
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenModifyCart(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenModifyCart(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenModifyCart(result.SkuId, result.InvtHoldQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 支付成功后
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenPay(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenPaymentConfirmed(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenPay(result.SkuId, result.InvtReservedQty, result.SalesQty, result.InvtHoldQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 已安排发货
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenDeliveryArranged(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenOrderDeliveryArranged(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenDeliveryArranged(result.SkuId, result.InvtReservedQty, result.InvtActualQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 支付后，取消订单
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenOrderCancel(TmpProductQty tmpProductQty)
        {
            var result = await CalculateWhenOrderCancel(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenOrderCancel(result.SkuId, result.InvtReservedQty, result.SalesQty, tmpProductQty.MsgId);
        }

        /// <summary>
        /// 支付超时或支付前取消订单
        /// </summary>
        /// <param name="tmpProductQty"></param>
        /// <returns></returns>
        async Task<int> UpdateQtyWhenPayTimeOut(TmpProductQty tmpProductQty)
        {
            var result = await CalculateQtyWhenPayTimeOut(tmpProductQty.SkuId, tmpProductQty.Qty);
            return await ProductQtyRepository.UpdateQtyWhenPayTimeOut(result.SkuId, result.InvtHoldQty, result.SalesQty, tmpProductQty.MsgId);
        }

        #endregion

        /// <summary>
        /// 补偿回写Qty
        /// </summary>
        /// <returns></returns>
        public async Task<SystemResult> HandleQtyAsync()
        {
            var result = new SystemResult();

            string queue = MQSetting.UpdateQtyQueue;
            string exchange = MQSetting.UpdateQtyExchange;

            var list = await baseRepository.GetListAsync<PushMessage>(x => x.State == MQState.UnDeal && x.QueueName == queue
                          && x.ExchangeName == exchange);

            var query = list.OrderBy(o => o.CreateDate).Take(100).ToList();
            if (query != null && query.Any())
            {
                this.Logger.LogInformation($"一共{query.Count()}条");
                foreach (var item in query)
                {
                    this.Logger.LogInformation($"正在发送MQ消息,队列={queue},消息={item.Id}");
                    rabbitMQService.PublishMsg(queue, exchange, item.Id.ToString());
                }
                this.Logger.LogInformation($"全部发送完了.....");
                result.Succeeded = true;
            }
            return result;
        }
    }
}
