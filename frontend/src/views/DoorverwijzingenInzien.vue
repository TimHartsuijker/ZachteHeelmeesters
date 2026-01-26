<template>
  <div class="referrals-page">
    <div class="referrals-card">
      <h1>Mijn Doorverwijzingen</h1>
      
      <div v-if="loading" class="loading">Laden...</div>
      
      <div v-else-if="error" class="error">{{ error }}</div>
      
      <div v-else-if="referrals.length === 0" class="no-referrals">
        Geen doorverwijzingen gevonden.
      </div>
      
      <table v-else class="referrals-table">
        <thead>
          <tr>
            <th>Code</th>
            <th>Behandeling</th>
            <th>Huisarts</th>
            <th>Opmerking</th>
            <th>Aangemaakt op</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="referral in referrals" :key="referral.id">
            <td>{{ referral.code }}</td>
            <td>{{ referral.treatmentCode }} - {{ referral.treatmentDescription }}</td>
            <td>{{ referral.doctorName }}</td>
            <td>{{ referral.note || '-' }}</td>
            <td>{{ formatDate(referral.createdAt) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup> 
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

const router = useRouter();
const referrals = ref([]);
const loading = ref(true);
const error = ref(null);

const loadReferrals = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    const patientId = sessionStorage.getItem('userId');
    
    if (!patientId) {
      error.value = 'U bent niet ingelogd.';
      router.push('/login');
      return;
    }
    
    const { data } = await axios.get(`/api/referral/patient/${patientId}`);
    // Sort referrals from newest to oldest
    referrals.value = data.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
  } catch (err) {
    console.error('Error loading referrals:', err);
    error.value = 'Fout bij het ophalen van doorverwijzingen.';
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('nl-NL', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

onMounted(loadReferrals);
</script>