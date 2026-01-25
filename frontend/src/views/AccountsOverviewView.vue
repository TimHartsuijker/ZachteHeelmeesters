<template>
  <div class="accounts-overview-page align-under-nav">
    <div class="main-users">
      <h1>Administratie</h1>
      
      <div class="section-header">
        <RouterLink to="/administratie/create-user" class="btn-add-account">
          + Account aanmaken
        </RouterLink>
      </div>
      
      <!-- Zoekbalk -->
      <div class="search-bar">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Zoeken..."
          class="search-input"
        />
      </div>
      
      <!-- Status meldingen -->
      <div v-if="isLoading" class="status-message">
        Accounts laden...
      </div>
      
      <div v-else-if="errorMessage" class="status-message error">
        {{ errorMessage }}
      </div>
      
      <div v-else-if="filteredUsers.length === 0" class="status-message">
        Geen accounts gevonden
      </div>
      
      <!-- Accounts lijst - ZONDER actieknoppen -->
      <div v-else class="accounts-list">
        <div class="account-card" v-for="user in filteredUsers" :key="user.id">
          <div class="account-info">
            <h3 class="account-name">{{ user.firstName }} {{ user.lastName }}</h3>
            <p class="account-email">Email: {{ user.email }}</p>
            <p class="account-role">Rol: {{ user.roleName }}</p>
            <p class="account-created">Aangemaakt: {{ formatDate(user.createdAt) }}</p>
          </div>
          <!-- GEEN actieknoppen meer -->
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

// State
const users = ref([])
const isLoading = ref(true)
const errorMessage = ref('')
const searchQuery = ref('')

// Laad gebruikers bij mount
onMounted(async () => {
  await loadUsers()
})

// Laad gebruikers van API
const loadUsers = async () => {
  isLoading.value = true
  errorMessage.value = ''
  
  try {
    // Gebruik een endpoint dat meer data teruggeeft
    const response = await axios.get('/api/users/all-with-roles')
    users.value = response.data.map(user => ({
      id: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      roleName: user.roleName || getRoleName(user.roleId),
      createdAt: user.createdAt || new Date().toISOString()
    }))
  } catch (error) {
    console.error('Fout bij laden gebruikers:', error)
    // Fallback naar normale endpoint
    try {
      const fallbackResponse = await axios.get('/api/users')
      users.value = fallbackResponse.data.map(user => ({
        id: user.id,
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        roleName: getRoleName(user.roleId),
        createdAt: new Date().toISOString()
      }))
    } catch (fallbackError) {
      errorMessage.value = 'Kon accounts niet laden'
    }
  } finally {
    isLoading.value = false
  }
}

// Helper om rolnaam te bepalen
const getRoleName = (roleId) => {
  const roleMap = {
    1: 'Patiënt',
    2: 'Huisarts',
    3: 'Specialist',
    4: 'Admin',
    5: 'Administratiemedewerker'
  }
  return roleMap[roleId] || 'Onbekend'
}

// Format datum
const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('nl-NL', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

// Filter gebruikers op zoekquery
const filteredUsers = computed(() => {
  if (!searchQuery.value.trim()) {
    return users.value.filter(user => user.roleName !== 'Patiënt')
  }
  
  const query = searchQuery.value.toLowerCase()
  return users.value.filter(user => 
    (user.roleName !== 'Patiënt') && (
      user.firstName.toLowerCase().includes(query) ||
      user.lastName.toLowerCase().includes(query) ||
      user.email.toLowerCase().includes(query) ||
      user.roleName.toLowerCase().includes(query)
    )
  )
})
</script>

<style scoped>
.accounts-overview-page {
  padding: 2rem;
  width: 100%;
}

.main-users {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 16px rgba(0,0,0,0.1);
  max-width: 1000px;
  margin: 0 auto;
}

h1 {
  color: #333;
  margin-bottom: 0.5rem;
  font-size: 2rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #eee;
}

.btn-add-account {
  padding: 0.5rem 1.5rem;
  background-color: #00960c;
  color: white;
  text-decoration: none;
  border-radius: 6px;
  font-weight: 500;
  transition: background-color 0.3s;
}

.btn-add-account:hover {
  background-color: #007a0a;
}

.search-bar {
  margin-bottom: 2rem;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
}

.search-input:focus {
  outline: none;
  border-color: #00960c;
  box-shadow: 0 0 0 2px rgba(0, 150, 12, 0.2);
}

/* Accounts lijst - ALLES ONDER ELKAAR */
.accounts-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 2rem;
}

.account-card {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1.25rem;
  border: 1px solid #dee2e6;
  display: grid;
  grid-template-columns: 2fr 3fr 2fr;
  gap: 1.5rem;
  align-items: center;
  transition: background-color 0.2s;
}

.account-info {
  display: contents; /* Dit zorgt ervoor dat de children direct in de grid komen */
}

.account-name {
  grid-column: 1;
  margin: 0;
  color: #333;
  font-size: 1rem;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.account-email {
  grid-column: 2;
  margin: 0;
  color: #666;
  font-size: 0.95rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.account-role {
  grid-column: 3;
  margin: 0;
  color: #00960c;
  font-weight: 500;
  font-size: 0.95rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.account-created {
  display: none; /* Verberg de aanmaakdatum zoals in de afbeelding */
}

.status-message {
  text-align: center;
  padding: 3rem;
  color: #666;
  font-size: 1.1rem;
}

.status-message.error {
  color: #d32f2f;
}

.account-card:hover {
  background-color: #f1f3f5;
}

/* Optioneel: voeg wat padding toe binnen de grid items voor betere leesbaarheid */
.account-name,
.account-email,
.account-role {
  padding: 0.25rem 0;
}
</style>