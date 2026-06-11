using Business.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Business.Infrastructure.Services;

public class FileStorageOptions
{
    public string RootPath { get; set; } = "App_Data/uploads";
    public int MaxFileSizeMb { get; set; } = 10;
}

public class LocalFileStorageService(IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly string _rootFullPath = Path.GetFullPath(options.Value.RootPath);

    public async Task<string> SaveAsync(Stream content, string relativeDirectory, string extension, CancellationToken cancellationToken)
    {
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var storagePath = Path.Combine(relativeDirectory, fileName);
        var fullPath = ResolveSafe(storagePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var target = File.Create(fullPath);
        await content.CopyToAsync(target, cancellationToken);

        return storagePath;
    }

    public Stream? OpenRead(string storagePath)
    {
        var fullPath = ResolveSafe(storagePath);
        return File.Exists(fullPath) ? File.OpenRead(fullPath) : null;
    }

    public Task DeleteAsync(string storagePath)
    {
        var fullPath = ResolveSafe(storagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string ResolveSafe(string storagePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootFullPath, storagePath));
        if (!fullPath.StartsWith(_rootFullPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Ungültiger Speicherpfad.");
        }

        return fullPath;
    }
}
