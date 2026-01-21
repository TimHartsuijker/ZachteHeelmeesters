<template>
  <div class="admin-dashboard-page">
    <!-- Content area -->
    <div class="admin-content">
      <p v-if="message">{{ message }}</p>
      <p v-else>Kies een feature in de navbar bovenaan.</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const adminId = ref(null);
const router = useRouter();

onMounted(() => {
  console.log('AdminDashboardView.vue mounted');
});
</script>