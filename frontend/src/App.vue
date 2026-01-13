<template>
  <div id="app">
    <main>
      <NavBar v-if="!route.meta.hideNavbar" />
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router';
import NavBar from './components/NavBar.vue';

const route = useRoute();
</script>

<style>
@import './assets/main.css';
</style>
