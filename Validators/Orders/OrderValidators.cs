using FirinApi.Models.DTOs.Orders;
using FluentValidation;

namespace FirinApi.Validators.Orders;

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.ProductType)
            .IsInEnum().WithMessage("Geçerli bir ürün tipi seçiniz (1=Ekmek, 2=Pasta).");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Ürün Id 0'dan büyük olmalıdır.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Ürün adedi en az 1 olmalıdır.");
    }
}

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri seçiniz.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Sipariş en az bir ürün içermelidir.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemValidator());
    }
}

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçerli bir sipariş durumu seçiniz.");
    }
}
