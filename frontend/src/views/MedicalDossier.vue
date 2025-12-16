<script setup lang="ts">
import { reactive, ref, computed } from 'vue';
import DossierEntry from '@/components/DossierEntry.vue';

interface Patient {
  name: string;
  age: number;
  doctor: string;
}

interface Entry {
  type: string;
  writer: string;
  date: string;
  content: string;
}

// Patient info
const patient = reactive<Patient>({
  name: "Jane Smith",
  age: 42,
  doctor: "Dr. Kim",
});

// All entries
const allEntries = reactive<Entry[]>([
  { type: "Afspraken", writer: "Dr. Kim", date: "2025-12-16", content: "Inhoud" },
  { type: "Behandeling", writer: "Nurse Anna", date: "2025-12-15", content: "Inhoud" },
  { type: "Behandeling", writer: "Dr. Kim", date: "2025-12-18", content: "Inhoud" },
  { type: "Behandeling", writer: "Nurse Anna", date: "2025-12-19", content: "Inhoud" },
  { type: "Afspraken", writer: "Dr. Kim", date: "2025-12-20", content: "Routine checkup scheduled." },
]);

// Temporary form filter inputs (bound to UI)
const tempFilterType = ref('Alles');
const tempFilterFrom = ref('');
const tempFilterTo = ref('');

// Active filters applied after clicking "Toepassen"
const activeFilterType = ref('Alles');
const activeFilterFrom = ref('');
const activeFilterTo = ref('');

// Computed filtered entries (based on active filters)
const filteredEntries = computed(() => {
  return allEntries
    .filter(entry => {
      const typeMatch = activeFilterType.value === 'Alles' || activeFilterType.value === entry.type;

      const fromDate = activeFilterFrom.value ? new Date(activeFilterFrom.value) : null;
      const toDate = activeFilterTo.value ? new Date(activeFilterTo.value) : null;
      const entryDate = new Date(entry.date);

      const afterFrom = fromDate ? entryDate >= fromDate : true;
      const beforeTo = toDate ? entryDate <= toDate : true;

      return typeMatch && afterFrom && beforeTo;
    })
    .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
});

// Apply filter function
function applyFilter() {
  activeFilterType.value = tempFilterType.value;
  activeFilterFrom.value = tempFilterFrom.value;
  activeFilterTo.value = tempFilterTo.value;
}
</script>

<template>
  <div class="w-screen min-h-screen bg-[var(--color-green-lightest)] flex flex-col overflow-x-hidden">

    <!-- Header -->
    <header class="w-full bg-[var(--color-green)] text-black p-6 flex flex-col gap-2">
      <h1 class="text-2xl font-bold">{{ patient.name }}</h1>
      <p>{{ patient.age }} jaar</p>
      <p>Huisarts {{ patient.doctor }}</p>
    </header>

    <!-- Filter Section -->
    <section class="w-full bg-white p-4 md:p-6 flex flex-col gap-4">
      <div class="flex flex-col sm:flex-row sm:items-center gap-2 flex-wrap w-full md:w-auto">
        <label class="text-gray-700 text-sm flex items-center gap-1">
          Type:
          <select v-model="tempFilterType" class="border border-gray-300 rounded px-2 py-1">
            <option>Alles</option>
            <option>Behandeling</option>
            <option>Afspraken</option>
          </select>
        </label>

        <label class="text-gray-700 text-sm flex items-center gap-1">
          Tussen:
          <input type="date" v-model="tempFilterFrom" class="border border-gray-300 rounded px-2 py-1" />
          -
          <input type="date" v-model="tempFilterTo" class="border border-gray-300 rounded px-2 py-1" />
        </label>
      </div>

      <div class="w-full flex justify-start">
        <button 
          @click="applyFilter"
          class="bg-[var(--color-green)] text-black font-semibold px-4 py-2 rounded 
                 border-[2px] border-[var(--color-green-dark)] shadow-md hover:bg-[var(--color-green-dark)]">
          Toepassen
        </button>
      </div>
    </section>

    <!-- Main Content: Entries -->
    <main class="flex-1 w-full bg-[var(--color-gray-light)] p-4 mt-4 flex justify-center">
      <div class="w-full max-w-3xl flex flex-col gap-4 px-2 sm:px-4">
        <DossierEntry
          v-for="(entry, index) in filteredEntries"
          :key="index"
          class="w-full"
          :type="entry.type"
          :writer="entry.writer"
          :date="entry.date"
          :content="entry.content"
        />
      </div>
    </main>

  </div>
</template>
