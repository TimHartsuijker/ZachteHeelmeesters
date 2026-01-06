<template>
  <Navbar />

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
        <template v-else-if="patient">
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
            <strong>Plaats:</strong> {{ patient.plaats }}
          </p>
        </template>
      </div>
    </section>
    
    <section class="profiel-rechts">
      <button class="button-box">
          Klopt huisarts niet? Pas dan nu hier aan.
      </button>
    </section>
  </main>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import '../assets/patiëntprofiel.css';
import '../assets/navbar.css';
import Navbar from '../components/Navbar.vue';
import { getPatient, type PatientResponse } from '../services/api';

// Reactive state
const patient = ref<PatientResponse | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);

// Hardcoded patient ID voor nu (later uit auth/session halen)
const PATIENT_ID = 2; // Emma de Vries uit je seed

onMounted(async () => {
  try {
    const data = await getPatient(PATIENT_ID);
    console.log('[Patient API response]', data); // ← Log de volledige payload
    patient.value = data;
  } catch (e) {
    console.error('Fout bij ophalen patient:', e);
    error.value = 'Kon gegevens niet ophalen. Probeer het later opnieuw.';
  } finally {
    loading.value = false;
  }
});
</script>


