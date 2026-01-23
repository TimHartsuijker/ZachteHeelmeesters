<template>
  <div class="admin-create-user-page align-under-nav">
    <div class="main-users">
      <h2>Nieuwe Gebruiker Aanmaken</h2>
      
      <!-- AC6.10.1: Formulier voor account aanmaken -->
      <form @submit.prevent="createUser" class="create-user-form">
        
        <!-- Persoonlijke gegevens -->
        <div class="form-section">
          <h3>Persoonlijke Gegevens</h3>
          
          <div class="form-row">
            <div class="form-group">
              <label for="firstName">Voornaam *</label>
              <input 
                type="text" 
                id="firstName" 
                v-model="formData.firstName" 
                required
                placeholder="Voornaam"
              />
            </div>
            
            <div class="form-group">
              <label for="lastName">Achternaam *</label>
              <input 
                type="text" 
                id="lastName" 
                v-model="formData.lastName" 
                required
                placeholder="Achternaam"
              />
            </div>
          </div>
          
          <div class="form-group">
            <label for="email">E-mailadres *</label>
            <input 
              type="email" 
              id="email" 
              v-model="formData.email" 
              required
              placeholder="voorbeeld@email.com"
            />
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label for="dateOfBirth">Geboortedatum</label>
              <input 
                type="date" 
                id="dateOfBirth" 
                v-model="formData.dateOfBirth"
              />
            </div>
            
            <div class="form-group">
              <label for="gender">Geslacht</label>
              <select id="gender" v-model="formData.gender">
                <option value="">Selecteer geslacht</option>
                <option value="Man">Man</option>
                <option value="Vrouw">Vrouw</option>
                <option value="Ander">Ander</option>
                <option value="Zeg ik liever niet">Zeg ik liever niet</option>
              </select>
            </div>
          </div>
          
          <div class="form-group">
            <label for="phoneNumber">Telefoonnummer</label>
            <input 
              type="tel" 
              id="phoneNumber" 
              v-model="formData.phoneNumber"
              placeholder="0612345678"
            />
          </div>
        </div>
        
        <!-- Adresgegevens -->
        <div class="form-section">
          <h3>Adresgegevens</h3>
          
          <div class="form-row">
            <div class="form-group">
              <label for="streetName">Straatnaam</label>
              <input 
                type="text" 
                id="streetName" 
                v-model="formData.streetName"
                placeholder="Straatnaam"
              />
            </div>
            
            <div class="form-group">
              <label for="houseNumber">Huisnummer</label>
              <input 
                type="text" 
                id="houseNumber" 
                v-model="formData.houseNumber"
                placeholder="123"
              />
            </div>
          </div>
          
          <div class="form-group">
            <label for="postalCode">Postcode</label>
            <input 
              type="text" 
              id="postalCode" 
              v-model="formData.postalCode"
              placeholder="1234AB"
            />
          </div>
          
          <div class="form-group">
            <label for="citizenServiceNumber">BSN</label>
            <input 
              type="text" 
              id="citizenServiceNumber" 
              v-model="formData.citizenServiceNumber"
              placeholder="123456789"
              maxlength="9"
            />
          </div>
        </div>
        
        <!-- AC6.10.5: Rol selectie -->
        <div class="form-section">
          <h3>Account Type *</h3>
          
          <div class="form-group">
            <label for="roleId">Soort Gebruiker</label>
            <select 
              id="roleId" 
              v-model="formData.roleId" 
              required
              @change="onRoleChange"
            >
              <option value="">Selecteer rol</option>
              <option v-for="role in roles" :key="role.id" :value="role.id">
                {{ role.roleName }}
              </option>
            </select>
          </div>
          
          <!-- Extra velden voor artsen/specialisten -->
          <div v-if="showPracticeField" class="form-group">
            <label for="practiceName">Praktijknaam</label>
            <input 
              type="text" 
              id="practiceName" 
              v-model="formData.practiceName"
              placeholder="Naam van de praktijk"
            />
          </div>
        </div>
        
        <!-- Foutmeldingen -->
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
        
        <!-- Succesmelding -->
        <div v-if="successMessage" class="success-message">
          {{ successMessage }}
          <div v-if="temporaryPassword" class="temp-password">
            <strong>Tijdelijk wachtwoord:</strong> {{ temporaryPassword }}
            <small>De gebruiker moet dit wachtwoord bij eerste login wijzigen.</small>
          </div>
        </div>
        
        <!-- AC6.10.2: Knop om account toe te voegen -->
        <div class="form-actions">
          <button type="submit" class="btn-primary" :disabled="isLoading">
            {{ isLoading ? 'Bezig...' : 'Account Aanmaken' }}
          </button>
          <button type="button" class="btn-secondary" @click="resetForm">
            Formulier Leegmaken
          </button>
        </div>
        
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'

const router = useRouter()

