using FirinApi.Models.DTOs.Breads;
using FluentValidation;

namespace FirinApi.Validators.Breads;

public class CreateBreadValidator : AbstractValidator<CreateBreadDto>
{
    public CreateBreadValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ekmek adı zorunludur.")
            .MaximumLength(100).WithMessage("Ekmek adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");
    }
}

public class UpdateBreadValidator : AbstractValidator<UpdateBreadDto>
{
    public UpdateBreadValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ekmek adı zorunludur.")
            .MaximumLength(100).WithMessage("Ekmek adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");
    }
}
