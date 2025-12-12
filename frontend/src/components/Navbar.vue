<script setup lang="ts">
import { RouterLink } from 'vue-router'
import '../css/navbar.css'
</script>

<template>
  <header>
    <nav class="navbar">
      <ul class="nav-menu">
        <li><RouterLink to="/">Dashboard</RouterLink></li>
        <li><RouterLink to="/afspraken">Afspraken</RouterLink></li>
        <li><RouterLink to="/afspraken">Medisch Dossier</RouterLink></li>
      </ul>
    </nav>
  </header>
</template>
