using Microsoft.EntityFrameworkCore;

namespace User_Management.Model;

public class UserRepository : IUserRepository
{
    private readonly UserManagementDbContext _dbContext;

    public UserRepository(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UserExists(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email) != null;
    }

    public async Task<User?> FindUserByUuid(string uuid)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Uuid == new Guid(uuid));
    }

    public async Task AddUser(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }
}
