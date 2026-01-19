<template>
  <div class="register-view-page">
    <div class="register-view-card">

      <h2>Registreren</h2>

      <form @submit.prevent="register">

        <!-- Voornaam -->
        <div class="register-view-form-group">
          <label for="firstname">Voornaam:</label>
          <input type="text" id="firstname" v-model="firstName" placeholder="Pink" />
        </div>

        <!-- Achternaam -->
        <div class="register-view-form-group">
          <label for="lastname">Achternaam:</label>
          <input type="text" id="lastname" v-model="lastName" placeholder="Eltje" />
        </div>

        <!-- Straatnaam -->
        <div class="register-view-form-group">
          <label for="street">Straatnaam:</label>
          <input type="text" id="street" v-model="streetName" placeholder="Dicklaanstraat" />
        </div>

        <!-- Huisnummer -->
        <div class="register-view-form-group">
          <label for="houseNumber">Huisnummer:</label>
          <input type="text" id="houseNumber" v-model="houseNumber" placeholder="73A" />
        </div>

        <!-- Postcode -->
        <div class="register-view-form-group">
          <label for="postalCode">Postcode:</label>
          <input type="text" minlength="6" maxlength="6" id="postalCode" v-model="postalCode" placeholder="1939AD"  />
        </div>

        <!-- BSN -->
        <div class="register-view-form-group">
          <label for="csn">Burgerservicenummer:</label>
          <input 
            type="text"
            inputmode="numeric"
            minlength="9"
            maxlength="9"
            placeholder="018941218" 
            id="csn" 
            v-model="citizenServiceNumber" 
            @input="citizenServiceNumber = citizenServiceNumber.replace(/\D/g, '')"
          />
        </div>

        <!-- Geboortedatum -->
        <div class="register-view-form-group">
          <label for="dob">Geboortedatum:</label>
          <input type="date" id="dob" v-model="dateOfBirth" />
        </div>

        <!-- Geslacht -->
        <div class="register-view-form-group">
          <label for="gender">Geslacht:</label>
          <select id="gender" v-model="gender">
            <option value="">Selecteer geslacht</option>
            <option value="Man">Man</option>
            <option value="Vrouw">Vrouw</option>
            <option value="Anders">Anders</option>
          </select>
        </div>

        <!-- Telefoon -->
        <div class="register-view-form-group">
          <label for="phone">Telefoonnummer:</label>
          <input type="text" 
            minlength="10" 
            maxlength="10" 
            id="phone" 
            v-model="phoneNumber" 
            placeholder="0630569461" 
            @input="phoneNumber = phoneNumber.replace(/\D/g, '')"
          />
        </div>

        <!-- Huisarts -->
        <div v-if="loading || !doctors">Huisartsen aan het ophalen...</div>
        <div v-else class="register-view-form-group">
          <label for="doctor">Huisarts:</label>
          <select id="doctor" v-model="doctor">
            <option value="">Selecteer huisarts</option>
            <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
              Dr. {{ doctor.firstName }} {{ doctor.lastName }}
            </option>
          </select>
        </div>

        <!-- Email -->
        <div class="register-view-form-group">
          <label for="email">Email:</label>
          <input type="email" id="email" v-model="email" placeholder="pink.eltje@gmail.com"/>
        </div>

        <!-- Wachtwoord -->
        <div class="register-view-form-group">
          <label for="password">Wachtwoord:</label>
          <input type="password" id="password" v-model="password" />
        </div>

        <!-- Foutmeldingen -->
        <p v-if="emptyError" class="error-text">Alle velden moeten ingevuld zijn.</p>
        <p v-if="registerError" class="error-text">{{ registerErrorMessage }}</p>
        <p v-if="registerSuccess" class="success-text">{{ registerSuccess }}</p>

        <button type="submit" :disabled="isSubmitting">
          {{ isSubmitting ? 'Bezig met registreren...' : 'Registreren' }}
        </button>

        <div class="login-link">
          <p>Heb je al een account? <button type="button" @click="goToLogin">Inloggen</button></p>
        </div>

      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

// Router initialiseren voor navigatie
const router = useRouter();

const loading = ref(true);

// Formulier velden
const firstName = ref("");
const lastName = ref("");
const streetName = ref("");
const houseNumber = ref("");
const postalCode = ref("");
const citizenServiceNumber = ref("");
const dateOfBirth = ref("");
const gender = ref("");
const phoneNumber = ref("");
const doctor = ref("");
const email = ref("");
const password = ref("");

// Status meldingen
const emptyError = ref(false);
const registerError = ref(false);
const registerErrorMessage = ref("");
const registerSuccess = ref("");
const isSubmitting = ref(false);

const doctors = ref([])
const fetchDoctors = async () => {
  try {
    // Axios haalt de data op en parseert deze automatisch naar JSON
    const response = await axios.get('/api/doctors');
    
    // Bij Axios gebruik je response.data in plaats van response.json()
    doctors.value = response.data;
    
    console.log('Doctors fetched:', doctors.value);
  } catch (error) {
    console.error('Error fetching doctors:', error);
    
    // Foutafhandeling
    doctors.value = [
      { id: null, firstName: 'Failed to fetch', lastName: 'doctors' }
    ];
  } finally {
    loading.value = false;
  }
}

// Registratie functie
const register = async () => {
  // Reset meldingen
  emptyError.value = false;
  registerError.value = false;
  registerErrorMessage.value = "";
  registerSuccess.value = "";

  // Check op lege velden
  if (!firstName.value || !lastName.value || !streetName.value || !houseNumber.value ||
      !postalCode.value || !citizenServiceNumber.value || !dateOfBirth.value || !gender.value ||
      !phoneNumber.value || !doctor.value || !email.value || !password.value) {
    emptyError.value = true;
    return;
  }

  isSubmitting.value = true;

  try {
    // GEBRUIK HIER EEN RELATIEVE URL:
    // Dus GEEN https://localhost:7240. De browser stuurt dit nu naar de Nginx proxy (poort 80)
    const response = await axios.post("/api/register", {
      firstName: firstName.value,
      lastName: lastName.value,
      streetName: streetName.value,
      houseNumber: houseNumber.value,
      postalCode: postalCode.value,
      citizenServiceNumber: citizenServiceNumber.value,
      dateOfBirth: dateOfBirth.value,
      gender: gender.value,
      phoneNumber: phoneNumber.value,
      doctorId: doctor.value,
      email: email.value,
      password: password.value
    });

    if (response.status === 201 || response.status === 200) {
      registerSuccess.value = "Registratie gelukt! Je wordt nu doorgestuurd...";

      setTimeout(() => {
        router.push("/");
      }, 1500);
    }
  } catch (error) {
    isSubmitting.value = false;
    // Verbeterde error logging voor debugging
    console.error("Registratie fout:", error.response || error);
    
    if (error.response?.data?.message) {
      registerErrorMessage.value = error.response.data.message;
    } else {
      registerErrorMessage.value = "Er is iets misgegaan bij registreren.";
    }
    registerError.value = true;
  }
};

onMounted(() => {
  console.log('App.vue mounted')
  fetchDoctors()
});

// Navigatie naar login
const goToLogin = () => {
  router.push("/");
};
</script>
