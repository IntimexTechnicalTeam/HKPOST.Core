using System;
using System.ComponentModel.DataAnnotations;
namespace WS.Model
{
    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderDetail : BaseEntity<Guid>
    {
        public Guid MerchantId { get; set; }
        public Guid ProductId { get; set; }

        public Guid OrderId { get; set; }

        public Guid SkuId { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 销售价格，购买价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }
         


    }
}
