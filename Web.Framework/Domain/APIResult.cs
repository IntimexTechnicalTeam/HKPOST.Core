using System;
using System.Runtime.Serialization;

namespace Web.Framework
{

    /// <summary>
    /// 返回参数
    /// </summary>
    [Serializable]
    [DataContract]
    public class APIResult
    {
        /// <summary>
        /// 返回参数
        /// </summary>
        public APIResult()
        {
            this.servertime = DateTime.UtcNow.Ticks;
        }
        /// <summary>
        /// 0成功，其它的是出错
        /// </summary>
        [DataMember]
        public int error
        {
            get;
            set;
        }
        /// <summary>
        /// 返回的成功或错误提示信息
        /// </summary>
        [DataMember]
        public string msg
        {
            get;
            set;
        }
        /// <summary>
        /// 10位服务的时间戳
        /// </summary>
        [DataMember]
        public long servertime
        {
            get;
            set;
        }
        ///<summary>
        ///签名
        ///</summary>
        [DataMember]
        public string sign
        {
            get;
            set;
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static APIResult SuccessMsg(string msg = null)
        {
            APIResult ret = new APIResult();
            ret.error = 0;
            ret.msg = msg;

            return ret;
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static APIResult<T> SuccessMsg<T>(T data)
        {
            APIResult<T> ret = new APIResult<T>();
            ret.error = 0;
            ret.data = data;

            return ret;
        }

        /// <summary>
        /// 返回失败消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static APIResult FailMsg(string msg = null)
        {
            APIResult ret = new APIResult();
            ret.error = -1;
            ret.msg = msg;

            return ret;
        }
    }
    /// <summary>
    /// 返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class APIResult<T> : APIResult
    {
        /// <summary>
        /// 数据
        /// </summary>
        [DataMember(Name = "data")]
        public virtual T data { get; set; }
    }
    /// <summary>
    /// 分页返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class APIPagedResult<T> : APIResult where T : class, new()
    {

        /// <summary>
        /// 
        /// </summary>
        public APIPagedResult()
        {
            this.data = new Data();
        }

        /// <summary>
        /// 数据
        /// </summary>
        [DataMember(Name = "data")]
        public virtual Data data { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [Serializable]
        [DataContract]
        public class Data
        {
            /// <summary>
            /// 数据
            /// </summary>
            //[Display(Name = "数据")]
            [DataMember]
            public virtual T data { get; set; }

            /// <summary>
            /// 总页数
            /// </summary>
            //[Display(Name = "总页数")]
            [DataMember]
            public int totalPage { get; set; }

            /// <summary>
            /// 总记录数
            /// </summary>
            //[Display(Name = "总记录数")]
            [DataMember]
            public int totalRecord { get; set; }
        }

    }
}

