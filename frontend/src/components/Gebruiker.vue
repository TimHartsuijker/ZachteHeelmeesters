<template>
  <div class="user-row">
    <span class="user-field"><strong>Naam:</strong> {{ name }}</span>
    <span class="user-field"><strong>Email:</strong> {{ email }}</span>
    
    <span class="user-field">
      <label :for="'role-select-' + userId"><strong>Rol:</strong></label>
      <select 
        v-model="selectedRoleId" 
        class="role-select" 
        :id="'role-select-' + userId"
      >
        <option v-for="rol in rollen" :key="rol.id" :value="rol.id">
          {{ formatRoleName(rol.roleName) }}
        </option>
      </select>
    </span>

    <button 
      class="save-btn" 
      @click="saveUser" 
      :disabled="saving || !isChanged"
    >
      {{ saving ? 'Opslaan...' : 'Save' }}
    </button>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import axios from 'axios';

// Props definiëren
const props = defineProps({
  userId: { type: Number, required: true },
  name: { type: String, required: true },
  email: { type: String, required: true },
  roleId: { type: Number, required: true }
});

// States (Data)
const rollen = ref([]);
const selectedRoleId = ref(props.roleId);
const originalRoleId = ref(props.roleId);
const saving = ref(false);

// Computed: Is er iets gewijzigd? (Handig voor de button state)
const isChanged = computed(() => selectedRoleId.value !== originalRoleId.value);

// Helper voor hoofdletter
const formatRoleName = (name) => {
  if (!name) return '';
  return name.charAt(0).toUpperCase() + name.slice(1);
};

// Logica voor ophalen rollen
const fetchRollen = async () => {
  try {
    const response = await axios.get('/api/roles');
    rollen.value = response.data;
  } catch (error) {
    console.error('Error fetching roles:', error);
    // Eventueel fallback rollen als de API faalt
    rollen.value = [
      { id: 1, roleName: 'Patiënt' },
      { id: 2, roleName: 'Huisarts' },
      { id: 3, roleName: 'Specialist' },
      { id: 4, roleName: 'Admin' }
    ];
  }
};

// Opslaan van de gebruiker
const saveUser = async () => {
  if (!isChanged.value) return;

  saving.value = true;
  try {
    // Let op: we gebruiken de /api prefix die de proxy verwacht
    await axios.put(`/api/users/${props.userId}`, { 
      roleId: Number(selectedRoleId.value) 
    });

    originalRoleId.value = selectedRoleId.value;
    alert('Wijzigingen zijn succesvol opgeslagen.');
  } catch (error) {
    console.error('Error updating user:', error);
    alert('Fout bij het opslaan: ' + (error.response?.data?.message || error.message));
  } finally {
    saving.value = false;
  }
};

// Watchers: Als de prop van buitenaf verandert, update de selectbox
watch(() => props.roleId, (newVal) => {
  selectedRoleId.value = newVal;
  originalRoleId.value = newVal;
});

// Lifecycle hook
onMounted(() => {
  fetchRollen();
});
</script>

<style scoped>
/* Jouw bestaande styles zijn al erg goed, ik heb ze behouden */
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
  width: 96%;
  max-width: 98%;
  margin: 1.5rem auto 0 auto;
}
.user-field {
  white-space: nowrap;
}
.save-btn {
  margin-left: auto;
  padding: 0.5rem 1.5rem;
  background: #B0DB9C;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.2s;
}
.save-btn:hover:not(:disabled) {
  background: #8FC97A;
}
.save-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}
.role-select {
  margin-left: 0.5rem;
  padding: 0.2rem 0.7rem;
  border-radius: 4px;
  border: 1px solid #B0DB9C;
  background: #fff;
  font-size: 1rem;
}
</style>