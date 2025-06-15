using DietaCore.Dto.ClientProgressDTOs;
using FluentValidation;

namespace DietaCore.Business.ValidationRules
{
    public class ProgressDtoValidator : AbstractValidator<ProgressDto>
    {
        public ProgressDtoValidator()
        {
            RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required.")
            .GreaterThan(0).WithMessage("Client ID must be a positive integer.");

            RuleFor(x => x.Weight)
                .NotEmpty().WithMessage("Weight is required.")
                .GreaterThan(0).WithMessage("Weight must be a positive number.");

            RuleFor(x => x.BodyFat)
                .GreaterThanOrEqualTo(0).WithMessage("Body fat percentage cannot be negative.")
                .LessThanOrEqualTo(100).WithMessage("Body fat percentage cannot exceed 100.");

            RuleFor(x => x.Muscle)
                .GreaterThanOrEqualTo(0).WithMessage("Muscle mass cannot be negative.")
                .LessThanOrEqualTo(100).WithMessage("Muscle mass cannot exceed 100.");

            RuleFor(x => x.Waist)
                .GreaterThanOrEqualTo(0).WithMessage("Waist measurement cannot be negative.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be in the future");
        }
    }
}
