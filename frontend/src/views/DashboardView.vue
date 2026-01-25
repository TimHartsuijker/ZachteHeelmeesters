<template>
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
          
          <div v-if="loadingReferrals" class="loading-small">Laden...</div>

          <ReferralList 
            v-else 
            :referrals="referrals" 
            :limit="3" 
          />
          
          <router-link v-if="referrals.length > 0" to="/doorverwijzingen-inzien" class="view-all-link">
            Bekijk alle doorverwijzingen →
          </router-link>
        </div>
      </section>
    </div>
  </main>
</template>

<script setup>
import axios from 'axios';
import { ref, onMounted } from 'vue'
import '../assets/dashboard.css';
import '../assets/navbar.css';
import ReferralList from '../components/ReferralList.vue';

const name = ref('')
const referrals = ref([])
const loadingReferrals = ref(true)

const loadDashboardData = async () => {
  const userId = sessionStorage.getItem('userId')
  if (!userId) return

  try {
    // 1. Haal patiënt naam op
    const res = await axios.get(`/api/patient/${userId}`)
    name.value = `${res.data.voornaam} ${res.data.achternaam}`

    // 2. Haal doorverwijzingen op (Compacte lijst voor dashboard)
    const refRes = await axios.get(`/api/referral/patient/${userId}`)
    // Sorteer op datum en sla op
    referrals.value = refRes.data.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
  } catch (err) {
    console.error('Error bij ophalen dashboard data:', err)
  } finally {
    loadingReferrals.value = false
  }
}

onMounted(loadDashboardData);
</script>