<script setup lang="ts">
import '../css/patiëntprofiel.css'
import '../css/navbar.css'

import Navbar from '../components/Navbar.vue'
</script>

<template>
  <Navbar />

  <main class="profiel-container">

    <!-- Linkerkant titel + tekst -->
    <section class="profiel-links">
      <h2>Mijn gegevens</h2>
      <p>Kort tekstje…</p>
    </section>

    <!-- Rechterkant profielgegevens -->
    <section class="profiel-rechts">
      <div class="profiel-card">
        <p>Voornaam: …<br>
        Achternaam: …</p>
        
        <p>Patiëntnummer: …<br>
        Huisartspraktijk: …<br>
        Huisartsnaam: …<br>
        BSN: …<br>
        Geboortedatum: …<br>
        Geslacht: …<br></p>
        <p>
        Straatnaam: …<br>
        Huisnummer: …<br>
        Postcode: …<br>
        Plaats: …</p>
      </div>
    </section>

  </main>
</template>
