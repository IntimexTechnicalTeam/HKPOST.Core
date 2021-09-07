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
    /// 庫存資料
    /// </summary>
    public class Inventory : BaseEntity<Guid>
    {
        /// <summary>
        /// 倉庫記錄ID
        /// </summary>
        [Required]
        [Column(Order = 3)]
        public Guid WHId { get; set; }
       
        /// <summary>
        /// Sku
        /// </summary>
        [Required]
        [Column(Order = 4)]
        public Guid Sku { get; set; }
        
        /// <summary>
        /// 倉存數量
        /// </summary>
        [Required]
        [Column(Order = 5)]
        public int Quantity { get; set; }

        /// <summary>
        /// 商家ID
        /// </summary>
        [Required]
        [Column(Order = 6)]
        public Guid MerchantId { get; set; }
    }
}
