<template>
  <nav class="navbar" aria-label="Hoofdnavigatie">
    <!-- Andere gebruikers: Specialist & Admin -->
    <ul v-if="userRole !== 'Patiënt'" class="nav-center-buttons">
      <li v-if="userRole === 'Specialist'">
        <RouterLink to="/agenda" aria-current="calendar page">Agenda</RouterLink>
      </li>
      <li v-if="userRole === 'Specialist' || userRole === 'Huisarts'" class="nav-center-buttons">
          <RouterLink to="/patienten" aria-current="patient overview page">Patiënten</RouterLink>
      </li>
      <li v-else-if="userRole === 'Huisarts'" class="nav-center-buttons">
          <RouterLink to="/doorverwijzing-aanmaken" aria-current="create referral page">Doorverwijzing Aanmaken</RouterLink>
      </li>
      <li v-else-if="userRole === 'Admin'" class="nav-center-buttons">
        <RouterLink to="/admin/users" aria-current="user management page">Gebruikersbeheer</RouterLink>
      </li>
      <li v-else-if="userRole === 'Administratiemedewerker'" class="nav-center-buttons">
        <RouterLink to="/administratie/accounts" aria-current="accounts page">
          Accounts Overzicht
        </RouterLink>
      </li>
    </ul>

    <!-- Patiënt: desktop menu altijd in DOM -->
    <ul class="nav-center-buttons patient-desktop" v-if="userRole === 'Patiënt'">
      <li>
        <RouterLink to="/dashboard">Dashboard</RouterLink>
      </li>
      <!-- <li>
        <RouterLink to="/afspraken">Mijn afspraken</RouterLink>
      </li> -->
      <li>
        <RouterLink to="/dossier">Mijn medisch dossier</RouterLink>
      </li>
      <li>
        <RouterLink to="/doorverwijzingen-inzien">Mijn doorverwijzingen</RouterLink>
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
      <!-- <li @click="menuOpen = false">
        <RouterLink to="/afspraken">Mijn afspraken</RouterLink>
      </li> -->
      <li @click="menuOpen = false">
        <RouterLink to="/dossier">Mijn medisch dossier</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/doorverwijzingen-inzien">Mijn doorverwijzingen</RouterLink>
      </li>
      <li @click="menuOpen = false">
        <RouterLink to="/patientprofiel">Mijn profiel</RouterLink>
      </li>
    </ul>
  </nav>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import LogoutButton from './LogoutButton.vue';

const menuOpen = ref(false);
const userRole = ref(sessionStorage.getItem('userRole'));

const toggleMenu = () => {
  menuOpen.value = !menuOpen.value;
};

// Functie om de rol te updaten als de sessie verandert
const updateSession = () => {
  userRole.value = sessionStorage.getItem('userRole');
};

onMounted(() => {
  // Luister naar een custom event als je de rol dynamisch wilt bijwerken
  window.addEventListener('session-updated', updateSession);
});

onBeforeUnmount(() => {
  // Netjes opruimen om memory leaks te voorkomen
  window.removeEventListener('session-updated', updateSession);
});
</script>