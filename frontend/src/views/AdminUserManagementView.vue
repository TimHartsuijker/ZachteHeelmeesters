<script setup>
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
      { rolId: 1, roleName: 'Patiënt' },
      { rolId: 2, roleName: 'Huisarts' },
      { rolId: 3, roleName: 'Specialist' },
      { rolId: 4, roleName: 'Systeembeheerder' }
    ]
  }
}

// AdminUserManagementView.vue
const filteredUsers = computed(() => {
  return users.value.filter(user => {
    // Voeg extra checks toe (user.firstName || "") om undefined errors te voorkomen
    const firstName = user.firstName || "";
    const lastName = user.lastName || "";
    const email = user.email || "";

    const searchLower = filterOptions.value.search.toLowerCase();
    const fullName = `${firstName} ${lastName}`.toLowerCase();
    
    const matchesSearch = fullName.includes(searchLower) || email.toLowerCase().includes(searchLower);
    

    const role = rollen.value.find(r => r.roleName === filterOptions.value.role);
    const matchesRole = !filterOptions.value.role || user.roleId === role?.id;

    return matchesSearch && matchesRole;
  });
});

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
});
</script>

<template>
  <main class="main-users align-under-nav">
    <Filter 
      :rollen="rollen"
      @filter-change="handleFilterChange"
    />
    
    <div v-if="loading" class="status-message">Loading users...</div>
    <div v-else-if="error" class="status-message error">Error: {{ error }}</div>
    <div v-else-if="filteredUsers.length === 0" class="status-message">No users found</div>
    <div v-else>
      <Gebruikers
        v-for="user in filteredUsers"
        :key="user.id"
        :userId="user.id"
        :name="`${user.firstName} ${user.lastName}`"
        :email="user.email"
        :roleId="user.roleId" 
        @update-user="handleUpdateUser"
      />
    </div>
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