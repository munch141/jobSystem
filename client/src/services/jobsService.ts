import { ApiClients } from './api-clients/jobsApiClient'
import Job from '@/models/job'

const baseUrl = 'https://localhost:5001'
const jobsClient: ApiClients.IJobsClient = new ApiClients.JobsClient(baseUrl)

export default {
  async getPendingJobs(): Promise<Job[]> {
    const pendingJobs = await jobsClient.pending()

    return pendingJobs.map((apiJob: ApiClients.Job) => apiJob as Job)
  },

  async addJob(job: Job): Promise<void> {
    return await jobsClient.jobs(job as ApiClients.Job)
  },
}
