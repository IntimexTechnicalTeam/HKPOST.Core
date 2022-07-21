using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Domain.Enums;

namespace WS.Model.MQMessage
{
    public class TmpProductQty
    {
        /// <summary>
        /// OrderId,ShoppingCartId
        /// </summary>
        public Guid Id { get; set; }

        public Guid SkuId { get; set; }

        public int Qty { get; set; }

        public MQType MsgType { get; set; }

        /// <summary>
        /// 一般为PushMessage.Id
        /// </summary>
        public Guid MsgId { get; set; }= Guid.Empty;
    }



}
