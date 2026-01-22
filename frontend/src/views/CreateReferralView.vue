<template>
  <div class="referral-view-page">
    <div class="referral-view-card">
    <h1>Create Referral</h1>
    <form>
      <div class="referral-view-form-group" >
        <select v-model="referral.patientId">
          <option value="" disabled>Select patient</option>
          <option v-for="patient in patients" :key="patient.id" :value="patient.id">
            {{ patient.firstName }} {{ patient.lastName }}
          </option>
        </select>
      </div>
      <div class="referral-view-form-group" >
        <select v-model="referral.treatmentCode">
          <option value="" disabled>Select treatment code</option>
          <option v-for="treatment in treatmentCodes" :key="treatment.id" :value="treatment.code">
            {{ treatment.code }} - {{ treatment.description }}
          </option>
        </select>
      </div>
      <div class="referral-view-form-group" >
        <input v-model="referral.note" placeholder="Note" />
      </div>
      <div class="referral-view-form-group">
        <button type="submit" id="login-btn" @click.prevent="submitReferral">Submit</button>
      </div>     
    </form>
  </div>
</div>
</template>

<script setup>

import { onMounted, ref } from 'vue';
import axios from "axios";
import { useRouter } from "vue-router";

const router = useRouter();

const treatmentCodes = ref([]);
const patients = ref([]);

const referral = ref({
  patientId: '',
  treatmentCode: '',
  note: ''
});

const loadTreatmentCodes = async () => {
  try {
    const { data } = await axios.get('/api/treatments');
    treatmentCodes.value = data ?? [];
  } catch (error) {
    console.error('Error fetching treatment codes:', error.response?.data || error.message);
  }
};

const loadPatients = async () => {
  try {
    const doctorId = sessionStorage.getItem('userId');
    const { data } = await axios.get('/api/users');
    // Filter to only show patients connected to the current doctor
    patients.value = data.filter(user => user.doctorId === parseInt(doctorId)) ?? [];
  } catch (error) {
    console.error('Error fetching patients:', error.response?.data || error.message);
  }
};

const submitReferral = async () => {
  try {
    const doctorId = sessionStorage.getItem('userId');
    
    if (!doctorId) {
      console.error('No doctor ID found. User may not be logged in.');
      return;
    }

    // Find the treatment ID from the selected code
    const selectedTreatment = treatmentCodes.value.find(t => t.code === referral.value.treatmentCode);
    if (!selectedTreatment) {
      console.error('Invalid treatment code selected.');
      return;
    }

    const response = await axios.post('/api/referral', {
      code: `${referral.value.treatmentCode}${parseInt(referral.value.patientId)}${parseInt(doctorId)}`,
      treatmentId: selectedTreatment.id,
      patientId: parseInt(referral.value.patientId),
      doctorId: parseInt(doctorId),
      note: referral.value.note,
      createdAt: new Date().toISOString()
    });

    console.log('Referral submitted successfully:', response.data);
  } catch (error) {
    console.error('Error submitting referral:', error.response?.data || error.message);
    // You can add user-facing error handling here (e.g., show a toast notification)
  }
};

onMounted(() => {
  loadTreatmentCodes();
  loadPatients();
});
</script>