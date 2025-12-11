<template>
  <nav class="navbar">
    <ul>
      <li>
        <a href="/Gebruikers.html">Gebruiker</a>
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
}

.navbar ul {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
}

.navbar li {
  margin-right: 1rem;
}

.navbar a {
  color: #222;
  text-decoration: none;
  font-weight: bold;
  font-size: 1.1rem;
}

body {
  padding-top: 3.5rem;
}
</style>
