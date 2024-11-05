

namespace ArticleProject.Application.Interfaces.Services
{
    public interface IDistributedLockService
    {
        Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan expiry, CancellationToken cancellationToken);
        Task ReleaseLockAsync(string lockKey, string lockValue);
    }
}
