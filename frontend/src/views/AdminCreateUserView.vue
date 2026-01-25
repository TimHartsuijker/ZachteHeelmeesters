<template>
  <div class="admin-create-user-page align-under-nav">
    <div class="main-users">
      <!-- Terug knop naar overzicht -->
      <div class="back-button">
        <RouterLink to="/administratie/accounts" class="back-link">
          ← Terug naar Accounts Overzicht
        </RouterLink>
      </div>
      
      <h1>Nieuw Account Aanmaken</h1>
      
      <form @submit.prevent="createUser" class="create-user-form">
        
        <!-- Persoonlijke gegevens -->
        <div class="form-section">
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
            <label for="email">Email *</label>
            <input 
              type="email" 
              id="email" 
              v-model="formData.email" 
              required
              placeholder="email@example.com"
            />
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
          
          <!-- 🔧 WACHTWOORD VELDEN - NU WEL VERPLICHT -->
          <div class="form-row">
            <div class="form-group">
              <label for="password">Wachtwoord *</label>
              <input 
                type="password" 
                id="password" 
                v-model="formData.password"
                required
                placeholder="Minimaal 8 karakters"
                minlength="8"
              />
              <p class="help-text">Wachtwoord moet minimaal 8 karakters lang zijn</p>
            </div>
            
            <div class="form-group">
              <label for="confirmPassword">Bevestig Wachtwoord *</label>
              <input 
                type="password" 
                id="confirmPassword" 
                v-model="confirmPassword"
                required
                placeholder="Herhaal wachtwoord"
              />
            </div>
          </div>
          
          <!-- Adresgegevens -->
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
          
          <div class="form-row">
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
        </div>
        
        <!-- Rol selectie -->
        <div class="form-section">
          <div class="form-group">
            <label for="roleId">Rol *</label>
            <select 
              id="roleId" 
              v-model="formData.roleId" 
              required
              class="role-select"
            >
              <option value="">Selecteer een rol</option>
              <option v-for="role in filteredRoles" :key="role.id" :value="role.id">
                {{ role.roleName }}
              </option>
            </select>
            <p class="help-text" v-if="formData.roleId && selectedRoleName === 'Admin'">
              <strong>Let op:</strong> Admin accounts moeten inloggen via de aparte admin login pagina.
            </p>
          </div>
        </div>
        
        <!-- Foutmeldingen -->
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
        
        <!-- Knoppen -->
        <div class="form-actions">
          <button type="submit" class="btn-primary" :disabled="isLoading">
            {{ isLoading ? 'Bezig...' : 'Account Aanmaken' }}
          </button>
          <button type="button" class="btn-secondary" @click="cancelForm">
            Annuleren
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
  phoneNumber: '',
  password: '', // 🔧 NU verplicht
  streetName: '',
  houseNumber: '',
  postalCode: '',
  citizenServiceNumber: '',
  dateOfBirth: '',
  gender: '',
  roleId: ''
})

const confirmPassword = ref('')
const roles = ref([])
const isLoading = ref(false)
const errorMessage = ref('')

// Filter rollen - NU ook Administratiemedewerker toestaan
const filteredRoles = computed(() => {
  return roles.value.filter(role => 
    role.roleName !== 'Patiënt'
    // ALLEEN Patiënt uitsluiten, dus Admin en Administratiemedewerker zijn toegestaan
  )
})

// Bepaal geselecteerde rol naam
const selectedRoleName = computed(() => {
  const role = roles.value.find(r => r.id === parseInt(formData.value.roleId))
  return role ? role.roleName : ''
})

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

// Account aanmaken functie
const createUser = async () => {
  // Reset messages
  errorMessage.value = ''
  
  console.log('Formulier data:', formData.value)
  console.log('Confirm password:', confirmPassword.value)
  
  // 🔧 WACHTWOORD NU TOEGEVOEGD AAN BASISVALIDATIE
  if (!formData.value.firstName || !formData.value.lastName || 
      !formData.value.email || !formData.value.password || // 🔧 Password nu verplicht
      !formData.value.roleId) {
    errorMessage.value = 'Vul alle verplichte velden in (gemarkeerd met *)'
    return
  }
  
  // 🔧 Wachtwoord validatie - NU ALTIJD omdat het verplicht is
  // 1. Controleer of wachtwoorden overeenkomen
  if (formData.value.password !== confirmPassword.value) {
    errorMessage.value = 'Wachtwoorden komen niet overeen'
    return
  }
  
  // 2. Controleer minimale lengte
  if (formData.value.password.length < 8) {
    errorMessage.value = 'Wachtwoord moet minimaal 8 karakters lang zijn'
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
      firstName: formData.value.firstName,
      lastName: formData.value.lastName,
      email: formData.value.email,
      phoneNumber: formData.value.phoneNumber || null,
      password: formData.value.password, // 🔧 NU altijd meegestuurd (niet meer null)
      streetName: formData.value.streetName || null,
      houseNumber: formData.value.houseNumber || null,
      postalCode: formData.value.postalCode || null,
      citizenServiceNumber: formData.value.citizenServiceNumber || null,
      dateOfBirth: formData.value.dateOfBirth || null,
      gender: formData.value.gender || null,
      roleId: parseInt(formData.value.roleId)
    }
    
    console.log('Versturen naar API:', requestData)
    
    // API call naar AdminUserController
    const response = await axios.post('/api/adminuser/create', requestData)
    
    console.log('API Response:', response.data)
    
    if (response.status === 200) {
      // ✅ DIRECT TERUG NAAR OVERZICHT MET NOTIFICATIE
      const successMessage = 'Het account is aangemaakt!';
      const userEmail = formData.value.email;
      
      // Sla notificatie op voor AccountsOverview
      sessionStorage.setItem('accountCreatedNotification', successMessage);
      sessionStorage.setItem('createdUserEmail', userEmail);
      
      // Reset formulier
      resetForm();
      
      // Ga direct terug naar overzicht
      router.push('/administratie/accounts');
    }
  } catch (error) {
    console.error('Fout bij aanmaken gebruiker:', error)
    
    if (error.response?.data?.message) {
      errorMessage.value = error.response.data.message
    } else if (error.response?.data?.error) {
      // Toon backend error details
      errorMessage.value = `Backend fout: ${error.response.data.error}`
      if (error.response.data.details) {
        errorMessage.value += ` (${error.response.data.details})`
      }
    } else if (error.code === 'ERR_NETWORK') {
      errorMessage.value = 'Kan geen verbinding maken met de server. Controleer of de backend draait.'
    } else {
      errorMessage.value = 'Er is een fout opgetreden bij het aanmaken van het account.'
    }
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
    phoneNumber: '',
    password: '', // 🔧 Reset naar lege string
    streetName: '',
    houseNumber: '',
    postalCode: '',
    citizenServiceNumber: '',
    dateOfBirth: '',
    gender: '',
    roleId: ''
  }
  confirmPassword.value = ''
}

