namespace WebApp.Data
{
    public class Worker : BackgroundService
    {
        public Guid Id { get => _id; }
        public string Status { get => _status; }
        public event Action WorkerStatusChanged;

        private string _status;
        private Guid _id;
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _maxAttempts;

        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _maxAttempts = configuration.GetValue<int>("MaxAttempts");
            _status = "Idle";
            _id = Guid.NewGuid();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker {WorkerId}: executing a job", Id);

            using var scope = _serviceProvider.CreateScope();

            while (!cancellationToken.IsCancellationRequested)
            {
                var queue = _serviceProvider.GetService<JobQueue>();
                var job = await queue.DequeueAsync();
                var attemptsCount = 0;
                var jobCompleted = false;

                _status = "Working";
                WorkerStatusChanged.Invoke();
                
                while (!jobCompleted && attemptsCount < _maxAttempts)
                {
                    try
                    {
                        await job.Invoke(cancellationToken);
                        jobCompleted = true;
                        _logger.LogInformation("Worker {WorkerId}: job done!", Id);
                    }
                    catch (Exception ex)
                    {
                        attemptsCount += 1;

                        if (attemptsCount < _maxAttempts)
                        {
                            _logger.LogError(ex, "Worker {WorkerId}: there was an error while executing a job, retrying. {AttemptsLeft} attempts left.", Id, _maxAttempts - attemptsCount);
                            continue;
                        }
                        
                        _logger.LogError(ex, "Worker {WorkerId}: there was an error while executing a job. {AttemptsCount} attempts failed.", Id, attemptsCount);
                    }
                }

                _status = "Idle";
                WorkerStatusChanged.Invoke();
            }
        }
    }
}
