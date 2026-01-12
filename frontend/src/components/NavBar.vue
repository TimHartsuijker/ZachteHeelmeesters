<template>
  <nav class="navbar" aria-label="Hoofdnavigatie">
    <ul>
      <li>
        <router-link to="/dossier" aria-current="page">Dossier</router-link>
      </li>
      <li>
        <router-link to="/doctor/upload" aria-current="page">Document Uploaden</router-link>
      </li>
    </ul>
  </nav>
</template>

<script>
export default {
  name: "NavBar",
  mounted() {
    console.log('nav.vue navbar mounted');
  }
}
</script>

<style scoped>
.navbar {
  background-color: #B0DB9C;
  width: 100vw;
  min-width: 100%;
  min-height: 70px;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1000;
  padding: 1.5rem 3rem;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  font-size: 1.5rem;
  box-shadow: 0 2px 16px 0 rgba(0,0,0,0.07);
}

.navbar ul {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  gap: 2rem;
}

.navbar li {
  margin-right: 0;
}

.navbar a {
  color: #222;
  text-decoration: none;
  font-weight: bold;
  font-size: 1.1rem;
  transition: color 0.3s ease;
  outline: none;
}

.navbar a:hover {
  color: #fff;
  background: none;
  box-shadow: none;
}
</style>

