<script setup lang="ts">
import { RouterLink } from 'vue-router'
import '../css/navbar.css'
</script>

<template>
  <header>
    <nav class="navbar">
      <ul class="nav-menu nav-left">
        <li><RouterLink to="/">Dashboard</RouterLink></li>
        <li><RouterLink to="/afspraken">Afspraken</RouterLink></li>
        <li><RouterLink to="/medischdossier">Medisch Dossier</RouterLink></li>
      </ul>
      <ul class="nav-menu nav-right">
        <li><RouterLink to="/Patiëntprofiel">Mijn Profiel</RouterLink></li>
      </ul>
    </nav>
  </header>
</template>
