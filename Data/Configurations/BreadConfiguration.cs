using FirinApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FirinApi.Data.Configurations;

public class BreadConfiguration : IEntityTypeConfiguration<Bread>
{
    public void Configure(EntityTypeBuilder<Bread> builder)
    {
        builder.Property(b => b.Name).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Price).HasPrecision(18, 2);
    }
}
