<script setup>
import NavBar from './components/nav.vue'
import Gebruikers from './components/Gebruiker.vue'

console.log('App.vue mounted');
</script>

<template>
  <header>
    <div class="wrapper">
      <NavBar/>
    </div>
  </header>

  <main class="main-users align-under-nav">
    <Gebruikers
      name="Jan Jansen"
      email="jan.jansen@email.com"
      role="Patiënt"
      :extraPermissionSB="true"
    />
    <!-- Add more <Gebruikers ... /> here for additional users -->
  </main>
</template>

<script>
export default {
  mounted() {
    console.log('App.vue template rendered');
  }
}
</script>

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
.main-users {
  width: 80vw;
  max-width: 80vw;
  /* Remove margin-top, use padding-top to offset fixed header */
  margin: 2rem auto;
  background: #CAE8BD;
  border-radius: 1.5rem;
  box-shadow: 0 2px 16px 0 rgba(0,0,0,0.07);
  padding: 0 0 2.5rem 0;
  margin-top: 112px; /* 70px min-height + 48px vertical padding + 20px gap */
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 0;
}
/* Remove .align-under-nav margin-top, not needed with padding-top */
.align-under-nav {
  margin-top: 0;
}
</style>
