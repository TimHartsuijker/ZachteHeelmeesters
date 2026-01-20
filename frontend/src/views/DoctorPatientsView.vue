<template>
  <main class="main-patients align-under-nav">
    <h1>Mijn patiënten</h1>
    
    <!-- Patient List -->
    <div class="patients-container">
      <div v-if="patients.length === 0" class="no-patients">
        <p>Geen patiënten toegewezen</p>
      </div>
      
      <div v-else class="patients-list">
        <div v-for="patient in patients" :key="patient.id" class="patient-row">
          <div class="patient-info">
            <div class="patient-name">{{ patient.firstName }} {{ patient.lastName }}</div>
            <div class="patient-details">
              <span class="detail-item">{{ formatDate(patient.dateOfBirth) }}</span>
              <span class="detail-separator">•</span>
              <span class="detail-item">{{ patient.email }}</span>
              <span v-if="patient.phoneNumber" class="detail-separator">•</span>
              <span v-if="patient.phoneNumber" class="detail-item">{{ patient.phoneNumber }}</span>
            </div>
          </div>
          <button @click="openMedicalRecord(patient.id)" class="btn-view-record">
            Dossier Openen →
          </button>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const router = useRouter();
const patients = ref([]);
const selectedPatient = ref(null);
const loading = ref(true);
const error = ref('');

const formatDate = (date) => {
  if (!date) return 'N/A';
  return new Date(date).toLocaleDateString('nl-NL', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};

const fetchPatients = async () => {
  try {
    loading.value = true;
    const doctorId = sessionStorage.getItem('userId');
    
    const response = await axios.get(`/api/users/doctor/${doctorId}/patients`, {
      withCredentials: true
    });
    
    patients.value = response.data || [];
    error.value = '';
  } catch (err) {
    console.error('Error fetching patients:', err);
    error.value = 'Fout bij het ophalen van patiënten';
    patients.value = [];
  } finally {
    loading.value = false;
  }
};

const openMedicalRecord = (patientId) => {
  router.push(`/dossier/${patientId}`);
};

onMounted(() => {
  fetchPatients();
});
</script>

<style scoped>
#app {
  background-color: #ECFAE5;
}

.main-patients {
  width: 80vw;
  max-width: 80vw;
  margin: 2rem auto;
  background: #CAE8BD;
  border-radius: 1.5rem;
  box-shadow: 0 2px 16px 0 rgba(0,0,0,0.07);
  padding: 2rem 2rem 2.5rem 2rem;
  margin-top: 160px;
  display: flex;
  flex-direction: column;
  align-items: center;
  font-family: Arial, sans-serif;
}

.align-under-nav {
  margin-top: 0;
}

h1 {
  color: #222;
  margin: 100px 0 2rem 0;
  text-align: center;
  font-size: 2.2rem;
  font-weight: bold;
  width: 100%;
  padding-top: 0.5rem;
}

.patients-container {
  background: #CAE8BD;
  padding: 0;
  border-radius: 0;
  box-shadow: none;
  width: 100%;
}

.no-patients {
  text-align: center;
  color: #666;
  padding: 2rem;
  font-size: 1.1rem;
}

.patients-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  width: 100%;
}

.patient-row {
  background: #ECFAE5;
  padding: 1.25rem 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border-left: 5px solid #B0DB9C;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.patient-row:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transform: translateY(-1px);
}

.patient-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.patient-name {
  color: #222;
  font-size: 1.1rem;
  font-weight: bold;
}

.patient-details {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
  color: #666;
  font-size: 0.9rem;
}

.detail-item {
  color: #555;
}

.detail-separator {
  color: #ccc;
  font-weight: bold;
}

.btn-view-record {
  background-color: #B0DB9C;
  color: #222;
  border: none;
  padding: 0.65rem 1.25rem;
  border-radius: 6px;
  font-weight: bold;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background-color 0.3s ease;
  white-space: nowrap;
  flex-shrink: 0;
}

.btn-view-record:hover {
  background-color: #9ecb8c;
}

/* Medical Record Modal */
.medical-record-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
  padding: 2rem;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 2.5rem;
  max-width: 900px;
  width: 100%;
  max-height: 85vh;
  overflow-y: auto;
  position: relative;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.2);
}

.close-btn {
  position: absolute;
  top: 1.5rem;
  right: 1.5rem;
  background: none;
  border: none;
  font-size: 1.8rem;
  cursor: pointer;
  color: #999;
  transition: color 0.2s ease;
}

.close-btn:hover {
  color: #333;
}

.modal-content h2 {
  margin-top: 0;
  color: #222;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.patient-header {
  background: #ECFAE5;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  border-left: 4px solid #B0DB9C;
}

.patient-header p {
  margin: 0.5rem 0;
  color: #555;
  font-size: 0.95rem;
}

.record-sections {
  display: grid;
  gap: 1.5rem;
}

.record-section {
  border-left: 4px solid #B0DB9C;
  padding: 1.5rem;
  background: #f9f9f9;
  border-radius: 6px;
}

.record-section h3 {
  margin: 0 0 0.75rem 0;
  color: #222;
  font-size: 1.1rem;
  font-weight: bold;
}

.record-section p {
  margin: 0;
  color: #555;
  line-height: 1.6;
  font-size: 0.95rem;
}

.empty-section {
  color: #999;
  font-style: italic;
}

@media (max-width: 1024px) {
  .main-patients {
    width: 90vw;
    max-width: 90vw;
  }
}

@media (max-width: 768px) {
  .main-patients {
    width: 95vw;
    max-width: 95vw;
    padding: 1.5rem 1rem 2rem 1rem;
  }

  .patient-row {
    flex-direction: column;
    align-items: flex-start;
  }

  .btn-view-record {
    width: 100%;
  }

  .modal-content {
    max-height: 90vh;
    padding: 1.5rem;
  }

  h1 {
    font-size: 1.5rem;
  }

  .patient-header {
    padding: 1rem;
  }

  .record-section {
    padding: 1rem;
  }
}
</style>
