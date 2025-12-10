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
      <h1>Welkom op het Dashboard {naam}</h1>
    </div>

    <div class="dashboard-overzicht">
      <h2>Overzicht</h2>
      <p>Hier komt een overzicht van belangrijke informatie.</p>
    </div>
  </main>
</template>

