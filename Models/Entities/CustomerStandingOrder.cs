namespace FirinApi.Models.Entities;

/// <summary>
/// Müşterinin her gün aldığı sabit ürün listesi.
/// </summary>
public class CustomerStandingOrder : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int BreadId { get; set; }
    public Bread Bread { get; set; } = null!;
    public int Quantity { get; set; }
}
