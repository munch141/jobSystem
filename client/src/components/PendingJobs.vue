<template>
  <h1>Pending jobs</h1>
</template>

<script lang="ts">
import jobStatusHub from '@/hubs/jobStatusHub'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'PendingJobs',
  async mounted() {
    try {
      await jobStatusHub.start()
      console.log('SignalR Connected.')
    } catch (err) {
      console.log(err)
    }
  },
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
