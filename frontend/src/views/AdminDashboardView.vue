<template>
  <div class="admin-dashboard-page">
    <AdminNavBar/>

    <!-- Content area -->
    <div class="admin-content">
      <p v-if="message">{{ message }}</p>
      <p v-else>Kies een feature in de navbar bovenaan.</p>
    </div>
  </div>
</template>

<script>
export default {
  name: "AdminDashboardView",
};
</script>

<script setup>
import AdminNavBar from '../components/AdminNavBar.vue';
</script>