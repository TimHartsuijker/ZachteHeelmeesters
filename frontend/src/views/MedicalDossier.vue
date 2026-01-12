<script setup lang="ts">
import { reactive, ref, computed, onMounted } from 'vue';
import NavBar from '@/components/NavBar.vue';

interface Patient {
  name: string;
  age: number;
  doctor: string;
}

interface MedicalFile {
  id: number;
  fileName: string;
  contentType: string;
  fileSize: number;
  uploadedAt: string;
  category: string | null;
  description: string | null;
  uploadedBy: {
    id: number;
    firstName: string;
    lastName: string;
  };
  appointmentDate: string;
}

// API Configuration
const API_BASE_URL = 'https://localhost:7240/api'; // Backend URL from launchSettings.json
const PATIENT_ID = 1; // TODO: Get from auth/session

// Patient info
const patient = reactive<Patient>({
  name: "Gebruiker Naam",
  age: 23,
  doctor: "Mr Janssen",
});

// Medical files
const allFiles = ref<MedicalFile[]>([]);
const categories = ref<string[]>([]);
const isLoading = ref(false);
const error = ref<string | null>(null);

// Filter state
const tempFilterCategory = ref('');
const tempFilterFrom = ref('');
const tempFilterTo = ref('');

const activeFilterCategory = ref('');
const activeFilterFrom = ref('');
const activeFilterTo = ref('');

// Group files by category/appointment
interface GroupedEntry {
  title: string;
  date: string;
  files: MedicalFile[];
  expanded: boolean;
  notes?: string;
  author?: string;
}

const groupedEntries = computed(() => {
  const filtered = allFiles.value.filter(file => {
    const categoryMatch = !activeFilterCategory.value || activeFilterCategory.value === file.category;
    
    const fromDate = activeFilterFrom.value ? new Date(activeFilterFrom.value) : null;
    const toDate = activeFilterTo.value ? new Date(activeFilterTo.value) : null;
    const fileDate = new Date(file.uploadedAt);
    
    const afterFrom = fromDate ? fileDate >= fromDate : true;
    const beforeTo = toDate ? fileDate <= toDate : true;
    
    return categoryMatch && afterFrom && beforeTo;
  });

  // Group by appointment date and category
  const groups = new Map<string, GroupedEntry>();
  
  filtered.forEach(file => {
    const key = `${file.category || 'Overige'}-${file.appointmentDate}`;
    if (!groups.has(key)) {
      groups.set(key, {
        title: file.category || 'Overige',
        date: formatDate(file.appointmentDate),
        files: [],
        expanded: false,
        author: `${file.uploadedBy.firstName} ${file.uploadedBy.lastName}`,
        notes: file.description
      });
    }
    groups.get(key)!.files.push(file);
  });

  return Array.from(groups.values()).sort((a, b) => 
    new Date(b.date).getTime() - new Date(a.date).getTime()
  );
});

// Fetch patient dossier files
async function fetchDossierFiles() {
  isLoading.value = true;
  error.value = null;
  
  try {
    const response = await fetch(`${API_BASE_URL}/MedicalDossier/patient/${PATIENT_ID}`);
    
    if (!response.ok) {
      throw new Error('Failed to fetch dossier files');
    }
    
    allFiles.value = await response.json();
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'An error occurred';
    console.error('Error fetching dossier:', err);
  } finally {
    isLoading.value = false;
  }
}

// Fetch available categories
async function fetchCategories() {
  try {
    const response = await fetch(`${API_BASE_URL}/MedicalDossier/patient/${PATIENT_ID}/categories`);
    
    if (response.ok) {
      categories.value = await response.json();
    }
  } catch (err) {
    console.error('Error fetching categories:', err);
  }
}

// Download file
async function downloadFile(fileId: number, fileName: string) {
  try {
    const response = await fetch(`${API_BASE_URL}/MedicalDossier/file/${fileId}`);
    
    if (!response.ok) {
      throw new Error('Failed to download file');
    }
    
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  } catch (err) {
    console.error('Error downloading file:', err);
    alert('Fout bij downloaden van bestand');
  }
}

// Apply filters
function applyFilter() {
  activeFilterCategory.value = tempFilterCategory.value;
  activeFilterFrom.value = tempFilterFrom.value;
  activeFilterTo.value = tempFilterTo.value;
}

// Toggle entry expansion
function toggleEntry(entry: GroupedEntry) {
  entry.expanded = !entry.expanded;
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
  fetchDossierFiles();
  fetchCategories();
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
            <div class="flex flex-col sm:flex-row gap-3 items-start sm:items-end w-full">
              <!-- Type Filter -->
              <div class="flex-1 min-w-[150px]">
                <label class="block text-sm text-gray-700 mb-1">Type:</label>
                <select 
                  v-model="tempFilterCategory" 
                  class="filter-select"
                >
                  <option value="">Alle</option>
                  <option value="Afspraak">Afspraak</option>
                  <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                </select>
              </div>

              <!-- Date Range Filter -->
              <div class="flex-1 min-w-[200px]">
                <label class="block text-sm text-gray-700 mb-1">Tussen:</label>
                <div class="flex items-center gap-2">
                  <input 
                    type="date" 
                    v-model="tempFilterFrom" 
                    class="filter-input"
                  />
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
              <div class="flex items-center gap-3">
                <span class="font-semibold text-gray-800">{{ entry.title }} {{ entry.date }}</span>
              </div>
              <svg 
                :class="['w-5 h-5 text-gray-600 transition-transform', entry.expanded ? 'rotate-90' : '']"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </button>

            <!-- Expanded Content -->
            <div v-if="entry.expanded" class="border-t border-gray-200">
              <!-- Date Line -->
              <div class="px-4 py-2 text-xs text-gray-500 border-b border-dashed border-gray-300">
                {{ formatDateLong(entry.files[0].uploadedAt) }}
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
                <button class="text-[#B0DB9C] hover:text-[#8FC97A]">
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
  width: 70%;
  max-width: 1400px;
  margin: 2rem auto;
}

.info-container {
  background: #CAE8BD;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  margin-bottom: 2rem;
}

.user-row {
  display: flex;
  gap: 2rem;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem 2.5rem;
  background: #ECFAE5;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  font-size: 1.1rem;
  width: 100%;
  max-width: none;
}

.entry-card {
  background: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  overflow: hidden;
  margin-top: 1.5rem;
}

.entry-header {
  width: 100%;
  padding: 1.5rem 2.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #ECFAE5;
  border: none;
  cursor: pointer;
  transition: background 0.2s;
  font-size: 1.1rem;
}

.entry-header:hover {
  background: #DDF6D2;
}

.entry-header:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.file-card {
  cursor: pointer;
  padding: 1rem 1.5rem;
  border-radius: 6px;
  transition: all 0.2s;
  box-shadow: 0 1px 4px rgba(0,0,0,0.05);
}

.file-card:hover {
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  transform: translateY(-1px);
}

.action-btn {
  padding: 0.5rem 1.5rem;
  background: #B0DB9C;
  color: #222;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  white-space: nowrap;
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
  font-size: 1rem;
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
</style>

