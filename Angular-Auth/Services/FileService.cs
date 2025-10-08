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

    public async Task SaveFile(string filename, string fileContent) =>
        await SaveFile(filename, fileContent, folder, extension);

    public async Task SaveFile(string filename, IFormFile fileContent) =>
        await SaveFile(filename, fileContent, folder, Path.GetExtension(fileContent.FileName));


    public static async Task SaveFile(string filename, string fileContent, string folder, string extension) {
        var filePath = GetFilePath(filename, folder, extension);

        var fileBytes = Encoding.UTF8.GetBytes(fileContent);

        // Save the file.
        await using var stream = new FileStream(filePath, FileMode.Create);
        await stream.WriteAsync(fileBytes);
    }

    public static async Task SaveFile(string filename, IFormFile fileContent, string folder, string extension) {
        var filePath = GetFilePath(filename, folder, extension);
        // Delete all files before saving a new one
        DeleteFile(filename, folder, ".*");

        // Save the file.
        await using var stream = new FileStream(filePath, FileMode.Create);
        await fileContent.CopyToAsync(stream);
    }

    public void DeleteFile(string filename) => DeleteFile(filename, folder, extension);

    public static void DeleteFile(string filename, string folder, string extension) {
        if (extension == ".*") {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"uploads/{folder}");
            var files = Directory.GetFiles(uploadsFolder, filename + ".*");
            foreach (var file in files) {
                File.Delete(file);
            }
        }
        else {
            var filePath = GetFilePath(filename, folder, extension);
            File.Delete(filePath);
        }
    }

    public string GetFilePath(string filename) => GetFilePath(filename, folder, extension);

    private static string GetFilePath(string filename, string folder, string extension) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"uploads/{folder}");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        if (extension == ".*") {
            var files = Directory.GetFiles(uploadsFolder, filename + ".*");
            if (files.Length > 0) return files[0];
        }

        var uniqueFileName = filename + extension;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        return filePath;
    }
}