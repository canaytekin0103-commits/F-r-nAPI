using FirinApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirinApi.Data;

/// <summary>
/// Fırındaki günlük satış ürünleri — fiyat veya ürün eklemek için bu listeyi düzenleyin.
/// </summary>
public static class BakerySeedData
{
    public static IReadOnlyList<(string Name, decimal Price, int Stock)> Products { get; } =
    [
        ("Kaşarlı Poğaça", 20m, 100),
        ("Peynirli Poğaça", 20m, 100),
        ("Sade Poğaça", 20m, 100),
        ("Simit", 20m, 100),
        ("Sade Açma", 20m, 100),
        ("Çikolatalı Açma", 20m, 100),
        ("Zeytinli Açma", 20m, 100),
        ("Ekmek", 20m, 50),
        ("Çavdar", 20m, 50),
        ("Kepek", 20m, 50),
    ];

    public static async Task EnsureProductsAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var existing = await db.Breads.ToListAsync(cancellationToken);
        var byName = existing.ToDictionary(b => b.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var (name, price, stock) in Products)
        {
            if (byName.TryGetValue(name, out var bread))
            {
                bread.Price = price;
                continue;
            }

            db.Breads.Add(new Bread { Name = name, Price = price, StockQuantity = stock });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
