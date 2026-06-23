using FirinApi.Models.DTOs.StandingOrders;
using FluentValidation;

namespace FirinApi.Validators.StandingOrders;

public class CreateStandingOrderValidator : AbstractValidator<CreateStandingOrderDto>
{
    public CreateStandingOrderValidator()
    {
        RuleFor(x => x.BreadId).GreaterThan(0).WithMessage("Geçerli bir ürün seçiniz.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Adet en az 1 olmalıdır.");
    }
}

public class UpdateStandingOrderValidator : AbstractValidator<UpdateStandingOrderDto>
{
    public UpdateStandingOrderValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Adet en az 1 olmalıdır.");
    }
}
