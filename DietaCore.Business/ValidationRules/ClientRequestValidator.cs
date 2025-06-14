using DietaCore.Dto.ClientDTOs;
using FluentValidation;

namespace DietaCore.Business.ValidationRules
{
    public class ClientRequestValidator : AbstractValidator<ClientRequestDto>
    {
        public ClientRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Please enter a valid phone number.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(dob => dob < DateTime.Now.AddYears(-18)).WithMessage("Client must be at least 18 years old.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .MaximumLength(20).WithMessage("Gender cannot exceed 20 characters.");

            RuleFor(x => x.Height)
                .NotEmpty().WithMessage("Height is required.")
                .GreaterThan(0).WithMessage("Height must be greater than zero.");

            RuleFor(x => x.InitialWeight)
                .NotEmpty().WithMessage("Initial weight is required.")
                .GreaterThan(0).WithMessage("Initial weight must be greater than zero.");
        }
    }
}
