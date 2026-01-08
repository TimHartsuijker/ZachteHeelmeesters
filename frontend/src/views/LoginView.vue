<template>
  <div class="login-view-page">
    <div class="login-view-card">

      <h2>Inloggen</h2>
      <form @submit.prevent="login">

        <div class="login-view-form-group">
          <label for="email">Email:</label>
          <input type="text" id="email" v-model="email" />
        </div>

        <div class="login-view-form-group">
          <label for="wachtwoord">Wachtwoord:</label>
          <input type="password" id="wachtwoord" v-model="wachtwoord" />
        </div>

        <!-- Foutmelding bij lege velden -->
        <p v-if="emptyError" id="empty-input-error" class="error-text">
          Gegevens moeten ingevuld zijn
        </p>

        <!-- Foutmelding bij incorrecte login -->
        <p v-if="loginError" id="login-error" class="error-text">
          {{ loginErrorMessage }}
        </p>

        <!-- Succesmelding -->
        <p v-if="loginSuccess" class="success-text">
          {{ loginSuccess }}
        </p>

        <button type="submit" id="login-btn">Login</button>
        <div class="admin-login-link">
  <button id="admin-login-link" @click="goToAdminLogin">
    Inloggen als beheerder
  </button>
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
      email: "",
      wachtwoord: "",
      emptyError: false,
      loginError: false,
      loginErrorMessage: "",
      loginSuccess: "",
    };
  },

  methods: {
    // 🔹 User login
    async login() {
      // Reset meldingen
      this.emptyError = false;
      this.loginError = false;
      this.loginErrorMessage = "";
      this.loginSuccess = "";

      // Check op lege velden
      if (!this.email || !this.wachtwoord) {
        this.emptyError = true;
        return;
      }

      try {
        const response = await axios.post("https://localhost:7240/api/login", {
          email: this.email,
          wachtwoord: this.wachtwoord,
        });

        if (response.status === 200) {
          console.log("Login gelukt!", response.data);

          // Sessie opslaan
          sessionStorage.setItem("isLoggedIn", "true");

          // Redirect naar user dashboard
          this.$router.push("/dashboard");
        }
      } catch (error) {
        // Foutmelding van backend (401/400)
        if (error.response && error.response.data && error.response.data.message) {
          this.loginErrorMessage = error.response.data.message;
        } else {
          this.loginErrorMessage = "Er is iets misgegaan bij het inloggen.";
        }

        this.loginError = true;
        console.log("Fout bij inloggen:", this.loginErrorMessage);
      }
    },

    // 🔹 Admin login knop (SPA navigatie, geen refresh)
    goToAdminLogin() {
      this.$router.push("/admin/login");
    }
  },
};
</script>

