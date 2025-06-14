using DietaCore.Dto.MealDTOs;
using FluentValidation;

namespace DietaCore.Business.ValidationRules
{
    public class MealRequestValidator : AbstractValidator<MealRequestDto>
    {
        public MealRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .Must((model, endTime) => endTime > model.StartTime).WithMessage("End time must be after start time.");

            RuleFor(x => x.Contents)
                .NotEmpty().WithMessage("Contents is required.")
                .MaximumLength(500).WithMessage("Contents cannot exceed 500 characters.");

            RuleFor(x => x.Calories)
                .NotEmpty().WithMessage("Calories is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Calories cannot be negative.");

            RuleFor(x => x.Proteins)
                .NotEmpty().WithMessage("Proteins is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Proteins cannot be negative.");

            RuleFor(x => x.Carbohydrates)
                .NotEmpty().WithMessage("Carbohydrates is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Carbohydrates cannot be negative.");

            RuleFor(x => x.Fats)
                .NotEmpty().WithMessage("Fats is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Fats cannot be negative.");

            RuleFor(x => x.DietPlanId)
                .NotEmpty().WithMessage("Diet plan ID is required.");
        }
    }
}
