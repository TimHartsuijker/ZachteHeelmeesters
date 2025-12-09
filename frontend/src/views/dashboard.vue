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

  <main>
    <div class="dashboard-content">
      <h1>Welkom op het Dashboard</h1>

      <h2>Patiënten</h2>

      <p v-if="patients.length === 0">Patiënten laden…</p>

      <ul v-else class="patient-list">
        <li v-for="p in patients" :key="p.id" class="patient-item">
          {{ p.name }}
        </li>
      </ul>
    </div>
  </main>
</template>

<style scoped>
.patient-list {
  padding: 0;
  list-style: none;
}

.patient-item {
  background: #eef4ff;
  padding: 12px 15px;
  margin-bottom: 10px;
  border-radius: 8px;
}
</style>
