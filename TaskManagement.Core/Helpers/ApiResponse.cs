﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helpers
{
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Success(string message, T? data = default, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? StatusCode { get; set; }

        public static ApiResponse Success(string message, int? statusCode)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode ?? 200
            };
        }
    }

}
