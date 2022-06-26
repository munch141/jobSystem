using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly JobQueue _jobQueue;
        private readonly IHubContext<JobStatusHub, IJobStatusClient> _jobStatusHubContext;
        private readonly int _maxRetries;
        private readonly Guid _id;

        public Worker(
            ILogger<Worker> logger,
            JobQueue jobQueue,
            IConfiguration configuration,
            IHubContext<JobStatusHub, IJobStatusClient> jobStatusHubContext)
        {
            _logger = logger;
            _jobQueue = jobQueue;
            _jobStatusHubContext = jobStatusHubContext;
            _maxRetries = configuration.GetValue<int>("JobRetriesLimit");
            _id = Guid.NewGuid();
        }

        public override Task StartAsync(CancellationToken stoppingToken) {
            _logger.LogInformation($"Worker {_id} started.");

            return base.StartAsync(stoppingToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            return ProcessJobQueueAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation($"Worker {_id} stopped.");

            return Task.CompletedTask;
        }

        private async Task ProcessJobQueueAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _jobQueue.DequeueAsync();
                var task = job.GetTask();
                var retriesCount = 0;

                _logger.LogInformation($"Starting execution of job {job.Id} of type {job.Name}.");

                await _jobStatusHubContext.Clients.All.UpdateJobStatus(job.Id, "Executing");

                while (retriesCount < _maxRetries)
                {
                    try
                    {
                        await task;
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        // prevent throwing an exception when cancelling task
                    }
                    catch (Exception ex)
                    {
                        if (retriesCount == _maxRetries)
                            _logger.LogError($"The job reached the max retries limit ({_maxRetries}) and failed.", ex);
                        else
                            _logger.LogError($"The job failed, {_maxRetries - retriesCount - 1} attempts left.", ex);

                        retriesCount++;
                    }
                }

                _logger.LogInformation($"Execution of job {job.Id} finished successfully.");
            }
        }
    }
}