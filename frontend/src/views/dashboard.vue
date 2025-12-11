<script setup lang="ts">
import '../css/dashboard.css'
import '../css/navbar.css'

import Navbar from '../components/Navbar.vue'
import api from '../services/api'

import { ref, onMounted } from 'vue'

interface Patient {
  id: number
  name: string
}

const patients = ref<Patient[]>([])

onMounted(async () => {
  try {
    const res = await api.get('/patient') // <-- automatische koppeling
    patients.value = res.data
  } catch (err) {
    console.error('Error bij ophalen patiënten:', err)
  }
})
</script>

<template>
  <Navbar />

  <main class="dashboard-main">
    <div class="welcome-box">
      <h1 data-test="welcome-message">Welkom op het Dashboard {naam}</h1>
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
