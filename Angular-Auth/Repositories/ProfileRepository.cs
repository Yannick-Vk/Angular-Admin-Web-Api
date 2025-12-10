using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Repositories;

public class ProfileRepository(UserManager<User> userManager) {
    private static readonly FileService Files = new("users/profile-pictures", ".*");

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

    public static async Task UpdateProfilePicture(User user, IFormFile image) {
        await Files.SaveFile(user.Id, image);
    }

    public static async Task<byte[]> GetProfilePicture(User user) {
        return await Files.GetFileBytes(user.Id);
    }

    public static void DeleteProfilePicture(User user) {
        Files.DeleteFile(user.Id);
    }

    public async Task UpdateUsername(User user, string username) {
        user.UserName = username;
        await userManager.UpdateAsync(user);
    }
}