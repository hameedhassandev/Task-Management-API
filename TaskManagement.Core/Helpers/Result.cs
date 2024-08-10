using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class Result<T>
    {
        public Result(bool isTrue, T value)
        {
            IsTrue = isTrue;
            Value = value;
            Message = string.Empty;
        }
        public Result(bool isTrue, string message, T value)
       : this(isTrue, value)
        {
            Message = message;
        }

        public Result(bool isTrue, string message)
        {
            IsTrue = isTrue;
            Value = default; 
            Message = message;
        }

        public Result(bool isTrue, string message, string errorType, T value)
           : this(isTrue, message, value)
        {
            ErrorType = errorType;
        }
        public Result(bool isTrue, string message, string errorType)
           : this(isTrue, message)
        {
            ErrorType = errorType;
        }
        public bool IsTrue { get; set; }
        public string Message { get; set; }
        public T? Value { get; set; }
        public string? ErrorType { get; set; }

    }
}
