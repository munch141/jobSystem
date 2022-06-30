<template>
  <PendingJobs></PendingJobs>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import PendingJobs from './components/PendingJobs.vue'

export default defineComponent({
  name: 'App',
  components: { PendingJobs },
})
</script>

<style></style>
