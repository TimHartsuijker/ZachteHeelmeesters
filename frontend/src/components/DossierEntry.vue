<script setup lang="ts">
import { ref } from 'vue';

interface Props {
  type: string;   
  writer: string;
  date: string;
  content: string;
}

const props = defineProps<Props>();

// Local collapse state
const isOpen = ref(false);
function toggle() {
  isOpen.value = !isOpen.value;
}

// Map plural type to singular display
const displayType = () => {
  switch (props.type) {
    case "Afspraken":
      return "Afspraak";
    case "Behandeling":
      return "Behandeling";
  }
};
</script>

<template>
  <div class="bg-white shadow-sm rounded-lg p-4 cursor-pointer">
    <!-- Collapsed header: type + date + arrow -->
    <div class="flex justify-between items-center">
      <span class="font-semibold text-gray-800">{{ displayType() }}</span>
      <div class="flex items-center gap-2">
        <span class="text-gray-500 text-sm">{{ date }}</span>
        <!-- Arrow button -->
        <button
          @click.stop="toggle"
          class="focus:outline-none focus:ring-2 focus:ring-green-500"
          :aria-expanded="isOpen"
        >
          <svg
            :class="{ 'rotate-90': isOpen }"
            class="h-5 w-5 text-green-700 transition-transform duration-200"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Expanded content -->
    <div v-if="isOpen" class="mt-2 text-gray-700 text-sm sm:text-base">
      <p>{{ content }}</p>
      <p class="mt-1 text-gray-500 text-xs">Toegevoegd door: {{ writer }}</p>
    </div>
  </div>
</template>

<style scoped>
.rotate-90 {
  transform: rotate(3deg);
}
</style>
