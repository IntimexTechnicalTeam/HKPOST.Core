using System;
using System.Collections.Generic;
using System.Text;

namespace Web.MQ
{
    public static class MQSettings
    {
       
        /// <summary>
        /// 入库单队列
        /// </summary>
        public static string InStcokOrderQueue { get { return "InStcokOrder.Queue"; } }

        public static string InStcokOrderExchange { get { return "InStcokOrder.Exchange"; } }


        /// <summary>
        /// 销售单队列
        /// </summary>
        public static string SaleOrderQueue { get { return "SaleOrder.Queue"; } }

        public static string SaleOrderExchange { get { return "SaleOrder.Exchange"; } }
    }
}
