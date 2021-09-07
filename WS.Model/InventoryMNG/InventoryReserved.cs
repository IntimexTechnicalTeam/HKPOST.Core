using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Domain.Enums;

namespace WS.Model
{
    /// <summary>
    /// 庫存預留記錄
    /// </summary>
    public class InventoryReserved : BaseEntity<Guid>
    {
        /// <summary>
        /// 庫存單元編碼
        /// </summary>
        [Required]
        [Column(Order = 3)]
        public Guid Sku { get; set; }
        //[ForeignKey("Sku")]
        //public virtual ProductSku ProductSkuInfo { get; set; }
        /// <summary>
        /// 訂單記錄ID
        /// </summary>
        [Required]
        [Column(Order = 4)]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 訂單資料
        /// </summary>
        //[ForeignKey("OrderId")]
        //public virtual Order Order { get; set; }
        /// <summary>
        /// 預留數量
        /// </summary>
        [Required]
        [Column(Order = 5)]
        public int ReservedQty { get; set; }
        /// <summary>
        /// 預留類型
        /// </summary>
        [Required]
        [Column(Order = 6)]
        public InvReservedType ReservedType { get; set; }
        /// <summary>
        /// 預留處理狀態
        /// </summary>
        [Required]
        [Column(Order = 7)]
        public InvReservedState ProcessState { get; set; }

        [Column(Order = 8)]
        public Guid SubOrderId { get; set; }
        /// <summary>
        /// 倉庫ID
        /// </summary>
        [NotMapped]
        public Guid? WHId { get; set; }
    }
}
