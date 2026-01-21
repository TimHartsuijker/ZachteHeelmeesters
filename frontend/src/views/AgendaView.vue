<template>
  <main class="main-agenda align-under-nav">
    <DoctorCalendar v-if="doctorId" :doctor-id="doctorId" />
  </main>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import DoctorCalendar from '../components/DoctorCalendar.vue';
import router from '@/router/router';

const doctorId = ref(null);

onMounted(() => {
  console.log('AgendaView.vue mounted');
  const userId = sessionStorage.getItem('userId');
  
  doctorId.value = parseInt(userId);
});
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

.main-agenda {
  width: 80vw;
  max-width: 80vw;
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
