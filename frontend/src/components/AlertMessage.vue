<script setup>
defineProps({
  type: { type: String, default: 'info' },
  message: { type: String, required: true },
})
</script>

<template>
  <div class="alert" :class="type">{{ message }}</div>
</template>
