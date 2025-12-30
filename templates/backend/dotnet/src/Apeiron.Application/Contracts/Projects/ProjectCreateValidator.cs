using FluentValidation;

namespace Apeiron.Application.Contracts.Projects;

public class ProjectCreateValidator : AbstractValidator<ProjectCreateRequest>
{
    public ProjectCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters.");
            
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be less than 500 characters.");
    }
}
