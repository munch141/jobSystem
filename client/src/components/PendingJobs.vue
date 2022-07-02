<template>
  <div class="flex flex-col">
    <h1>Pending jobs</h1>
    <p v-for="job in pendingJobs" :key="job.id">{{ job.name }}</p>
  </div>
</template>

<script lang="ts">
import hubConnection from '@/hubs/jobsHubConnection'
import Job from '@/models/job'
import jobsService from '@/services/jobsService'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'PendingJobs',
  data() {
    return {
      pendingJobs: [] as Job[],
    }
  },
  async mounted() {
    hubConnection.on('EnqueueJob', (job: Job) => this.pendingJobs.push(job))
    hubConnection.on(
      'DequeueJob',
      (jobId: string) =>
        (this.pendingJobs = this.pendingJobs.filter(
          (job: Job) => job.id != jobId
        ))
    )

    this.pendingJobs = await jobsService.getPendingJobs()
  },
  unmounted() {
    hubConnection.off('EnqueueJob')
    hubConnection.off('DequeueJob')
  },
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
