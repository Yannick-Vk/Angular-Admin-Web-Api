using Angular_Auth.Dto;

namespace Angular_Auth.Services;

public interface IUserService {
    public Task<List<UserDto>> GetUsers();
    public Task<UserDto?> GetUser(string username);
}