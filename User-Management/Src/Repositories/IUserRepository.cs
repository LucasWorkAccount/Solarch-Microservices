namespace User_Management.Model;

public interface IUserRepository
{
    public  Task<bool> UserExists(string email);

    public  Task<User?> FindUserByUuid(string uuid);

    public Task AddUser(User user);


   public Task<User?> FindUserByEmail(string email);

   public Task EditUser(User user);
}
