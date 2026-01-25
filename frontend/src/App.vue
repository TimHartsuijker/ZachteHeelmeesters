<template>
  <div id="app">
    <main>
      <NavBar v-if="!route.meta.hideNavbar" />
      <RouterView />
    </main>
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router';
import { RouterView } from 'vue-router';
import NavBar from './components/Navbar.vue';

const route = useRoute();
</script>

<style></style>
