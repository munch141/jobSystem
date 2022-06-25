using StackExchange.Redis;
using System.Text.Json;

namespace WebApi
{
    public class JobQueue
    {
        private IConnectionMultiplexer _redis { get; set; }
        private RedisKey _queueKey { get; set; }
        private SemaphoreSlim _semaphore { get; set; }

        public event EventHandler QueueUpdated;

        public JobQueue(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _queueKey = new RedisKey("jobQueue");
            _semaphore = new SemaphoreSlim(0);
        }

        public async Task InitializeQueueAsync()
        {
            var jobCount = await CountAsync();

            if (jobCount > 0) {
                _semaphore.Release((int) jobCount);
            }
        }

        public async Task EnqueueAsync(Job job)
        {
            var db = _redis.GetDatabase();
            var serializedJob = JsonSerializer.Serialize(job);
            
            await db.ListRightPushAsync(_queueKey, new RedisValue(serializedJob));
            QueueUpdated?.Invoke(this, EventArgs.Empty);
            
            _semaphore.Release();
        }

        public async Task<Job?> DequeueAsync()
        {
            await _semaphore.WaitAsync();
            
            var db = _redis.GetDatabase();
            var serializedJob = await db.ListLeftPopAsync(_queueKey);
            var job = serializedJob.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<Job>(serializedJob.ToString());

            QueueUpdated?.Invoke(this, EventArgs.Empty);

            return job;
        }

        public async Task<List<Job>> ListAsync()
        {
            var db = _redis.GetDatabase();
            var serializedList = await db.ListRangeAsync(_queueKey);
            var list = serializedList
                .Select(serializedJob => JsonSerializer.Deserialize<Job>(serializedJob.ToString()))
                .ToList();

            return list;
        }

        public async Task<long> CountAsync()
        {
            var db = _redis.GetDatabase();
            var length = await db.ListLengthAsync(_queueKey);

            return length;
        }
    }
}
