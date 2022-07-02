using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly JobQueue _jobQueue;
        private readonly IHubContext<JobsHub, IJobsClient> _jobsHubContext;
        private readonly int _maxRetries;
        private readonly Guid _id;

        public Worker(
            ILogger<Worker> logger,
            JobQueue jobQueue,
            IConfiguration configuration,
            IHubContext<JobsHub, IJobsClient> jobStatusHubContext)
        {
            _logger = logger;
            _jobQueue = jobQueue;
            _jobsHubContext = jobStatusHubContext;
            _maxRetries = configuration.GetValue<int>("JobRetriesLimit");
            _id = Guid.NewGuid();
        }

        public override Task StartAsync(CancellationToken stoppingToken) {
            _logger.LogInformation(this.LogMessage($"Worker {_id} started."));

            return base.StartAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _jobQueue.DequeueAsync();

                await _jobsHubContext.Clients.All.DequeueJob(job.Id);

                var task = job.GetTask();
                var retriesCount = 0;

                _logger.LogInformation(this.LogMessage($"Starting execution of job {job.Id} of type {job.Name}."));

                await _jobsHubContext.Clients.All.UpdateJobStatus(job, "Executing");

                while (retriesCount < _maxRetries)
                {
                    try
                    {
                        await task;

                        _logger.LogInformation(this.LogMessage($"Execution of job {job.Id} finished successfully."));

                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        // prevent throwing an exception when cancelling task
                    }
                    catch (Exception ex)
                    {
                        if (retriesCount == _maxRetries)
                            _logger.LogError(this.LogMessage($"The job reached the max retries limit ({_maxRetries}) and failed."), ex);
                        else
                            _logger.LogError(this.LogMessage($"The job failed, {_maxRetries - retriesCount - 1} attempts left."), ex);

                        retriesCount++;
                    }
                }

                await _jobsHubContext.Clients.All.UpdateJobStatus(job, "Finished");
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation(this.LogMessage($"Worker {_id} stopped."));

            return Task.CompletedTask;
        }

        private string LogMessage(string message) => $"Worker {_id}: {message}.";
    }
}