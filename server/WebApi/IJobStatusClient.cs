namespace WebApi
{
    public interface IJobStatusClient
    {
         public Task UpdateJobStatus(Guid jobId, string status);
    }
}