using FirinApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FirinApi.Data.Configurations;

public class CakeConfiguration : IEntityTypeConfiguration<Cake>
{
    public void Configure(EntityTypeBuilder<Cake> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.Price).HasPrecision(18, 2);
    }
}
