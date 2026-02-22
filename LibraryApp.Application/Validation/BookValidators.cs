using FluentValidation;
using LibraryApp.Application.Contracts;

namespace LibraryApp.Application.Validation;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Must(title => title?.Trim().Length >= 2).WithMessage("Title must be at least 2 characters long.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("Valid AuthorId is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid CategoryId is required.");
            
        RuleFor(x => x.TotalCopies)
            .GreaterThanOrEqualTo(1).WithMessage("TotalCopies must be at least 1.");
    }
}

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Must(title => title?.Trim().Length >= 2).WithMessage("Title must be at least 2 characters long.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("Valid AuthorId is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Valid CategoryId is required.");
            
        RuleFor(x => x.TotalCopies)
            .GreaterThanOrEqualTo(1).WithMessage("TotalCopies must be at least 1.");
    }
}