// Annuleren
const cancelForm = () => {
  router.push('/administratie/accounts')
}
</script>

<style scoped>
.admin-create-user-page {
  padding: 2rem;
  width: 100%;
}

.main-users {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 16px rgba(0,0,0,0.1);
  max-width: 100%;
  margin: 0 auto;
}

.back-button {
  margin-bottom: 1.5rem;
}

.back-link {
  display: inline-flex;
  align-items: center;
  color: #666;
  text-decoration: none;
  font-size: 0.9rem;
  transition: color 0.2s;
}

.back-link:hover {
  color: #00960c;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
  font-size: 1.8rem;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #eee;
}

.create-user-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  width: 100%;
}

.form-section {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1.5rem;
  border: 1px solid #dee2e6;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-row {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
  width: 100%;
}

.form-row .form-group {
  flex: 1;
  margin-bottom: 0;
  min-width: 0; /* Voorkomt overflow */
}

.form-row:last-child {
  margin-bottom: 0;
}

/* Eenvoudigere label styling */
label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
  width: 100%;
}

/* Input velden styling - volle breedte */
input[type="text"],
input[type="email"],
input[type="tel"],
input[type="password"],
input[type="date"],
select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 1rem;
  background-color: white;
  color: #333;
  box-sizing: border-box;
}

input:focus,
select:focus {
  outline: none;
  border-color: #00960c;
  box-shadow: 0 0 0 2px rgba(0, 150, 12, 0.1);
}

/* Rol dropdown specifieke styling */
.role-select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='%23333' viewBox='0 0 16 16'%3E%3Cpath d='M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  background-size: 16px 12px;
  padding-right: 2.5rem;
}

/* Hulpteksten */
.help-text {
  font-size: 0.85rem;
  color: #666;
  margin-top: 0.5rem;
  font-style: italic;
  width: 100%;
}

/* Foutmelding */
.error-message {
  background-color: #f8d7da;
  color: #721c24;
  padding: 1rem;
  border-radius: 4px;
  border: 1px solid #f5c6cb;
  margin-top: 1rem;
  width: 100%;
  box-sizing: border-box;
}

/* Formulier acties */
.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #eee;
  width: 100%;
}

.btn-primary {
  padding: 0.75rem 2rem;
  background-color: #00960c;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s;
  min-width: 150px;
}

.btn-primary:hover:not(:disabled) {
  background-color: #007a0a;
}

.btn-primary:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.btn-secondary {
  padding: 0.75rem 2rem;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s;
  min-width: 150px;
}

.btn-secondary:hover {
  background-color: #545b62;
}

/* Vereiste velden indicatie */
label[for]:has(+ input[required])::after,
label[for]:has(+ select[required])::after {
  content: " *";
  color: #dc3545;
}

/* Volledige breedte voor belangrijke velden */
.form-group.full-width {
  width: 100%;
}

.form-group.full-width input,
.form-group.full-width select {
  width: 100%;
}

/* Compactere layout opties */
.form-group.compact {
  margin-bottom: 1rem;
}

.form-group.compact label {
  font-size: 0.9rem;
  margin-bottom: 0.25rem;
}

.form-group.compact input,
.form-group.compact select {
  padding: 0.5rem 0.75rem;
  font-size: 0.95rem;
}

input[type="password"] {
  position: relative;
}

label[for="password"]::after,
label[for="confirmPassword"]::after {
  content: " *";
  color: #dc3545;
  margin-left: 2px;
}

.help-text {
  font-size: 0.85rem;
  color: #666;
  margin-top: 0.5rem;
  font-style: italic;
  width: 100%;
}

.password-strength {
  height: 4px;
  background-color: #e9ecef;
  margin-top: 4px;
  border-radius: 2px;
  overflow: hidden;
}

.password-strength-bar {
  height: 100%;
  transition: width 0.3s, background-color 0.3s;
}

.password-strength.weak .password-strength-bar {
  width: 33%;
  background-color: #dc3545;
}

.password-strength.medium .password-strength-bar {
  width: 66%;
  background-color: #ffc107;
}

.password-strength.strong .password-strength-bar {
  width: 100%;
  background-color: #28a745;
}
</style>