using ArticleProject.Application.Interfaces.Services;
using StackExchange.Redis;

namespace ArticleProject.Infrastructure.Persistence
{
    public class RedisDistributedLockService : IDistributedLockService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisDistributedLockService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        // Try to acquire the lock
        public async Task<bool> AcquireLockAsync(string lockKey, string lockValue, TimeSpan expiry, CancellationToken cancellationToken)
        {
            var result = await _db.StringSetAsync(lockKey, lockValue, expiry, When.NotExists);
            return result;
        }

        // Release the lock
        public async Task ReleaseLockAsync(string lockKey, string lockValue)
        {
            var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

            await _db.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { lockValue });
        }
    }
}
