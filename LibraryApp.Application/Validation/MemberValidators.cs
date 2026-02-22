using FluentValidation;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Validation;

public class UpdateMemberDtoValidator : AbstractValidator<UpdateMemberDto>
{
    public UpdateMemberDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .Must(name => name?.Trim().Length >= 2).WithMessage("FullName must be at least 2 characters long.");


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => role == LibraryApp.Domain.Constants.Roles.Admin || role == LibraryApp.Domain.Constants.Roles.Member)
            .WithMessage($"Role must be either '{LibraryApp.Domain.Constants.Roles.Admin}' or '{LibraryApp.Domain.Constants.Roles.Member}'.");
    }
}
