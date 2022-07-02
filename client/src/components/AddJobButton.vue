<template>
  <button @click="addJob">add a job</button>
</template>

<script lang="ts">
import jobsService from '@/services/jobsService'
import { defineComponent } from 'vue'

import { v4 as uuid } from 'uuid'
import Job from '@/models/job'

export default defineComponent({
  name: 'AddJobButton',
  methods: {
    async addJob() {
      const job: Job = new Job(uuid(), 'new job', new Date())

      await jobsService.addJob(job)
    },
  },
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
