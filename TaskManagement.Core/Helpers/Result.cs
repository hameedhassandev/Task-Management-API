using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class Result<T>
    {
        public Result(bool isSuccessful, T value)
        {
            IsSuccessful = isSuccessful;
            Value = value;
            Message = string.Empty;
        }
        public Result(bool isSuccessful, string message, T value)
       : this(isSuccessful, value)
        {
            Message = message;
        }

        public Result(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Value = default;
            Message = message;
        }

        public Result(bool isSuccessful, string message, Error error)
          : this(isSuccessful, message)
        {
            Error = error;
        }

        public Result(bool isSuccessful, string message, T value, Error error)
           : this(isSuccessful, message, value)
        {
            Error = error;
        }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T? Value { get; set; }
        public Error? Error { get; set; }
     

        public static Result<T> Success(string message, T value)
        {
            return new Result<T>(true, message, value);
        }

        public static Result<T> Success(string message)
        {
            return new Result<T>(true, message);
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>(false, message);
        }

        public static Result<T> Failure(string message, Error error)
        {
            return new Result<T>(false, message, error);
        }
    }
}
