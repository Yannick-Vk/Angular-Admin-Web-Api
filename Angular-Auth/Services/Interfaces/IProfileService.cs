using Angular_Auth.Dto.Users;

namespace Angular_Auth.Services.Interfaces;

public interface IProfileService {
    public Task UpdateEmail(string userId, string newEmail);
    public Task<bool> UpdateUsername(string userId, string newUsername);
    public Task UpdatePassword(string userId, string password, string newPassword);
    public Task UploadProfilePicture(string userId, ProfilePictureUpload pictureUpload);
    public Task UploadProfilePictureFromUrl(string userId, string pictureUrl);
    public Task<byte[]> GetProfilePicture(string userId);
    public Task DeleteProfilePicture(string userId);
    public bool IsUsernameAvailable(string username);
    public Task ResetPassword(string email);
}