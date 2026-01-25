<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';

const API_BASE_URL = 'https://localhost:7240/api'; // Backend URL from launchSettings.json

// Props
interface Props {
  patientId: number;
  doctorId: number;
  appointmentId: number;
}

const props = defineProps<Props>();

// Emits
const emit = defineEmits<{
  (e: 'upload-success'): void;
  (e: 'upload-error', error: string): void;
}>();

// State
const selectedFile = ref<File | null>(null);
const category = ref('');
const description = ref('');
const isUploading = ref(false);
const dragOver = ref(false);

// Predefined categories
const categories = [
  'Röntgenfoto',
  'Labresultaten',
  'Ontslagbrief',
  'Verwijsbrief',
  'Medicatielijst',
  'Behandelplan',
  'Overige'
];

// File selection
function onFileSelected(event: Event) {
  const target = event.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    selectedFile.value = target.files[0];
  }
}

// Drag and drop handlers
function onDragOver(event: DragEvent) {
  event.preventDefault();
  dragOver.value = true;
}

function onDragLeave() {
  dragOver.value = false;
}

function onDrop(event: DragEvent) {
  event.preventDefault();
  dragOver.value = false;
  
  if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
    selectedFile.value = event.dataTransfer.files[0];
  }
}

// Remove selected file
function removeFile() {
  selectedFile.value = null;
}

// Format file size
function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B';
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB';
  return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
}

async function uploadFile() {
  if (!selectedFile.value) {
    alert('Selecteer eerst een bestand');
    return;
  }

  if (!category.value) {
    alert('Selecteer een categorie');
    return;
  }

  isUploading.value = true;

  try {
    const formData = new FormData();
    formData.append('file', selectedFile.value);
    formData.append('patientId', props.patientId.toString());
    formData.append('doctorId', props.doctorId.toString());
    formData.append('appointmentId', props.appointmentId.toString());
    formData.append('category', category.value);
    
    if (description.value) {
      formData.append('description', description.value);
    }

    // Axios POST request
    const response = await axios.post(`${API_BASE_URL}/MedicalDossier/upload`, formData, {
      withCredentials: true,
      // Optioneel: voeg een upload voortgangsbalk toe
      onUploadProgress: (progressEvent) => {
        const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
        console.log(`Upload voortgang: ${percentCompleted}%`);
      }
    });

    // Reset form (Axios succes is altijd status 2xx)
    selectedFile.value = null;
    category.value = '';
    description.value = '';

    emit('upload-success');
    alert('Bestand succesvol geüpload');
  } catch (err) {
    // Axios error handling: check of de server een specifieke error message stuurde
    const errorMessage = err.response?.data?.message || err.message || 'Er is een fout opgetreden';
    
    emit('upload-error', errorMessage);
    alert('Fout bij uploaden: ' + errorMessage);
  } finally {
    isUploading.value = false;
  }
}
</script>

<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <h2 class="text-xl font-bold mb-4 text-gray-800">Document Uploaden</h2>
    
    <!-- Drag and drop area -->
    <div
      @dragover="onDragOver"
      @dragleave="onDragLeave"
      @drop="onDrop"
      :class="[
        'border-2 border-dashed rounded-lg p-8 text-center transition-colors',
        dragOver ? 'border-[var(--color-green)] bg-green-50' : 'border-gray-300'
      ]"
    >
      <div v-if="!selectedFile">
        <svg class="mx-auto h-12 w-12 text-gray-400" stroke="currentColor" fill="none" viewBox="0 0 48 48">
          <path d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
        </svg>
        <p class="mt-2 text-sm text-gray-600">
          <label class="cursor-pointer text-[var(--color-green-dark)] font-semibold hover:underline">
            Klik om een bestand te selecteren
            <input type="file" class="hidden" @change="onFileSelected" accept=".pdf,.doc,.docx,.jpg,.jpeg,.png,.txt" />
          </label>
          of sleep het hierheen
        </p>
        <p class="text-xs text-gray-500 mt-1">PDF, Word, JPG, PNG tot 10MB</p>
      </div>

      <!-- Selected file preview -->
      <div v-else class="flex items-center justify-between bg-gray-50 rounded p-3">
        <div class="flex items-center gap-3 flex-1 min-w-0">
          <svg class="h-8 w-8 text-[var(--color-green)] flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <div class="flex-1 min-w-0">
            <p class="font-medium text-gray-800 truncate">{{ selectedFile.name }}</p>
            <p class="text-sm text-gray-500">{{ formatFileSize(selectedFile.size) }}</p>
          </div>
        </div>
        <button
          @click="removeFile"
          class="flex-shrink-0 ml-3 text-red-600 hover:text-red-800"
          title="Verwijder bestand"
        >
          <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Category selection -->
    <div class="mt-4">
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Categorie <span class="text-red-500">*</span>
      </label>
      <select
        v-model="category"
        class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[var(--color-green)]"
        required
      >
        <option value="" disabled>Selecteer een categorie</option>
        <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
      </select>
    </div>

    <!-- Description -->
    <div class="mt-4">
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Beschrijving (optioneel)
      </label>
      <textarea
        v-model="description"
        class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[var(--color-green)]"
        rows="3"
        placeholder="Voeg eventuele notities of beschrijving toe..."
      ></textarea>
    </div>

    <!-- Upload button -->
    <div class="mt-6 flex justify-end">
      <button
        @click="uploadFile"
        :disabled="!selectedFile || !category || isUploading"
        :class="[
          'px-6 py-2 rounded-lg font-semibold transition-colors',
          !selectedFile || !category || isUploading
            ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
            : 'bg-[var(--color-green)] text-black hover:bg-[var(--color-green-dark)] border-2 border-[var(--color-green-dark)]'
        ]"
      >
        <span v-if="isUploading" class="flex items-center gap-2">
          <svg class="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Uploaden...
        </span>
        <span v-else>Document Uploaden</span>
      </button>
    </div>
  </div>
</template>

<style scoped>
/* Additional styles if needed */
</style>
