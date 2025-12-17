<template>
  <Navbar />

  <main class="dashboard-main">
    <div class="welcome-box">
      <!-- Loading state -->
      <h1 v-if="loading" data-test="welcome-message">Welkom op het Dashboard</h1>
      
      <!-- Met naam -->
      <h1 v-else-if="patient" data-test="welcome-message">
        Welkom op het Dashboard, {{ patient.voornaam }}!
      </h1>
      
      <!-- Fallback bij error -->
      <h1 v-else data-test="welcome-message">Welkom op het Dashboard</h1>
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
import '../css/dashboard.css'
import '../css/navbar.css'

import Navbar from '../components/Navbar.vue'
import { getPatient, type PatientResponse } from '../services/api'

import { ref, onMounted } from 'vue'

// Patient data
const patient = ref<PatientResponse | null>(null)
const loading = ref(true)

// Hardcoded patient ID voor nu (later uit auth/session halen)
const PATIENT_ID = 2 // Emma de Vries

onMounted(async () => {
  try {
    patient.value = await getPatient(PATIENT_ID)
  } catch (err) {
    console.error('Error bij ophalen patiënt:', err)
  } finally {
    loading.value = false
  }
})
</script>
