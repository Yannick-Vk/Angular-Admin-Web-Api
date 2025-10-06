using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public class ProfileService(UserManager<User> userManager) : IProfileService {
    public async Task UpdateEmail(string userId, string newEmail, string password) {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) {
            throw new UserNotFoundException($"Cannot find user with id {userId}");
        }

        var correctPassword = await userManager.CheckPasswordAsync(user, password);
        if (!correctPassword) throw new WrongCredentialsException("Username and/or password is incorrect");

        var setEmailResult = await userManager.SetEmailAsync(user, newEmail);
        if (!setEmailResult.Succeeded) {
            throw new Exception(
                $"Failed to update email: {string.Join(", ", setEmailResult.Errors.Select(e => e.Description))}");
        }
    }
}