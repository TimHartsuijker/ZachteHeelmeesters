<template>
  <nav class="navbar" aria-label="Hoofdnavigatie">
    <!-- Andere gebruikers: Specialist & Admin -->
    <ul v-if="userRole !== 'Patiënt'" class="nav-center-buttons">
      <li v-if="userRole === 'Specialist'">
        <RouterLink to="/agenda" aria-current="page">Agenda</RouterLink>
      </li>
      <li v-else-if="userRole === 'Admin'">
        <RouterLink to="/admin/users" aria-current="user-management page">Gebruikersbeheer</RouterLink>
      </li>
    </ul>

    <!-- Patiënt: desktop menu altijd in DOM -->
    <ul class="nav-center-buttons patient-desktop" v-if="userRole === 'Patiënt'">
      <li>
        <RouterLink to="/dashboard">Dashboard</RouterLink>
      </li>
      <li>
        <RouterLink to="/afspraken">Mijn afspraken</RouterLink>
      </li>
      <li>
        <RouterLink to="/medischdossier">Mijn medisch dossier</RouterLink>
      </li>
      <li>
        <RouterLink to="/patientprofiel">Mijn profiel</RouterLink>
      </li>
    </ul>

    <LogoutButton />
  </nav>

  <!-- Patiënt: hamburger menu altijd in DOM -->
  <nav class="navbar-phone" v-if="userRole === 'Patiënt'">
    <button class="hamburger" @click="toggleMenu">
      ☰
    </button>
    <ul class="nav-menu" :class="{ open: menuOpen }">
      <li @click="menuOpen = false">
        <RouterLink to="/dashboard">Dashboard</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/afspraken">Mijn afspraken</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/medischdossier">Mijn medisch dossier</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/patientprofiel">Mijn profiel</RouterLink>
      </li>
    </ul>
  </nav>
</template>

<script setup>
import { ref } from 'vue';
import LogoutButton from './LogoutButton.vue';

const menuOpen = ref(false);
const toggleMenu = () => menuOpen.value = !menuOpen.value;

const userRole = sessionStorage.getItem('userRole');
</script>