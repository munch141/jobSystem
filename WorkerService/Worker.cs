using ApplicationCore;

namespace WorkerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly JobQueue _jobQueue;

        public Worker(ILogger<Worker> logger, JobQueue jobQueue) {
            _logger = logger;
            _jobQueue = jobQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                var jobsCount = await _jobQueue.CountAsync();
                _logger.LogInformation("Pending jobs at {time}: {jobCount}", DateTimeOffset.Now.ToString("hh:mm"), jobsCount);
                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
        }
    }
}