<template>
  <main class="profiel-container">

    <section class="profiel-boven">
      <h2>Mijn gegevens</h2>
      <p>Op deze pagina vind je een overzicht van je persoonlijke gegevens zoals deze bij ons bekend zijn.
         Deze informatie wordt gebruikt voor het verlenen van zorg en het onderhouden van contact.<br><br>
         Controleer of je gegevens correct en actueel zijn.<br>
         Kloppen je gegevens niet of wil je iets laten aanpassen?<br>
         Neem dan contact op met de administratie.</p>
    </section>

    <section class="profiel-onder">
      <div class="profiel-card">
        <!-- Loading state -->
        <div v-if="loading" class="loading-state">
          <p>Gegevens worden geladen...</p>
        </div>
        
        <!-- Error state -->
        <div v-else-if="error" class="error-state">
          <p>{{ error }}</p>
        </div>
        
        <!-- Data weergeven -->
        <div v-else-if="patient" class="patient-data">
          <p>
            <strong>Voornaam:</strong> {{ patient.voornaam }} <br>
            <strong>Achternaam:</strong> {{ patient.achternaam }}
          </p>

          <p>
            <strong>Patiëntnummer:</strong> {{ patient.patientID }} <br>
            <strong>BSN:</strong> {{ patient.bsn }} <br>
            <strong>Geboortedatum:</strong> {{ patient.geboortedatum }} <br>
            <strong>Geslacht:</strong> {{ patient.geslacht }}
          </p>

          <p>
            <strong>Huisartspraktijk:</strong> {{ patient.huisartspraktijk }}<br>
            <strong>Huisartsnaam:</strong> {{ patient.huisartsnaam }}
          </p>

          <p>
            <strong>Telefoonnummer:</strong> {{ patient.telefoonnummer }} <br>
            <strong>Emailadres:</strong> {{ patient.email }} <br>
            <strong>Straatnaam:</strong> {{ patient.straatnaam }} <br>
            <strong>Huisnummer:</strong> {{ patient.huisnummer }} <br>
            <strong>Postcode:</strong> {{ patient.postcode }} <br>
          </p>
        </div>
      </div>
    </section>
    
    <section class="profiel-rechts">
      <button class="button-box">
          Klopt huisarts niet? Pas dan nu hier aan.
      </button>
    </section>
  </main>
</template>


<style scoped>
@import '../assets/patiëntprofiel.css';
@import '../assets/navbar.css';
</style>


<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { createPatientSkeleton } from '../utils/patientSchema.js';

// Initialiseer met een leeg skelet
const patient = ref(createPatientSkeleton());
const loading = ref(true);
const error = ref(null);

onMounted(async () => {
  const userId = sessionStorage.getItem('userId');
  try {
    const res = await axios.get('/api/patient/' + userId);
    
    // Vul het skelet met de echte data
    patient.value = createPatientSkeleton(res.data);
    
  } catch (e) {
    error.value = "Fout bij laden";
  } finally {
    loading.value = false;
  }
});
</script>

