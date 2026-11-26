// using readiness.repository;
//
// namespace readiness.service;
//
// public class UserService
// {
//     private readonly IUserRepository _repository;
//
//     public UserService(IUserRepository repository)
//     {
//         _repository = repository;
//     }
//
//     // TODO: No Longer needed since claims already exist
//     // public async Task<bool> IsUserModerator(long userId)
//     // {
//     //     User user = await _repository.GetUserByIdAsync(userId);
//     //     return user.Role.Equals(Role.Administrator);
//     // }
// }