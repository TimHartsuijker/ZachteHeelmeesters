<script setup>
import NavBar from '../components/nav.vue'
import Filter from '../components/Filter.vue'
import Gebruikers from '../components/Gebruiker.vue'
import { ref, onMounted, computed } from 'vue'

const users = ref([])
const rollen = ref([])
const loading = ref(true)
const error = ref(null)

const filterOptions = ref({
  search: "",
  role: ""
})

const fetchUsers = async () => {
  try {
    loading.value = true
    const response = await fetch('https://localhost:7240/api/users')
    if (!response.ok) throw new Error('Failed to fetch users')
    users.value = await response.json()
    error.value = null
  } catch (err) {
    console.error('Error fetching users:', err)
    error.value = 'Cannot connect to backend. Make sure the backend is running (dotnet run in backend folder)'
  } finally {
    loading.value = false
  }
}

const fetchRollen = async () => {
  try {
    const response = await fetch('https://localhost:7240/api/roles')
    if (!response.ok) throw new Error('Failed to fetch roles')
    rollen.value = await response.json()
  } catch (error) {
    console.error('Error fetching roles:', error)
    rollen.value = [
      { rolID: 1, rolnaam: 'patiënt' },
      { rolID: 2, rolnaam: 'huisarts' },
      { rolID: 3, rolnaam: 'specialist' },
      { rolID: 4, rolnaam: 'administrator' },
      { rolID: 5, rolnaam: 'Systeembeheerder' }
    ]
  }
}

const filteredUsers = computed(() => {
  return users.value.filter(user => {
    // Search filter (name and email)
    if (filterOptions.value.search) {
      const searchLower = filterOptions.value.search.toLowerCase()
      const fullName = `${user.voornaam} ${user.achternaam}`.toLowerCase()
      const emailLower = user.email.toLowerCase()
      
      const matchesSearch = fullName.includes(searchLower) || emailLower.includes(searchLower)
      if (!matchesSearch) return false
    }

    // Role filter
    if (filterOptions.value.role) {
      if (user.rolnaam !== filterOptions.value.role) return false
    }

    return true
  })
})

const handleFilterChange = (filters) => {
  filterOptions.value = filters
}

const handleUpdateUser = async (userData) => {
  console.log('Update user:', userData)
}

onMounted(() => {
  console.log('App.vue mounted')
  fetchRollen()
  fetchUsers()
})
</script>

<template>
  <header>
    <div class="wrapper">
      <NavBar/>
    </div>
  </header>

  <main class="main-users align-under-nav">
    <Filter 
      :rollen="rollen"
      @filter-change="handleFilterChange"
    />
    
    <div v-if="loading" class="status-message">Loading users...</div>
    <div v-else-if="error" class="status-message error">Error: {{ error }}</div>
    <div v-else-if="filteredUsers.length === 0" class="status-message">No users found</div>
    <template v-else>
      <Gebruikers
        v-for="user in filteredUsers"
        :key="user.gebruikersID"
        :userId="user.gebruikersID"
        :name="`${user.voornaam} ${user.achternaam}`"
        :email="user.email"
        :role="user.rolnaam || ''"
        :roleId="user.rol"
        @update-user="handleUpdateUser"
      />
    </template>
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
  width: 100vw;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1000;
}
#app {
  background-color: #ECFAE5;
}
</style>