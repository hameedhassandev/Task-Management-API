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
            StatusCode = 200;
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

        public Result(bool isSuccessful, string message, string errorType, T value, int statusCode)
           : this(isSuccessful, message, value)
        {
            ErrorType = errorType;
            StatusCode = statusCode;
        }
        public Result(bool isSuccessful, string message, string errorType, int statusCode)
           : this(isSuccessful, message)
        {
            ErrorType = errorType;
            StatusCode = statusCode;
        }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T? Value { get; set; }
        public string? ErrorType { get; set; }
        public int? StatusCode { get; set; }

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

        public static Result<T> Failure(string message, string errorType, int statusCodes)
        {
            return new Result<T>(false, message, errorType, statusCodes);
        }
    }
}
