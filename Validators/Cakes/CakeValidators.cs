using FirinApi.Models.DTOs.Cakes;
using FluentValidation;

namespace FirinApi.Validators.Cakes;

public class CreateCakeValidator : AbstractValidator<CreateCakeDto>
{
    public CreateCakeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pasta adı zorunludur.")
            .MaximumLength(100).WithMessage("Pasta adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");
    }
}

public class UpdateCakeValidator : AbstractValidator<UpdateCakeDto>
{
    public UpdateCakeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pasta adı zorunludur.")
            .MaximumLength(100).WithMessage("Pasta adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");
    }
}
