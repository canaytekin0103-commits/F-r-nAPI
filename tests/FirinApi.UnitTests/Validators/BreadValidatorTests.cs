using FirinApi.Models.DTOs.Breads;
using FirinApi.Validators.Breads;

namespace FirinApi.UnitTests.Validators;

public class BreadValidatorTests
{
    private readonly CreateBreadValidator _validator = new();

    [Fact]
    public void CreateBread_BosIsim_HataVerir()
    {
        var dto = new CreateBreadDto("", 10m, 5);

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void CreateBread_NegatifFiyat_HataVerir()
    {
        var dto = new CreateBreadDto("Simit", -5m, 10);

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public void CreateBread_GecerliVeri_HataVermez()
    {
        var dto = new CreateBreadDto("Simit", 15m, 100);

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }
}
