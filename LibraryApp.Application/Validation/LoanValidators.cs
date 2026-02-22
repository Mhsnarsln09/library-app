using FluentValidation;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Validation;

public class CreateLoanDtoValidator : AbstractValidator<CreateLoanDto>
{
    public CreateLoanDtoValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0).WithMessage("Valid BookId is required.");

        RuleFor(x => x.MemberId)
            .GreaterThan(0).WithMessage("Valid MemberId is required.");
    }
}
