using FluentValidation;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Validation;

public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
{
    public CreateAuthorDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Name is required.")
            .Must(name => name?.Trim().Length >= 2).WithMessage("Name must be at least 2 characters long.");
    }
}

public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
{
    public UpdateAuthorDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Name is required.")
            .Must(name => name?.Trim().Length >= 2).WithMessage("Name must be at least 2 characters long.");
    }
}
