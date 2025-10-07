using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public class ProfileService(UserManager<User> userManager) : IProfileService {
    public async Task UpdateEmail(string userId, string newEmail, string password) {
        var user = await GetUserOrException(userId);
        await CheckPasswordOrException(user, password);

        var setEmailResult = await userManager.SetEmailAsync(user, newEmail);
        if (!setEmailResult.Succeeded) {
            throw new Exception(
                $"Failed to update email: {string.Join(", ", setEmailResult.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdatePassword(string userId, string newPassword, string password) {
        var user = await GetUserOrException(userId);
        await CheckPasswordOrException(user, password);

        var setEmailResult = await userManager.ChangePasswordAsync(user, password, newPassword);
        if (!setEmailResult.Succeeded) {
            throw new Exception(
                $"Failed to update password: {string.Join(", ", setEmailResult.Errors.Select(e => e.Description))}");
        }
    }

    private async Task<User> GetUserOrException(string userId, string? message = null) {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null) return user;

        message ??= $"Cannot find user with id {userId}";
        throw new UserNotFoundException(message);
    }

    private async Task CheckPasswordOrException(User user, string password, string? message = null) {
        var correctPassword = await userManager.CheckPasswordAsync(user, password);
        message ??= "Username and/or password is incorrect";
        if (!correctPassword) throw new WrongCredentialsException(message);
    }
}