<script setup>
import NavBar from './components/nav.vue'
import DoctorCalendar from './components/DoctorCalendar.vue'
console.log('App.vue mounted');
</script>

<template>
  <header>
    <div class="wrapper">
      <NavBar/>
    </div>
  </header>

  <main class="main-agenda align-under-nav">
    <DoctorCalendar />
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
  width: 100vw;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1000;
}
#app {
  background-color: #ECFAE5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>

<style>
.main-agenda {
  width: 80vw;
  max-width: 80vw;
  /* Remove margin-top, use padding-top to offset fixed header */
  margin: 2rem auto;
  background: #CAE8BD;
  border-radius: 1.5rem;
  box-shadow: 0 2px 16px 0 rgba(0,0,0,0.07);
  padding: 0 0 2.5rem 0;
  margin-top: 140px;
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 0;
}
/* Remove .align-under-nav margin-top, not needed with padding-top */
.align-under-nav {
  margin-top: 0;
}
.status-message {
  padding: 2rem;
  text-align: center;
  font-size: 1.1rem;
  color: #666;
}
.status-message.error {
  color: #d32f2f;
}
</style>