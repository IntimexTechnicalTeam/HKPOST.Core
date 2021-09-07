using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Domain.Enums
{
    public enum InvReservedState
    {
        /// <summary>
        /// 預留中
        /// </summary>
        RESERVED = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        FINISH = 2,
        /// <summary>
        /// 已取消
        /// </summary>
        CANCEL = 3
    }
}
