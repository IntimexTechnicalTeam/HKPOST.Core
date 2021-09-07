using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Domain.Enums
{
    public enum MQType
    {
        None =0,
        UpdateInvt=1,
    }

    public enum MQState
    {
        /// <summary>
        /// 未处理
        /// </summary>
        UnDeal,
        /// <summary>
        /// 已处理
        /// </summary>
        Deal,
        /// <summary>
        /// 异常
        /// </summary>
        Exception
    }
}
