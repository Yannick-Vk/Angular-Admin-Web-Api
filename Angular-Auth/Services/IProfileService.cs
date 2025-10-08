using Angular_Auth.Dto;

namespace Angular_Auth.Services;

public interface IProfileService {
    public Task UpdateEmail(string userId, string newEmail, string password);
    public Task UpdatePassword(string userId, string newPassword, string password);
    public Task UploadProfilePicture(string userId, ProfilePictureUpload pictureUpload);
    public Task<byte[]> GetProfilePicture(string userId);
    public Task DeleteProfilePicture(string userId);
}