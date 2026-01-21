<template>
  <nav class="navbar" aria-label="Hoofdnavigatie">
    <ul>
      <li v-if="userRole === 'Specialist'" class="nav-center-buttons">
        <RouterLink to="/agenda" aria-current="page">Agenda</RouterLink>
      </li>
      <li v-else-if="userRole === 'Patiënt'" class="nav-center-buttons">
        <RouterLink to="/dashboard" aria-current="dashboard page">Dashboard</RouterLink>
        <RouterLink to="/afspraken" aria-current="appointments page">Mijn afspraken</RouterLink>
        <RouterLink to="/medischdossier" aria-current="medical dossier page">Mijn medisch dossier</RouterLink>
        <RouterLink to="/patientprofiel" aria-current="patient profile page">Mijn profiel</RouterLink>
      </li>
      <li v-else-if="userRole === 'Admin'" class="nav-center-buttons">
        <RouterLink to="/admin/users" aria-current="user-management page">Gebruikersbeheer</RouterLink>
      </li>
    </ul>
    <LogoutButton />
  </nav>

  <nav class="navbar-phone">
    <button class="hamburger" @click="toggleMenu">
      ☰
    </button>

    <ul class="nav-menu" :class="{ open: menuOpen }">
      <li @click="menuOpen = false">
        <RouterLink to="/dashboard">Dashboard</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/afspraken">Afspraken</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/afspraken">Medisch Dossier</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/Patiëntprofiel">Mijn Profiel</RouterLink>
      </li>
    </ul>
  </nav>
</template>

<script setup>
import LogoutButton from './LogoutButton.vue';
import { ref, onMounted } from 'vue';
import '../assets/navbar.css';

const menuOpen = ref(false);

const toggleMenu = () => {
  menuOpen.value = !menuOpen.value;
};

onMounted(() => {
  console.log('nav.vue navbar mounted');
});
const userRole = sessionStorage.getItem('userRole');
</script>

<style scoped>
.navbar {
  background-color: #B0DB9C;
  width: 100vw;
  min-width: 100%;
  min-height: 70px;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1000;
  padding: 1.5rem 3rem;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  font-size: 1.5rem;
  font-family: Arial, sans-serif;
  box-shadow: 0 2px 16px 0 rgba(0,0,0,0.07);
}

.navbar ul {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  gap: 2rem;
}

.navbar li {
  margin-right: 0;
}

.navbar a {
  color: #222;
  text-decoration: none;
  font-weight: bold;
  font-size: 1.1rem;
  font-family: Arial, sans-serif;
  transition: color 0.3s ease;
  outline: none;
}

.navbar a:hover {
  color: #fff;
  background: none;
  box-shadow: none;
}

.navbar :deep(a) {
  color: #222;
  text-decoration: none;
  font-weight: bold;
  font-size: 1.1rem;
  transition: color 0.3s ease;
  outline: none;
}

body {
  padding-top: 3.5rem;
}
</style>
