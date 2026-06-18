using FirinApi.Data;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FirinApi.Repositories.Implementations;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
