using System.Text;

namespace Angular_Auth.Services;

public class FileService(string folder, string extension) {
    public async Task<string> GetFileContent(string filename) => await GetFileContent(filename, folder, extension);

    public static async Task<string> GetFileContent(string filename, string folder, string extension) {
        var filePath = GetFilePath(filename, folder, extension);
        // Initialize an empty string, if the file exist add the contents
        var content = string.Empty;
        if (File.Exists(filePath)) content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        return content;
    }

    public async Task<byte[]> GetFileBytes(string filename) => await GetFileBytes(filename, folder, extension);

    public static async Task<byte[]> GetFileBytes(string filename, string folder, string extension) {
        var filePath = GetFilePath(filename, folder, extension);
        var bytes = File.Exists(filePath) ? await File.ReadAllBytesAsync(filePath) : [];
        return bytes;
    }

    public async Task SaveFile(string filename, string fileContent) => await SaveFile(filename, fileContent, folder, extension);


    public static async Task SaveFile(string filename, string fileContent, string folder, string extension) {
        var filePath = GetFilePath(filename, folder, extension);

        var fileBytes = Encoding.UTF8.GetBytes(fileContent);

        // Save the file.
        await using var stream = new FileStream(filePath, FileMode.Create);
        await stream.WriteAsync(fileBytes);
    }

    public void DeleteFile(string filename) => DeleteFile(filename, folder, extension);

    public static void DeleteFile(string filename, string folder, string extension) {
        var filePath = GetFilePath(filename, folder, extension);
        File.Delete(filePath);
    }

    public string GetFilePath(string filename) => GetFilePath(filename, folder, extension);

    private static string GetFilePath(string filename, string folder, string extension) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"uploads/{folder}");
        var uniqueFileName = filename + extension;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        return filePath;
    }
}