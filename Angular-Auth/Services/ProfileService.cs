using System.Net;
using Angular_Auth.Dto.Users;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Repositories;
using Angular_Auth.Services.Interfaces;
using Angular_Auth.Utils;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public class ProfileService(
    UserManager<User> userManager,
    IAuthenticationService authenticationService,
    IMailService mailService,
    ProfileRepository repo,
    ILoggerFactory loggerFactory)
    : IProfileService {
    private readonly ILogger<MailBuilder> _mailBuilderLogger = loggerFactory.CreateLogger<MailBuilder>();

    public async Task UpdateEmail(string userId, string newEmail) {
        var user = await GetUserOrException(userId);
        await repo.UpdateEmail(user, newEmail);
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Net.WebUtility.UrlEncode(token);

        var verificationLink = $"https://localhost:7134/api/v1/auth/verify-email?userId={user.Id}&token={encodedToken}";
        
        var mail = new MailBuilder(_mailBuilderLogger)
            .To((user.UserName!, user.Email!))
            .From((IMailService.FromName, IMailService.FromAdress))
            .Subject("Updated Email")
            .AddFiles("change-email", [("link", verificationLink), ("user", user.UserName!)])
            .Build();

        await mailService.SendEmail(mail);
    }

    public async Task UpdatePassword(string userId, string password, string newPassword) {
        var user = await GetUserOrException(userId);
        await CheckPasswordOrException(user, password);
        await repo.UpdatePassword(user, password, newPassword);
    }

    public async Task<bool> UpdateUsername(string userId, string newUsername) {
        var user = await GetUserOrException(userId);
        if (!IsUsernameAvailable(newUsername)) {
            return false;
        }

        await repo.UpdateUsername(user, newUsername);
        return true;
    }

    public async Task UploadProfilePicture(string userId, ProfilePictureUpload pictureUpload) {
        var user = await GetUserOrException(userId);
        string[] validExt = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        var extension = Path.GetExtension(pictureUpload.Image.FileName);
        if (!validExt.Contains(extension)) {
            throw new InvalidFileExtensionException(
                $"Invalid image format. Please upload a JPG, PNG, GIF, or WEBP file. Got {extension}");
        }

        await repo.UpdateProfilePicture(user, pictureUpload.Image);
    }

    public async Task UploadProfilePictureFromUrl(string userId, string pictureUrl) {
        var user = await GetUserOrException(userId);
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(pictureUrl);
        response.EnsureSuccessStatusCode();

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType;

        var extension = ".jpeg"; // Default extension
        if (contentType != null) {
            extension = contentType switch {
                "image/jpeg" => ".jpeg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => extension
            };
        }

        string[] validExt = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        if (!validExt.Contains(extension)) {
            throw new InvalidFileExtensionException($"Invalid image format from URL. Content type: {contentType}");
        }

        var fileName = user.Id + extension;
        using var stream = new MemoryStream(imageBytes);
        IFormFile formFile = new FormFile(stream, 0, imageBytes.Length, fileName, fileName);

        await repo.UpdateProfilePicture(user, formFile);
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



    private async Task<User> GetUserByEmailOrException(string email, string? message = null) {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null) return user;

        message ??= $"Cannot find user with email {email}";
        throw new UserNotFoundException(message);
    }

    private async Task CheckPasswordOrException(User user, string password, string? message = null) {
        var correctPassword = await userManager.CheckPasswordAsync(user, password);
        message ??= "Username and/or password is incorrect";
        if (!correctPassword) throw new WrongCredentialsException(message);
    }

    public bool IsUsernameAvailable(string username) {
        return !userManager.Users.Any(u => u.UserName == username);
    }

    public async Task SendResetPasswordMail(string email) {
        var user = await GetUserByEmailOrException(email);
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        
        // Encode the url to remove '/' etc
        var link = $"https://localhost:5173/reset-password/{user.Id}/{WebUtility.UrlEncode(token)}";
        
        var mail = new MailBuilder(_mailBuilderLogger)
            .To((email, email))
            .Subject("Reset Password")
            .AddFiles("reset-password", [("token", link)])
            .Build();
        
        _mailBuilderLogger.LogInformation("{mail}", mail.ToString());
        // TODO: Remove token log and send mail
        _mailBuilderLogger.LogInformation("Token: {token}", token);
        // await mailService.SendEmail(mail);
    }

    public async Task<IdentityResult> ConfirmResetPassword(string userId, string token, string newPassword) {
        var user = await GetUserOrException(userId);
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result;
    }
}