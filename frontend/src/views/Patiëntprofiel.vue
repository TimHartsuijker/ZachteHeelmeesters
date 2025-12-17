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
        <p>Voornaam: … <br>
           Achternaam: … </p>

        <p>Patiëntnummer: … <br>
           BSN: … <br>
           Geboortedatum: … <br>
           Geslacht: … </p>

        <p>Huisartspraktijk: …<br>
           Huisartsnaam: … </p>

        <p>Telefoonnummer: … <br>
           Emailadres: … <br>
           Straatnaam: … <br>
           Huisnummer: … <br>
           Postcode: … <br>
           Plaats: … </p>
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
import '../css/patiëntprofiel.css'
import '../css/navbar.css'
import Navbar from '../components/Navbar.vue'
</script>
