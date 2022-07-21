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
    public class PushMessage : BaseEntity<Guid>
    {

        /// <summary>
        /// 可以是SkuId,OrderId等
        /// </summary>
        public Guid ItemId { get; set; }

        ///<summary>
        ///队列名称
        ///</summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar")]
        public string QueueName { get; set; }

        ///<summary>
        ///路由名称
        ///</summary>
        [MaxLength(100)]
        [Column(TypeName = "nvarchar")]
        public string ExchangeName { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        public MQState State { get; set; } = MQState.UnDeal;

        public MQType MsgType { get; set; } = MQType.None;

        ///<summary>
        ///消息内容
        ///</summary>
        public string MsgContent { get; set; }

        ///<summary>
        ///重试次数
        ///</summary>
        public int Retries { get; set; } = 0;

        [MaxLength(100)]
        [Column(TypeName = "nvarchar")]
        public string Remark { get; set; }
    }
}

