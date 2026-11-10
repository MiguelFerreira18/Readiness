namespace readiness.repository;

//TODO:DELETE ON MERGE SINCE THIS IS A MOCK UP

public interface IUserRepository
{
    public Task<User> GetUserByIdAsync(long id);
}

public class UserRepository
{
    
}