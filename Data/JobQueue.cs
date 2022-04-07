using System.Collections.Concurrent;

namespace jobSystem.Data
{
    public class JobQueue
    {
        public int CurrentJobsCount => _queue.Count;

        private ConcurrentQueue<Func<CancellationToken, Task>> _queue = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public event Action QueueChanged;

        public void Enqueue(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException("tried to add a null work item.");

            _queue.Enqueue(workItem);
            _semaphore.Release();

            QueueChanged?.Invoke();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync()
        {
            await _semaphore.WaitAsync();
            _queue.TryDequeue(out var workItem);

            QueueChanged?.Invoke();

            return workItem;
        }
    }
}
