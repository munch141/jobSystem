<template>
  <h1>Finished jobs: {{ finishedJobs }}</h1>
</template>

<script lang="ts">
import hubConnection from '@/hubs/jobsHubConnection'
import Job from '@/models/job'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'FinishedJobs',
  data() {
    return {
      finishedJobs: 0,
    }
  },
  methods: {
    onJobStatusUpdate(_: Job, status: string) {
      if (status == 'Finished') this.finishedJobs++
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
