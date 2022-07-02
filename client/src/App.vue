<template>
  <AddJobButton class="mr-2"></AddJobButton>
  <FinishedJobs></FinishedJobs>
  <div class="flex">
    <PendingJobs class="mr-2"></PendingJobs>
    <ExecutingJobs class="mr-2"></ExecutingJobs>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import hubConnection from '@/hubs/jobsHubConnection'
import PendingJobs from './components/PendingJobs.vue'
import AddJobButton from './components/AddJobButton.vue'
import ExecutingJobs from './components/ExecutingJobs.vue'
import FinishedJobs from './components/FinishedJobs.vue'

export default defineComponent({
  name: 'App',
  components: { PendingJobs, AddJobButton, ExecutingJobs, FinishedJobs },
  async mounted() {
    try {
      await hubConnection.start()
      console.log('SignalR Connected.')
    } catch (err) {
      console.log(err)
    }
  },
  async unmounted() {
    await hubConnection.stop()
  },
})
</script>

<style></style>
