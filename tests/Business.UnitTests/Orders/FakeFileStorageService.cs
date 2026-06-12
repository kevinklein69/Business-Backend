using Business.Application.Common.Interfaces;

namespace Business.UnitTests.Orders;

public class FakeFileStorageService : IFileStorageService
{
    public List<string> SavedPaths { get; } = [];
    public List<string> DeletedPaths { get; } = [];

    public Task<string> SaveAsync(Stream content, string relativeDirectory, string extension, CancellationToken cancellationToken)
    {
        var path = Path.Combine(relativeDirectory, $"{Guid.NewGuid():N}{extension}");
        SavedPaths.Add(path);
        return Task.FromResult(path);
    }

    public Stream? OpenRead(string storagePath) => null;

    public Task DeleteAsync(string storagePath)
    {
        DeletedPaths.Add(storagePath);
        return Task.CompletedTask;
    }
}
