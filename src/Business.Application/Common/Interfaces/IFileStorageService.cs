namespace Business.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string relativeDirectory, string extension, CancellationToken cancellationToken);
    Stream? OpenRead(string storagePath);
    Task DeleteAsync(string storagePath);
}
