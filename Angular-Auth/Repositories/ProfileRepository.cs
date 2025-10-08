using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Repositories;

public class ProfileRepository(AppDbContext context, UserManager<User> userManager) {
    public async Task UpdateEmail(User user, string newEmail) {
        var emailResult = await userManager.SetEmailAsync(user, newEmail);
        if (!emailResult.Succeeded) {
            throw new Exception(
                $"Failed to update email: {string.Join(", ", emailResult.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdatePassword(User user, string password, string newPassword) {
        var passwordResult = await userManager.ChangePasswordAsync(user, password, newPassword);
        if (!passwordResult.Succeeded) {
            throw new Exception(
                $"Failed to update password: {string.Join(", ", passwordResult.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdateProfilePicture(User user, IFormFile image) {
        var extension = Path.GetExtension(image.FileName);
        await FileService.SaveFile(user.Id, image, "users/profile-pictures", extension);
    }

    public async Task<byte[]> GetProfilePicture(User user) {
        return await FileService.GetFileBytes(user.Id, "users/profile-pictures", ".*");
    }
}