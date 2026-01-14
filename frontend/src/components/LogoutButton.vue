<template>
  <button class="logout-btn" @click.prevent="logout">Uitloggen</button>
</template>

<script setup>
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();

const logout = async () => {
  try {
    await axios.post("https://localhost:7240/api/auth/logout", {}, {
      withCredentials: true // ZORGT DAT DE SESSIE-COOKIE WORDT MEEGESTUURD
    });
    
    sessionStorage.clear();
    router.push("/");
  } catch (error) {
    console.error("Logout failed:", error);
  }
};

</script>