<script setup lang="ts">
import { reactive, ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import NavBar from '@/components/NavBar.vue';
import axios from 'axios';

axios.defaults.withCredentials = true;

const route = useRoute();
const router = useRouter();

interface Patient {
  name: string;
  age: number;
  doctor: string;
}

interface Treatment {
  id: number;
  code: string;
  name: string;
  specialism: string;
}

interface MedicalFile {
  id: number;
  fileName: string;
  contentType: string;
  fileSize: number;
  uploadedAt: string;
  category: string | null;
  description: string | null;
}

interface MedicalEntry {
  id: number;
  title: string | null;
  notes: string | null;
  category: string | null;
  createdAt: string;
  appointmentDate: string | null;
  createdBy?: {
    id: number;
    firstName: string;
    lastName: string;
  };
  treatment?: Treatment | null;
  files: MedicalFile[];
}

// API Configuration
const API_BASE_URL = '/api'; // Use relative URL with Vite proxy

// Patient context
const patient = reactive<Patient>({ name: '', age: 0, doctor: '' });
const patientId = ref<string | null>(null);
const userRole = ref<string | null>(sessionStorage.getItem('userRole'));
const isViewingOtherPatient = ref(false);

// Medical entries
const allEntries = ref<MedicalEntry[]>([]);
const treatments = ref<Treatment[]>([]);
const isLoading = ref(false);
const error = ref<string | null>(null);

// Filter state
const tempFilterTreatmentId = ref<string>('');
const tempFilterFrom = ref('');
const tempFilterTo = ref('');

const activeFilterTreatmentId = ref<string>('');
const activeFilterFrom = ref('');
const activeFilterTo = ref('');

// Expanded state tracking (separate from computed entries to maintain reactivity)
const expandedEntries = ref<Set<string>>(new Set());

// Group files by category/appointment
interface GroupedEntry {
  title: string;
  date: string;
  dateRaw?: string;
  files: MedicalFile[];
  key: string;
  notes?: string;
  author?: string;
  treatment?: Treatment | null;
}

type GroupedEntries = Record<string, GroupedEntry>;

const groupedEntries = computed(() => {
  const result: GroupedEntries = {};
  const filteredEntries = allEntries.value.filter((entry) => {
    const entryDate = new Date(entry.createdAt).toISOString().split('T')[0];
    const fromDate = activeFilterFrom.value ? new Date(activeFilterFrom.value) : null;
    const toDate = activeFilterTo.value ? new Date(activeFilterTo.value) : null;
    const matchesTreatment = !activeFilterTreatmentId.value || `${entry.treatment?.id ?? ''}` === activeFilterTreatmentId.value;
    const matchesFromDate = !fromDate || new Date(entryDate) >= fromDate;
    const matchesToDate = !toDate || new Date(entryDate) <= toDate;
    return matchesTreatment && matchesFromDate && matchesToDate;
  });

  for (const entry of filteredEntries) {
    const date = new Date(entry.createdAt).toISOString().split('T')[0];
    const key = `${entry.treatment?.name || 'Overige'}-${date}-${entry.id}`;

    if (!result[key]) {
      result[key] = {
        title: entry.title || entry.category || 'Notitie',
        date: formatDate(entry.appointmentDate || entry.createdAt),
        dateRaw: entry.createdAt,
        files: [],
        key: key,
        notes: entry.notes || undefined,
        author: entry.createdBy ? `${entry.createdBy.firstName} ${entry.createdBy.lastName}` : 'Onbekend',
        treatment: entry.treatment || null,
      };
    }

    const filesWithMeta = entry.files.map((file) => ({
      ...file,
      dateRaw: entry.createdAt,
      appointmentDate: entry.appointmentDate ? new Date(entry.appointmentDate).toISOString().split('T')[0] : undefined,
    } as any));

    result[key].files.push(...filesWithMeta);
  }

  return Object.values(result).sort((a, b) => new Date(b.dateRaw || '').getTime() - new Date(a.dateRaw || '').getTime());
});

// Check if user is a doctor
const isDoctor = computed(() => {
  return userRole.value === 'Huisarts' || userRole.value === 'Specialist';
});

// Go back to patients list (only for doctors)
const goBack = () => {
  router.push('/patienten');
};



async function fetchPatientDetails() {
  try {
    if (!patientId.value) {
      throw new Error('Geen patientId gevonden in sessie');
    }

    const { data: userData } = await axios.get(`/api/users/${patientId.value}/medical-record`);

    // Update patient object met data
    const dob = new Date(userData.dateOfBirth);
    const age = userData.dateOfBirth ? Math.floor((new Date().getTime() - dob.getTime()) / (365.25 * 24 * 60 * 60 * 1000)) : 0;
    
    patient.name = `${userData.firstName} ${userData.lastName}`;
    patient.age = age;
    patient.doctor = 'Huisarts';
  } catch (err) {
    console.error('Error fetching patient details:', err);
    patient.name = 'Patiënt';
    patient.age = 0;
    patient.doctor = 'Onbekend';
  }
}

async function fetchDossierEntries() {
  isLoading.value = true;
  error.value = null;

  try {
    if (!patientId.value) {
      throw new Error('Geen patientId gevonden in sessie');
    }

    const response = await axios.get(`/api/MedicalDossier/patient/${patientId.value}`);

    allEntries.value = response.data;
  } catch (err) {
    error.value = err.response?.data?.message || err.message || 'An error occurred';
    console.error('Error fetching dossier:', err);
  } finally {
    isLoading.value = false;
  }
}

async function fetchTreatments() {
  try {
    if (!patientId.value) {
      throw new Error('Geen patientId gevonden in sessie');
    }

    const response = await axios.get(`/api/MedicalDossier/patient/${patientId.value}/treatments`);

    treatments.value = response.data;
  } catch (err) {
    console.error('Error fetching treatments:', err);
  }
}

async function downloadFile(fileId, fileName) {
  try {
    const response = await axios.get(`/api/MedicalDossier/file/${fileId}`, {
      responseType: 'blob'
    });

    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    
    // Cleanup
    link.remove();
    window.URL.revokeObjectURL(url);
  } catch (err) {
    console.error('Error downloading file:', err);
    alert('Fout bij downloaden van bestand');
  }
}

// Apply filters
function applyFilter() {
  activeFilterTreatmentId.value = tempFilterTreatmentId.value;
  activeFilterFrom.value = tempFilterFrom.value;
  activeFilterTo.value = tempFilterTo.value;
}

// Toggle entry expansion
function toggleEntry(entry: GroupedEntry) {
  if (expandedEntries.value.has(entry.key)) {
    expandedEntries.value.delete(entry.key);
  } else {
    expandedEntries.value.add(entry.key);
  }
}

// Check if entry is expanded
function isEntryExpanded(entry: GroupedEntry): boolean {
  return expandedEntries.value.has(entry.key);
}

// Format date
function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('nl-NL', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
}

// Format date for display
function formatDateLong(dateString: string): string {
  return new Date(dateString).toLocaleDateString('nl-NL', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

// Get file icon color based on type
function getFileColor(contentType: string): string {
  if (contentType.includes('pdf')) return '#CAE8BD';
  if (contentType.includes('word')) return '#B0DB9C';
  if (contentType.includes('image')) return '#DDF6D2';
  return '#ECFAE5';
}

// Load data on mount
onMounted(() => {
  // Check if viewing another patient's dossier (doctor view)
  if (route.params.patientId) {
    patientId.value = String(route.params.patientId);
    isViewingOtherPatient.value = true;
  } else {
    // View own dossier (patient view)
    patientId.value = sessionStorage.getItem('userId');
    isViewingOtherPatient.value = false;
  }
  
  fetchPatientDetails();
  fetchTreatments();
  fetchDossierEntries();
});
</script>

<template>
  <div class="min-h-screen bg-[#ECFAE5] flex flex-col" style="padding-top: 70px;">
    <!-- Navigation -->
    <NavBar />

    <!-- Main Container -->
    <div class="flex-1 flex flex-col items-center">
      <!-- Content wrapper with 70% width -->
      <div class="content-wrapper">
        <!-- Back Button (only for doctors) -->
        <div v-if="isDoctor && isViewingOtherPatient" class="mb-4">
          <button @click="goBack" class="back-button">
            ← Terug naar patiënten
          </button>
        </div>

        <!-- Header and Filter Container -->
        <div class="info-container">
          <!-- Header Card -->
          <div class="user-row">
            <div>
              <h2 class="text-lg font-semibold text-gray-800">{{ patient.name }}</h2>
              <p class="text-sm text-gray-600">{{ patient.age }} Jaar</p>
              <p class="text-sm text-gray-600">Huisarts {{ patient.doctor }}</p>
            </div>
          </div>

          <!-- Filter Section -->
          <div class="user-row" style="margin-top: 1.5rem;">
            <div class="flex flex-col w-full gap-3">
              <!-- Type Filter -->
              <div class="w-full">
                <label class="block text-sm text-gray-700 mb-1">Behandeling:</label>
                <select 
                  v-model="tempFilterTreatmentId" 
                  class="filter-select"
                >
                  <option value="">Alle</option>
                  <option 
                    v-for="treatment in treatments" 
                    :key="treatment.id" 
                    :value="treatment.id.toString()"
                  >
                    {{ treatment.name }}
                  </option>
                </select>
              </div>

              <!-- Date Range Filter -->
              <div class="w-full">
                <label class="block text-sm text-gray-700 mb-1">Tussen:</label>
                <div class="flex flex-col sm:flex-row items-start sm:items-center gap-2 w-full">
                  <input 
                    type="date" 
                    v-model="tempFilterFrom" 
                    class="filter-input"
                  />
                  <span class="hidden sm:inline text-gray-600">tot</span>
                  <input 
                    type="date" 
                    v-model="tempFilterTo" 
                    class="filter-input"
                  />
                </div>
              </div>

              <!-- Filter Button with Icon -->
              <button 
                @click="applyFilter"
                class="action-btn"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
                </svg>
                Toepassen
              </button>
            </div>
          </div>
        </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="flex-1 flex items-center justify-center" style="min-height: 300px;">
        <div class="text-center">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-gray-300 border-t-[#CAE8BD]"></div>
          <p class="mt-4 text-gray-600">Dossier laden...</p>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="flex-1 flex items-center justify-center" style="min-height: 300px;">
        <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded max-w-md">
          <p class="font-bold">Fout bij laden van dossier</p>
          <p>{{ error }}</p>
        </div>
      </div>

      <!-- Entries List -->
      <div v-else class="flex-1 pb-4">
        <!-- Empty state -->
        <div v-if="groupedEntries.length === 0" class="user-row" style="margin-top: 1.5rem;">
          <p class="text-gray-600 text-center w-full">Geen documenten gevonden</p>
        </div>

        <!-- Grouped Entries -->
        <div v-else class="space-y-3">
          <div
            v-for="(entry, index) in groupedEntries"
            :key="index"
            class="entry-card"
          >
            <!-- Entry Header (Collapsible) -->
            <button
              @click="toggleEntry(entry)"
              class="entry-header"
            >
              <div class="flex flex-col sm:flex-row sm:items-center gap-2 sm:gap-3 flex-1">
                <span class="font-semibold text-gray-800 break-words">{{ entry.title }} {{ entry.date }}</span>
                <span v-if="entry.treatment" class="text-sm text-gray-600 whitespace-nowrap">• {{ entry.treatment.name }}</span>
              </div>
              <svg 
                :class="['w-5 h-5 text-gray-600 transition-transform flex-shrink-0', isEntryExpanded(entry) ? 'rotate-90' : '']"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </button>

            <!-- Expanded Content -->
            <div v-if="isEntryExpanded(entry)" class="border-t border-gray-200">
              <!-- Date Line -->
              <div class="px-4 py-2 text-xs text-gray-500 border-b border-dashed border-gray-300">
                {{ formatDateLong(entry.dateRaw || (entry.files[0] && entry.files[0].uploadedAt)) }}
              </div>

              <!-- Files -->
              <div class="p-4 space-y-3">
                <div
                  v-for="file in entry.files"
                  :key="file.id"
                  @click="downloadFile(file.id, file.fileName)"
                  class="file-card"
                  :style="{ backgroundColor: getFileColor(file.contentType) }"
                >
                  <div class="flex items-center justify-between">
                    <span class="text-sm font-medium text-gray-800">{{ file.fileName }}</span>
                  </div>
                </div>
              </div>

              <!-- Notes Section -->
              <div v-if="entry.notes" class="px-4 py-3 bg-gray-50 border-t border-gray-200">
                <p class="text-sm text-gray-700">{{ entry.notes }}</p>
              </div>

              <!-- Author Line -->
              <div class="px-4 py-2 text-xs text-gray-500 border-t border-dashed border-gray-300 flex items-center justify-between">
                <span>- {{ entry.author }}</span>
                <button v-if="isDoctor" class="text-[#B0DB9C] hover:text-[#8FC97A]">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Consistent styling based on user management */
.content-wrapper {
  width: 90%;
  max-width: 1400px;
  margin: 1.5rem auto;
  padding: 0 1rem;
}

.info-container {
  background: #CAE8BD;
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  margin-bottom: 1.5rem;
}

.user-row {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: flex-start;
  justify-content: space-between;
  padding: 1rem 1.25rem;
  background: #ECFAE5;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  font-size: 1rem;
  width: 100%;
  max-width: none;
}

.entry-card {
  background: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  overflow: hidden;
  margin-top: 1rem;
}

.entry-header {
  width: 100%;
  padding: 1rem 1.25rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  background: #CAE8BD;
  border: none;
  cursor: pointer;
  transition: background 0.2s;
  font-size: 0.95rem;
  flex-wrap: wrap;
}

.entry-header:hover {
  background: #DDF6D2;
}

.entry-header:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.entry-header > div {
  flex: 1;
  min-width: 200px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.file-card {
  cursor: pointer;
  padding: 0.75rem 1rem;
  border-radius: 6px;
  transition: all 0.2s;
  box-shadow: 0 1px 4px rgba(0,0,0,0.05);
  font-size: 0.9rem;
}

.file-card:hover {
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  transform: translateY(-1px);
}

.action-btn {
  padding: 0.5rem 1rem;
  background: #B0DB9C;
  color: #222;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  white-space: nowrap;
  width: 100%;
  justify-content: center;
}

.action-btn:hover {
  background: #8FC97A;
}

.action-btn:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.filter-select,
.filter-input {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border-radius: 4px;
  border: 1px solid #B0DB9C;
  background: #fff;
  font-size: 0.9rem;
}

.filter-select:focus,
.filter-input:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
  border-color: #8FC97A;
}

.rotate-90 {
  transform: rotate(90deg);
}

/* Tablet and above (834px and up) */
@media (min-width: 834px) {
  .content-wrapper {
    width: 85%;
    padding: 0 1.5rem;
  }

  .info-container {
    padding: 1.25rem;
    margin-bottom: 2rem;
  }

  .user-row {
    flex-direction: row;
    gap: 1.5rem;
    align-items: center;
    padding: 1.25rem 1.75rem;
    font-size: 1rem;
  }

  .entry-header {
    padding: 1.25rem 1.75rem;
    font-size: 1rem;
  }

  .entry-header > div {
    min-width: 250px;
  }

  .file-card {
    padding: 0.85rem 1.25rem;
    font-size: 0.95rem;
  }

  .action-btn {
    padding: 0.6rem 1.25rem;
    font-size: 0.95rem;
    width: auto;
    justify-content: flex-start;
  }
}

/* Desktop (1210px and up) */
@media (min-width: 1210px) {
  .content-wrapper {
    width: 75%;
    padding: 0 2rem;
  }

  .info-container {
    padding: 1.5rem;
    margin-bottom: 2rem;
  }

  .user-row {
    padding: 1.5rem 2.5rem;
    font-size: 1.05rem;
  }

  .entry-header {
    padding: 1.5rem 2.5rem;
    font-size: 1.05rem;
  }

  .entry-header > div {
    min-width: 300px;
  }

  .file-card {
    padding: 1rem 1.5rem;
    font-size: 1rem;
  }

  .action-btn {
    padding: 0.6rem 1.5rem;
    font-size: 1rem;
  }
}

/* Full HD (1920px and up) */
@media (min-width: 1920px) {
  .content-wrapper {
    width: 70%;
  }

  .user-row {
    font-size: 1.1rem;
  }

  .entry-header {
    font-size: 1.1rem;
  }

  .file-card {
    font-size: 1.05rem;
  }
}

.back-button {
  background-color: #B0DB9C;
  color: #222;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  font-weight: bold;
  cursor: pointer;
  font-size: 0.95rem;
  transition: background-color 0.3s ease;
  margin-bottom: 1rem;
  margin-top: 1rem;
}

.back-button:hover {
  background-color: #9ecb8c;
}
</style>

