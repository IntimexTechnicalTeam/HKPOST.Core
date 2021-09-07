using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Web.Framework
{
    /// <summary>
    /// 分页基类
    /// </summary>
    [DataContract]
    public class PageModel<T> where T :class,new()
    {
        /// <summary>
        /// 页码
        /// </summary>

        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页多少条
        /// </summary>

        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; } = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalCount / (decimal)PageSize);
            }
            set { }
        }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> Data = new List<T>();
    }
}
