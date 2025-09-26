using Angular_Auth.Dto;
using Angular_Auth.Models;

namespace Angular_Auth.Services;

public interface IUserService {
    public Task<List<UserDto>> GetUsers();
    public Task<UserDto?> GetUserDto(string username);
    public Task<User?> GetUserByUsername(string username);
    public Task<User?> GetFullUser(string id);
}