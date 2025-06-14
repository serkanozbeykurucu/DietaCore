using DietaCore.Shared.Common.Responses.ComplexTypes;

namespace DietaCore.Shared.Common.Responses.Abstract
{
    public interface IServiceResponse
    {
        ResponseCode ResponseCode { get; }
        string Message { get; }
    }
}
