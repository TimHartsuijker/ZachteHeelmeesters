<template>
  <div class="doctor-patients-view">
    <h1>Mijn patiënten</h1>
    
    <!-- Patient List -->
    <div class="patients-container">
      <div v-if="patients.length === 0" class="no-patients">
        <p>Geen patiënten toegewezen</p>
      </div>
      
      <div v-else class="patients-list">
        <div v-for="patient in patients" :key="patient.id" class="patient-card">
          <div class="patient-info">
            <h3>{{ patient.firstName }} {{ patient.lastName }}</h3>
            <p><strong>Geboortedatum:</strong> {{ formatDate(patient.dateOfBirth) }}</p>
            <p><strong>Email:</strong> {{ patient.email }}</p>
            <p v-if="patient.phoneNumber"><strong>Telefoon:</strong> {{ patient.phoneNumber }}</p>
          </div>
          <button @click="openMedicalRecord(patient.id)" class="btn-view-record">
            Medisch Dossier Openen
          </button>
        </div>
      </div>
    </div>

    <!-- Medical Record Modal/View -->
    <div v-if="selectedPatient" class="medical-record-modal">
      <div class="modal-content">
        <button @click="closeMedicalRecord" class="close-btn">✕</button>
        <h2>Medisch Dossier - {{ selectedPatient.firstName }} {{ selectedPatient.lastName }}</h2>
        
        <div class="patient-header">
          <p><strong>Geboortedatum:</strong> {{ formatDate(selectedPatient.dateOfBirth) }}</p>
          <p><strong>Email:</strong> {{ selectedPatient.email }}</p>
          <p><strong>Telefoonnummer:</strong> {{ selectedPatient.phoneNumber || 'N/A' }}</p>
        </div>

        <div class="record-sections">
          <div class="record-section">
            <h3>Klachten</h3>
            <p v-if="!selectedPatient.complaints" class="empty-section">Geen klachten geregistreerd</p>
            <p v-else>{{ selectedPatient.complaints }}</p>
          </div>

          <div class="record-section">
            <h3>Diagnoses</h3>
            <p v-if="!selectedPatient.diagnoses" class="empty-section">Geen diagnoses geregistreerd</p>
            <p v-else>{{ selectedPatient.diagnoses }}</p>
          </div>

          <div class="record-section">
            <h3>Behandelingen</h3>
            <p v-if="!selectedPatient.treatments" class="empty-section">Geen behandelingen geregistreerd</p>
            <p v-else>{{ selectedPatient.treatments }}</p>
          </div>

          <div class="record-section">
            <h3>Doorverwijzingen</h3>
            <p v-if="!selectedPatient.referrals" class="empty-section">Geen doorverwijzingen geregistreerd</p>
            <p v-else>{{ selectedPatient.referrals }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

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

const openMedicalRecord = async (patientId) => {
  try {
    const response = await axios.get(`/api/users/${patientId}/medical-record`, {
      withCredentials: true
    });
    
    selectedPatient.value = response.data;
  } catch (err) {
    console.error('Error fetching medical record:', err);
    error.value = 'Fout bij het ophalen van het medisch dossier';
  }
};

const closeMedicalRecord = () => {
  selectedPatient.value = null;
};

onMounted(() => {
  fetchPatients();
});
</script>

<style scoped>
.doctor-patients-view {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  padding-top: 100px;
  font-family: Arial, sans-serif;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

.patients-container {
  background: #f9f9f9;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.no-patients {
  text-align: center;
  color: #666;
  padding: 2rem;
  font-size: 1.1rem;
}

.patients-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 1.5rem;
}

.patient-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #B0DB9C;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.patient-info h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.3rem;
}

.patient-info p {
  margin: 0.5rem 0;
  color: #666;
  font-size: 0.95rem;
}

.btn-view-record {
  background-color: #B0DB9C;
  color: #222;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  font-size: 0.95rem;
  transition: background-color 0.3s ease;
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
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
  padding: 2rem;
}

.modal-content {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  max-width: 800px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
}

.close-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

.close-btn:hover {
  color: #333;
}

.modal-content h2 {
  margin-top: 0;
  color: #333;
  margin-bottom: 1.5rem;
}

.patient-header {
  background: #f9f9f9;
  padding: 1rem;
  border-radius: 4px;
  margin-bottom: 2rem;
}

.patient-header p {
  margin: 0.5rem 0;
  color: #666;
}

.record-sections {
  display: grid;
  gap: 1.5rem;
}

.record-section {
  border: 1px solid #e0e0e0;
  padding: 1rem;
  border-radius: 4px;
}

.record-section h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 1.1rem;
}

.record-section p {
  margin: 0;
  color: #666;
  line-height: 1.6;
}

.empty-section {
  color: #999;
  font-style: italic;
}

@media (max-width: 768px) {
  .patients-list {
    grid-template-columns: 1fr;
  }

  .modal-content {
    max-height: 80vh;
    padding: 1.5rem;
  }

  .doctor-patients-view {
    padding: 1rem;
    padding-top: 100px;
  }
}
</style>
