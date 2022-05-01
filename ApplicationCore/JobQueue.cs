﻿using StackExchange.Redis;
using System.Text.Json;

namespace ApplicationCore
{
    public class JobQueue
    {
        private IConnectionMultiplexer _redis { get; set; }
        private RedisKey _queueKey { get; set; }

        public JobQueue(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _queueKey = new RedisKey("jobQueue");
        }

        public async Task EnqueueAsync(Job job)
        {
            var db = _redis.GetDatabase();
            var serializedJob = JsonSerializer.Serialize(job);

            await db.ListRightPushAsync(_queueKey, new RedisValue(serializedJob));
        }

        public async Task<Job?> DequeueAsync()
        {
            var db = _redis.GetDatabase();
            var serializedJob = await db.ListLeftPopAsync(_queueKey);

            var job = serializedJob.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<Job>(serializedJob.ToString());

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
