namespace User_Management.Model;

public interface IUserRepository
{
    public  Task<bool> UserExists(string uuid);

    public  Task<User?> FindUserByUuid(string email);

    public Task AddUser(RegisterUser user);


   public Task<User?> FindUserByEmail(string email);
}
