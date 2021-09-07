using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;
using WS.BLL.Interface;
using WS.Domain.Enums;
using WS.Domain.PreHeat;
using WS.Model;

namespace WS.BLL.Impl
{
    public class PreHotProductQtyBll : ServiceBase , IPreHotProductQtyBll
    {
        IServiceProvider _service;

        public PreHotProductQtyBll(IServiceProvider services) : base(services)
        {
            this._service = services;           
        }


        public async Task<SystemResult> CreatePreHeat()
        {
            var result = new SystemResult();

            var InvtActualQtyLst = baseRepository.GetList<Inventory>(x => x.IsActive && !x.IsDeleted).Select(s => new PreProductQty { SkuId = s.Sku, InvtActualQty = s.Quantity });

            var InvtReservedQtyLst = baseRepository.GetList<InventoryReserved>(x => x.IsActive && !x.IsDeleted && x.ProcessState == InvReservedState.RESERVED)
                                               .GroupBy(x => x.Sku).Select(s => new PreProductQty { SkuId = s.Key, InvtReservedQty = s.Sum(w => w.ReservedQty) });

            var InvtHoldQtyLst = baseRepository.GetList<ShoppingCartItem>(x => x.IsActive && !x.IsDeleted)
                                            .GroupBy(x => x.SkuId).Select(s => new PreProductQty { SkuId = s.Key, InvtHoldQty = s.Sum(w => w.Qty) });

            var TmpLst = from a in InvtActualQtyLst
                         join b in InvtReservedQtyLst on a.SkuId equals b.SkuId into ab
                         from c in ab.DefaultIfEmpty()
                         join d in InvtHoldQtyLst on a.SkuId equals d.SkuId into cd
                         from e in cd.DefaultIfEmpty()
                         select new PreProductQty
                         {
                             SkuId = a.SkuId ?? Guid.Empty,
                             InvtActualQty = a.InvtActualQty ?? 0,
                             InvtReservedQty = c.InvtReservedQty ?? 0,
                             InvtHoldQty = e.InvtHoldQty ?? 0,
                         };

            result = await SetDataToCache(TmpLst.ToList());

            return result;
        }

        private async Task<SystemResult> SetDataToCache(List<PreProductQty> lst)
        {
            var result = new SystemResult();

            var InvtActualQtyLst = lst.Where(x => x.InvtActualQty > 0);
            var InvtReservedQtyLst = lst.Where(x => x.InvtReservedQty > 0);
            var InvtHoldQtyLst = lst.Where(x => x.InvtHoldQty > 0);
            var SaleQtyLst = lst.Where(x => x.SaleQty > 0);

            string SalesQtyKey = $"{CacheKey.SalesQty}";
            string InvtHoldQtyKey = $"{CacheKey.InvtHoldQty}";
            string InvtActualQtyKey = $"{CacheKey.InvtActualQty}";
            string InvtReservedQtyKey = $"{CacheKey.InvtReservedQty}";
         
            await UpdateCache(InvtActualQtyLst, InvtActualQtyKey, "InvtActualQty");
            await UpdateCache(InvtReservedQtyLst, InvtReservedQtyKey, "InvtReservedQty");
            await UpdateCache(InvtHoldQtyLst, InvtHoldQtyKey, "InvtHoldQty");
            await UpdateCache(SaleQtyLst, SalesQtyKey, "SaleQty");

            return result;
        }

        private async Task<SystemResult> UpdateCache(IEnumerable<PreProductQty> Lst, string cacheKey, string fieldName)
        {
            var result = new SystemResult();
            foreach (var item in Lst)
            {
                this.Logger.LogInformation($"开始处理Sku={item.SkuId}的{fieldName}");

                decimal qty = Convert.ToDecimal(item.GetType().GetProperty(fieldName).GetValue(item));
                await RedisHelper.ZAddAsync(cacheKey, (qty, item.SkuId.ToString()));

                this.Logger.LogInformation($"成功处理Sku={item.SkuId}的{fieldName}库存值：{qty}");
            }

            return result;
        }
    }
}
