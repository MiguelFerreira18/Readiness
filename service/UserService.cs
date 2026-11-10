using readiness.repository;

namespace readiness.service;

public interface IUserService
{
    public Task<bool> IsUserModerator(long userId);
}




public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsUserModerator(long userId)
    {
        User user = await _repository.GetUserByIdAsync(userId);
        return user.Role.Equals(Role.Moderator);
    }
}