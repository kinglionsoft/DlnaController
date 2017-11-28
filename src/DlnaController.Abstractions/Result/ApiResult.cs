using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DlnaController.Abstractions
{
    /// <summary>
    /// Http 返回数据
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 状态码，默认情况下大于等于0时表示成功
        /// </summary>
        /// <returns></returns>
        public int Code { get; set; }

        /// <summary>
        /// 状态码说明
        /// </summary>
        /// <returns></returns>
        public string Message { get; set; }
        
        public bool Success() => Code >= 0;

        public ApiResult() : this(0)
        { }

        public ApiResult(int code, string message = "ok")
        {
            Code = code;
            Message = message;
        }
    }

    /// <summary>
    /// Http 返回数据
    /// </summary>
    public sealed class ApiResult<T> : ApiResult // where T : DtoBase
    {
        public T Data { get; set; }

        public ApiResult(T data, int code = 0, string message = "ok") : base(code, message)
        {
            Data = data;
        }
    }
}