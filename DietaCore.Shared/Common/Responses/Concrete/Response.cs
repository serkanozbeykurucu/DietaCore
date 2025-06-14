using DietaCore.Shared.Common.Responses.Abstract;
using DietaCore.Shared.Common.Responses.ComplexTypes;

namespace DietaCore.Shared.Common.Responses.Concrete
{
    public class Response<T> : IServiceResponse
    {
        public ResponseCode ResponseCode { get; }
        public string Message { get; }
        public T? Data { get; }
        public Response(ResponseCode responseCode)
        {
            ResponseCode = responseCode;
            Message = responseCode.ToString();
        }
        public Response(ResponseCode responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }
        public Response(ResponseCode responseCode, T data)
        {
            ResponseCode = responseCode;
            Data = data;
            Message = responseCode.ToString();
        }
        public Response(ResponseCode responseCode, T data, string message)
        {
            ResponseCode = responseCode;
            Data = data;
            Message = message;
        }
    }
    public class Response : IServiceResponse
    {
        public ResponseCode ResponseCode { get; }
        public string Message { get; }

        public Response(ResponseCode responseCode)
        {
            ResponseCode = responseCode;
            Message = responseCode.ToString();
        }

        public Response(ResponseCode responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }
    }
}
