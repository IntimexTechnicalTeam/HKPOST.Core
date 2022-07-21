using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Model
{
    /// <summary>
    /// 產品數量匯總
    /// </summary>
    public class ProductQty : BaseEntity<Guid>
    {
        /// <summary>
        /// 庫存單元編碼
        /// </summary>
        [Required]
        [Column(Order = 3)]
        public Guid SkuId { get; set; }
        /// <summary>
        /// 實際庫存數量
        /// </summary>
        [Required]
        [Column(Order = 4)]
        public int InvtActualQty { get; set; }
        /// <summary>
        /// 可銷售數量
        /// </summary>
        [Required]
        [Column(Order = 5)]
        public int SalesQty { get; set; }
        /// <summary>
        /// 庫存預留數量
        /// </summary>
        [Required]
        [Column(Order = 6)]
        public int InvtReservedQty { get; set; }
        /// <summary>
        /// 庫存留貨數量
        /// </summary>
        [Required]
        [Column(Order = 7)]
        public int InvtHoldQty { get; set; }
        /// <summary>
        /// 時間戳
        /// </summary>
        [Timestamp]
        [Column(Order = 8)]        
        public byte[] VersionId { get; set; }
    }
}
