using FluentValidation.Results;

namespace DietaCore.Shared.Extensions
{
    public static class ValidationExtensions
    {
        public static string[] GetErrorMessages(this ValidationResult validationResult)
        {
            return validationResult.Errors.Select(x => x.ErrorMessage).ToArray();
        }
    }
}
