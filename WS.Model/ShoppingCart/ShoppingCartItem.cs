using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WS.Model
{
    /// <summary>
    /// 购物车
    /// </summary>
    public class ShoppingCartItem : BaseEntity<Guid>
    {
        /// <summary>
        ///  会员ID/临时ID
        /// </summary>
        [Column(Order = 3)]
        public Guid MemberId { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [Column(Order = 4)]
        public Guid ProductId { get; set; }

        /// <summary>
        /// 单品ID
        /// </summary>
        [Column(Order = 5)]
        public Guid SkuId { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        [Column(Order = 6)]
        public int Qty { get; set; }

        
    }
}
