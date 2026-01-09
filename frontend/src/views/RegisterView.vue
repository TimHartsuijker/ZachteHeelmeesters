<template>
  <div class="register-view-page">
    <div class="register-view-card">

      <h2>Registreren</h2>

      <form @submit.prevent="register">

        <!-- Voornaam -->
        <div class="register-view-form-group">
          <label for="firstname">Voornaam:</label>
          <input type="text" id="firstname" v-model="firstName" />
        </div>

        <!-- Achternaam -->
        <div class="register-view-form-group">
          <label for="lastname">Achternaam:</label>
          <input type="text" id="lastname" v-model="lastName" />
        </div>

        <!-- Straatnaam -->
        <div class="register-view-form-group">
          <label for="street">Straatnaam:</label>
          <input type="text" id="street" v-model="streetName" />
        </div>

        <!-- Huisnummer -->
        <div class="register-view-form-group">
          <label for="houseNumber">Huisnummer:</label>
          <input type="text" id="houseNumber" v-model="houseNumber" />
        </div>

        <!-- Postcode -->
        <div class="register-view-form-group">
          <label for="postalCode">Postcode:</label>
          <input type="text" id="postalCode" v-model="postalCode" />
        </div>

        <!-- BSN -->
        <div class="register-view-form-group">
          <label for="csn">Burgerservicenummer:</label>
          <input type="text" id="csn" v-model="citizenServiceNumber" />
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
          <input type="text" id="phone" v-model="phoneNumber" />
        </div>

        <!-- Email -->
        <div class="register-view-form-group">
          <label for="email">Email:</label>
          <input type="text" id="email" v-model="email" />
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

        <button type="submit">Registreren</button>

        <div class="login-link">
          <p>Heb je al een account? <button type="button" @click="goToLogin">Inloggen</button></p>
        </div>

      </form>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      firstName: "",
      lastName: "",
      streetName: "",
      houseNumber: "",
      postalCode: "",
      citizenServiceNumber: "",
      dateOfBirth: "",
      gender: "",
      phoneNumber: "",
      email: "",
      password: "",
      emptyError: false,
      registerError: false,
      registerErrorMessage: "",
      registerSuccess: "",
    };
  },
  methods: {
    async register() {
      // Reset meldingen
      this.emptyError = false;
      this.registerError = false;
      this.registerErrorMessage = "";
      this.registerSuccess = "";

      // Check op lege velden
      if (!this.firstName || !this.lastName || !this.streetName || !this.houseNumber ||
          !this.postalCode || !this.citizenServiceNumber || !this.dateOfBirth || !this.gender ||
          !this.phoneNumber || !this.email || !this.password) {
        this.emptyError = true;
        return;
      }

      try {
        const response = await axios.post("https://localhost:7240/api/register", {
          firstName: this.firstName,
          lastName: this.lastName,
          streetName: this.streetName,
          houseNumber: this.houseNumber,
          postalCode: this.postalCode,
          citizenServiceNumber: this.citizenServiceNumber,
          dateOfBirth: this.dateOfBirth,
          gender: this.gender,
          phoneNumber: this.phoneNumber,
          email: this.email,
          password: this.password
        });

        if (response.status === 201) {
          this.registerSuccess = "Registratie gelukt! Je wordt doorgestuurd naar de login.";
          setTimeout(() => this.$router.push("/"), 1500);
        }
      } catch (error) {
        if (error.response && error.response.data && error.response.data.message) {
          this.registerErrorMessage = error.response.data.message;
        } else {
          this.registerErrorMessage = "Er is iets misgegaan bij registreren.";
        }
        this.registerError = true;
      }
    },

    goToLogin() {
      this.$router.push("/");
    }
  }
};
</script>
