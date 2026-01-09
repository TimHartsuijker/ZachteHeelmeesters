<template>
  <div class="admin-dashboard-page">
    <!-- Navbar -->
    <nav class="admin-navbar">
      <h1 class="logo">Admin Dashboard</h1>
      
      <!-- Gecentreerde knoppen -->
      <div class="nav-center-buttons">
        <button @click="viewUsers">Gebruikersbeheer</button>
        <button @click="openSettings">Instellingen</button>
        <button @click="viewAuditLogs">Audit logs</button>
      </div>

      <!-- Logout rechts -->
      <button class="logout-btn" @click="logout">Uitloggen</button>
    </nav>

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
  data() {
    return {
      message: "",
    };
  },
  methods: {
    viewUsers() {
      this.message = "Gebruikerslijst getoond (voorbeeld).";
    },
    openSettings() {
      this.message = "Instellingen geopend (voorbeeld).";
    },
    viewAuditLogs() {
      this.message = "Audit logs getoond (voorbeeld).";
    },
    logout() {
      sessionStorage.removeItem("isAdminLoggedIn");
      this.$router.push("/");
    },
  },
};
</script>