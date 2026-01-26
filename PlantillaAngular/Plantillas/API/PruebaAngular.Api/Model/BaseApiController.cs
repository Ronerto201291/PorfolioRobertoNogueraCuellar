using Microsoft.AspNetCore.Mvc;

namespace PruebaAngular.Api.Model
{
    public class ApiDataResult<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }

        public ApiDataResult()
        {
            Success = true;
        }

        public ApiDataResult(T data, string message = null)
        {
            Data = data;
            Message = message;
            Success = true;
        }
    }

    public abstract class BaseApiController : ControllerBase
    {
        protected ApiDataResult<T> CreateApiResult<T>(T data, string message = null)
        {
            return new ApiDataResult<T>(data, message);
        }

        protected ApiDataResult<object> CreateApiResult(object data, string message = null)
        {
            return new ApiDataResult<object>(data, message);
        }
    }
}
