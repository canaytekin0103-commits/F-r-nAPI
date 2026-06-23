using FirinApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FirinApi.Data.Configurations;

public class CustomerStandingOrderConfiguration : IEntityTypeConfiguration<CustomerStandingOrder>
{
    public void Configure(EntityTypeBuilder<CustomerStandingOrder> builder)
    {
        builder.HasIndex(s => new { s.CustomerId, s.BreadId }).IsUnique();

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.StandingOrders)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Bread)
            .WithMany()
            .HasForeignKey(s => s.BreadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
