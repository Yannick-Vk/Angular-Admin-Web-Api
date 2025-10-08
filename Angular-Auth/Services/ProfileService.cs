using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public class ProfileService(UserManager<User> userManager, ProfileRepository repo) : IProfileService {
    public async Task UpdateEmail(string userId, string newEmail, string password) {
        var user = await GetUserOrException(userId);
        await CheckPasswordOrException(user, password);
        await repo.UpdateEmail(user, newEmail);
    }

    public async Task UpdatePassword(string userId, string newPassword, string password) {
        var user = await GetUserOrException(userId);
        await CheckPasswordOrException(user, password);
        await repo.UpdatePassword(user, password, newPassword);
    }

    public async Task UploadProfilePicture(string userId, ProfilePictureUpload pictureUpload) {
        var user = await GetUserOrException(userId);
        await repo.UpdateProfilePicture(user, pictureUpload.Image);
    }

    public async Task<byte[]> GetProfilePicture(string userId) {
        var user = await GetUserOrException(userId);
        return await repo.GetProfilePicture(user);
    }

    public async Task DeleteProfilePicture(string userId) {
        var user = await GetUserOrException(userId);
        repo.DeleteProfilePicture(user);
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