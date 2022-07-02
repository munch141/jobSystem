namespace WebApi
{
    public interface IJobsClient
    {
         public Task UpdateJobStatus(Job job, string status);

         public Task EnqueueJob(Job job);

         public Task DequeueJob(Guid jobId);
    }
}