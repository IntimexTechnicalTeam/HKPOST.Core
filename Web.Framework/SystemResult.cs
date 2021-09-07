using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Framework
{
    public class SystemResult
    {
        [JsonProperty(PropertyName = "Succeeded")]
        public bool Succeeded { get; set; } = false;

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "ReturnValue")]
        public object ReturnValue { get; set; }

    }

    public class SystemResult<T> : SystemResult
    { 
        public new T ReturnValue { get; set; }
    }

}