// Formulier data
const formData = ref({
  firstName: '',
  lastName: '',
  email: '',
  streetName: '',
  houseNumber: '',
  postalCode: '',
  citizenServiceNumber: '',
  dateOfBirth: '',
  gender: '',
  phoneNumber: '',
  roleId: '',
  practiceName: ''
})

const roles = ref([])
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const temporaryPassword = ref('')

// Computed property om te bepalen of praktijknaam veld moet worden getoond
const showPracticeField = computed(() => {
  const selectedRole = roles.value.find(r => r.id === parseInt(formData.value.roleId))
  return selectedRole && ['Huisarts', 'Specialist'].includes(selectedRole.roleName)
})

// Functie bij wijzigen rol
const onRoleChange = () => {
  if (!showPracticeField.value) {
    formData.value.practiceName = ''
  }
}

// Laad rollen bij mount
onMounted(async () => {
  try {
    const response = await axios.get('/api/roles')
    roles.value = response.data
  } catch (error) {
    console.error('Fout bij laden rollen:', error)
    errorMessage.value = 'Kon rollen niet laden'
  }
})

// AC6.10.2 & AC6.10.3 & AC6.10.4: Account aanmaken functie
const createUser = async () => {
  // Reset messages
  errorMessage.value = ''
  successMessage.value = ''
  temporaryPassword.value = ''
  
  // Validatie
  if (!formData.value.firstName || !formData.value.lastName || 
      !formData.value.email || !formData.value.roleId) {
    errorMessage.value = 'Vul alle verplichte velden in (gemarkeerd met *)'
    return
  }
  
  // Email validatie
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(formData.value.email)) {
    errorMessage.value = 'Vul een geldig e-mailadres in'
    return
  }
  
  // BSN validatie indien ingevuld
  if (formData.value.citizenServiceNumber && 
      !/^\d{9}$/.test(formData.value.citizenServiceNumber)) {
    errorMessage.value = 'BSN moet uit 9 cijfers bestaan'
    return
  }
  
  isLoading.value = true
  
  try {
    // Prepare data for API
    const requestData = {
      ...formData.value,
      dateOfBirth: formData.value.dateOfBirth || null
    }
    
    // API call
    const response = await axios.post('/api/adminuser/create', requestData)
    
    if (response.status === 200) {
      successMessage.value = response.data.message
      temporaryPassword.value = response.data.temporaryPassword
      resetForm()
      
      // Optioneel: doorsturen naar gebruikersoverzicht
      setTimeout(() => {
        router.push('/admin/users')
      }, 3000)
    }
  } catch (error) {
    console.error('Fout bij aanmaken gebruiker:', error)
    errorMessage.value = error.response?.data?.message || 'Er is een fout opgetreden bij het aanmaken van het account'
  } finally {
    isLoading.value = false
  }
}

// Formulier resetten
const resetForm = () => {
  formData.value = {
    firstName: '',
    lastName: '',
    email: '',
    streetName: '',
    houseNumber: '',
    postalCode: '',
    citizenServiceNumber: '',
    dateOfBirth: '',
    gender: '',
    phoneNumber: '',
    roleId: '',
    practiceName: ''
  }
}
</script>

<style scoped>
.admin-create-user-page {
  padding: 2rem;
}

.main-users {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 16px rgba(0,0,0,0.1);
  max-width: 800px;
  margin: 0 auto;
}

h2 {
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

h3 {
  color: #555;
  margin: 1.5rem 0 1rem 0;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #B0DB9C;
}

.form-section {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: #f9f9f9;
  border-radius: 8px;
}

.form-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.form-row .form-group {
  flex: 1;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #555;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #B0DB9C;
  box-shadow: 0 0 0 2px rgba(176, 219, 156, 0.2);
}

.form-group input[required] {
  border-left: 4px solid #B0DB9C;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  margin-top: 2rem;
}

.btn-primary {
  padding: 0.75rem 2rem;
  background-color: #00960c;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn-primary:hover:not(:disabled) {
  background-color: #007a0a;
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  padding: 0.75rem 2rem;
  background-color: #f0f0f0;
  color: #333;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn-secondary:hover {
  background-color: #e0e0e0;
}

.error-message {
  background-color: #fee;
  color: #d32f2f;
  padding: 1rem;
  border-radius: 6px;
  margin: 1rem 0;
  border-left: 4px solid #d32f2f;
}

.success-message {
  background-color: #e8f5e8;
  color: #388e3c;
  padding: 1rem;
  border-radius: 6px;
  margin: 1rem 0;
  border-left: 4px solid #388e3c;
}

.temp-password {
  margin-top: 1rem;
  padding: 0.75rem;
  background: #fff;
  border-radius: 4px;
  border: 1px dashed #388e3c;
}

.temp-password small {
  display: block;
  margin-top: 0.5rem;
  color: #666;
  font-size: 0.85rem;
}
</style>