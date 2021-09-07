using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Model.MQMessage
{
    public class TmpProductQty
    {
        /// <summary>
        /// PushMessage.Id
        /// </summary>
        public Guid Id { get; set; }

        public Guid SkuId { get; set; }

        public int InvtActualQty { get; set; }

        public int SalesQty { get; set; }

        public int InvtReservedQty { get; set; }

        public int InvtHoldQty { get; set; }

        public QtyType QtyType { get; set; }

    }

    public enum QtyType {

        WhenPurchasing,
        WhenAddToCart,
        WhenDeleteCart,
        WhenModifyCart,
        WhenPay,
        WhenOrderComplate,
        WhenOrderCancel
    }

}
