using Angular_Auth.Dto;
using Angular_Auth.Models;

namespace Angular_Auth.Services;

public interface IUserService {
    public Task<List<UserDto>> GetUsers();
}