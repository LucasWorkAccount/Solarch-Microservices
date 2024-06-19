using Microsoft.EntityFrameworkCore;

namespace User_Management.Model;

public class UserRepository : IUserRepository
{
    private readonly UserManagementDbContext _dbContext;

    public UserRepository(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UserExists(string uuid)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Uuid == new Guid(uuid)) == null;
    }

    public async Task<User?> FindUserByUuid(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Uuid == new Guid(email));
    }

    public async Task AddUser(RegisterUser user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }
}
