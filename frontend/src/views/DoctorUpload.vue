<script setup lang="ts">
import { ref, onMounted } from 'vue';
import FileUpload from '@/components/FileUpload.vue';
import NavBar from '@/components/NavBar.vue';

// TODO: Get these from authentication/session
const DOCTOR_ID = 2;
const PATIENT_ID = 1;
const APPOINTMENT_ID = 1;

const uploadSuccess = ref(false);

function onUploadSuccess() {
  uploadSuccess.value = true;
  setTimeout(() => {
    uploadSuccess.value = false;
  }, 3000);
}

function onUploadError(error: string) {
  console.error('Upload error:', error);
}
</script>

<template>
  <div class="min-h-screen bg-[#ECFAE5] flex flex-col" style="padding-top: 70px;">
    
    <!-- Navigation -->
    <NavBar />

    <!-- Success message -->
    <div v-if="uploadSuccess" class="mx-auto mt-6 max-w-2xl w-full px-4">
      <div class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded">
        ✓ Document succesvol geüpload naar het patiëntendossier
      </div>
    </div>

    <!-- Main content -->
    <main class="flex-1 w-full p-4 mt-6 flex justify-center">
      <div class="w-full max-w-2xl">
        <FileUpload
          :patient-id="PATIENT_ID"
          :doctor-id="DOCTOR_ID"
          :appointment-id="APPOINTMENT_ID"
          @upload-success="onUploadSuccess"
          @upload-error="onUploadError"
        />

        <!-- Info box -->
        <div class="mt-6 bg-blue-50 border border-blue-200 rounded-lg p-4">
          <h3 class="font-semibold text-blue-900 mb-2">Instructies voor uploaden:</h3>
          <ul class="text-sm text-blue-800 space-y-1">
            <li>• Toegestane bestandsformaten: PDF, Word (.doc, .docx), afbeeldingen (.jpg, .png)</li>
            <li>• Maximum bestandsgrootte: 10 MB</li>
            <li>• Selecteer altijd de juiste categorie voor het document</li>
            <li>• Voeg indien mogelijk een beschrijving toe voor meer context</li>
            <li>• Het document wordt direct zichtbaar in het patiëntendossier</li>
          </ul>
        </div>
      </div>
    </main>

  </div>
</template>

<style scoped>
/* Additional styles if needed */
</style>
