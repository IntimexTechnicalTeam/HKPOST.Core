using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Model
{
    /// <summary>
    /// 非會員購物車
    /// </summary>
    public class NonMbrShoppingCartItem : BaseEntity<Guid>
    {
        /// <summary>
        ///  顧客臨時ID
        /// </summary>
        [Column(Order = 3)]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 產品ID
        /// </summary>
        [Column(Order = 4)]
        public Guid ProductId { get; set; }

        /// <summary>
        /// SKU ID
        /// </summary>
        [Column(Order = 5)]
        public Guid SkuId { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        [Column(Order = 6)]
        public int Qty { get; set; }

       
    }
}
