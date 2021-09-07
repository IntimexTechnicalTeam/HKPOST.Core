using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Framework
{
    public class ServiceException : Exception
    {
        public ServiceException()
        {

        }
        public ServiceException(string msg) : base(msg)
        {

        }
    }

    public class ApiServiceException : Exception
    {
        public ApiServiceException()
        {

        }
        public ApiServiceException(int errorCode, string msg) : base(msg)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
    }

    public class InvalidInputException : Exception
    {
        public InvalidInputException()
        {
        }
        public InvalidInputException(string message)
            : base(message)
        {
        }
    }
}