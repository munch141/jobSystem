<template>
  <div class="flex flex-col">
    <h1>Executing jobs</h1>
    <p v-for="job in jobs" :key="job.id">{{ job.name }}</p>
  </div>
</template>

<script lang="ts">
import hubConnection from '@/hubs/jobsHubConnection'
import Job from '@/models/job'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'ExecutingJobs',
  data() {
    return {
      jobs: [] as Job[],
    }
  },
  methods: {
    onJobStatusUpdate(job: Job, status: string) {
      if (status == 'Executing') this.jobs.push(job)
      else
        this.jobs = this.jobs.filter(
          (executingJob: Job) => executingJob.id != job.id
        )
    },
  },
  mounted() {
    hubConnection.on('UpdateJobStatus', this.onJobStatusUpdate)
  },
  unmounted() {
    hubConnection.off('UpdateJobStatus')
  },
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
