using DietaCore.Dto.DietPlanDTOs;
using FluentValidation;

namespace DietaCore.Business.ValidationRules
{
    public class DietPlanRequestValidator : AbstractValidator<DietPlanRequestDto>
    {
        public DietPlanRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(startDate => startDate.Date >= DateTime.Today).WithMessage("Start date must be today or in the future.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .Must((model, endDate) => endDate > model.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x.InitialWeight)
                .NotEmpty().WithMessage("Initial weight is required.")
                .GreaterThan(0).WithMessage("Initial weight must be greater than zero.");

            RuleFor(x => x.TargetWeight)
                .NotEmpty().WithMessage("Target weight is required.")
                .GreaterThan(0).WithMessage("Target weight must be greater than zero.");

            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("Client ID is required.");
        }
    }
}
