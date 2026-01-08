<template>
  <Navbar />

  <main class="dashboard-main">
    <div class="welcome-box">
      <h1 data-test="welcome-message">Welkom op het Dashboard {{ name }}</h1>
    </div>

    <div class="dashboard-overzicht">
      <h2>Overzicht</h2>
      <section class="dashboard-grid">
        <div class="panel panel-left">
          <h2>Afspraken</h2>
          <p>Hier komen de aankomende afspraken</p>
        </div>

        <div class="panel panel-right">
          <h2>Doorverwijzingen</h2>
          <p>Hier komen de Doorverwijzingen.</p>
        </div>
      </section>
    </div>
  </main>
</template>

<script setup lang="ts">
import Navbar from '../components/Navbar.vue'
import api from '../services/api'
import { ref, onMounted } from 'vue'

const name = ref('')

onMounted(async () => {
  try {
    const userId = sessionStorage.getItem('userId')
    if (!userId) throw new Error('Niet ingelogd')

    const res = await api.get(`/patient/${userId}`)
    name.value = `${res.data.voornaam} ${res.data.achternaam}`
  } catch (err) {
    console.error('Error bij ophalen patiënt:', err)
  }
})
</script>

<style scoped>
@import '../assets/dashboard.css';
@import '../assets/navbar.css';
</style>
