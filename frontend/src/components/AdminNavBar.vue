<template>
    <nav class="admin-navbar">
        <a @click="goToDashboard"><h1>Admin</h1></a>
        
        <!-- Gecentreerde knoppen -->
        <div class="nav-center-buttons">
        <button @click="viewUsers">Gebruikersbeheer</button>
        <button @click="openSettings">Instellingen</button>
        <button @click="viewAuditLogs">Audit logs</button>
        </div>

        <!-- Logout rechts -->
        <button class="logout-btn" @click="logout">Uitloggen</button>
    </nav>
</template>

<script>
export default {
  name: "AdminNavBar",
};
</script>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

// Initialize hooks and state
const router = useRouter();
const message = ref("");

function goToDashboard() {
  router.push("/admin/dashboard");
}

function viewUsers() {
  router.push("/admin/users");
}

function openSettings() {
  router.push("/admin/dashboard");
}

function viewAuditLogs() {
  router.push("/admin/dashboard");
}

function logout() {
  sessionStorage.removeItem("isAdminLoggedIn");
  router.push("/");
}
</script>