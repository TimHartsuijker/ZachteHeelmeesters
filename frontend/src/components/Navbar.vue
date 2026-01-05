<template>
  <!-- Voor laptop -->
  <header>
    <nav class="navbar">
      <ul class="nav-menu">
        <li><RouterLink to="/">Dashboard</RouterLink></li>
        <li><RouterLink to="/afspraken">Afspraken</RouterLink></li>
        <li><RouterLink to="/afspraken">Medisch Dossier</RouterLink></li>
      </ul>
    </nav>
  </header>

  <!-- Voor tablet en telefoon -->
  <header>
    <nav class="navbar">
      <button class="hamburger" @click="toggleMenu">
        ☰
      </button>

      <ul class="nav-menu" :class="{ open: menuOpen }">
        <li @click="menuOpen = false">
          <RouterLink to="/">Dashboard</RouterLink>
        </li>
        <li @click="menuOpen = false">
          <RouterLink to="/afspraken">Afspraken</RouterLink>
        </li>
        <li @click="menuOpen = false">
          <RouterLink to="/afspraken">Medisch Dossier</RouterLink>
        </li>
      </ul>
    </nav>
  </header>
</template>


<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import '../css/navbar.css'

const menuOpen = ref(false)

const toggleMenu = () => {
  menuOpen.value = !menuOpen.value
}
</script>