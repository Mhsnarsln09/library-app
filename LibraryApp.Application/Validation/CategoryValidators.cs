using FluentValidation;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Validation;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Must(name => name?.Trim().Length >= 2).WithMessage("Name must be at least 2 characters long.");
    }
}

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Must(name => name?.Trim().Length >= 2).WithMessage("Name must be at least 2 characters long.");
    }
}
