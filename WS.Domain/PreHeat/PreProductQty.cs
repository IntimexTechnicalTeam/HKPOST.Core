using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Domain.PreHeat
{
    public  class PreProductQty
    {
        public Guid? SkuId { get; set; } = Guid.Empty;

        /// <summary>
        /// 实际库存
        /// </summary>
        public int? InvtActualQty { get; set; } = 0;

        /// <summary>
        /// 预留数
        /// </summary>
        public int? InvtReservedQty { get; set; } = 0;


        public int? InvtHoldQty { get; set; }= 0;


        /// <summary>
        /// 可销售库存
        /// </summary>
        public int? SaleQty => (InvtActualQty - InvtReservedQty - InvtHoldQty) < 0 ? 0 : (InvtActualQty - InvtReservedQty - InvtHoldQty);
    }
}
