using Angular_Auth.Dto.Users;
using Angular_Auth.Models;

namespace Angular_Auth.Services.Interfaces;

public interface IUserService {
    public Task<List<UserDto>> GetUsers();
    public Task<User> GetUserById(string userId);
    public Task<User> GetUserByUsername(string username);
    public Task<User?> GetFullUser(string id);
    public Task<byte[]> GetUserProfilePicture(string userId);
}