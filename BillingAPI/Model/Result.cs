using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public ResultStatus Status { get; set; }
        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Status={Status}, Message={Message}";
        }
    }

    [DataContract]
    public class DataResult<T> : Result
    {
        [DataMember]
        public T Data { get; set; }
    }

    public enum ResultStatus
    {
        Success = 200,
        Error = 400
    }
}
