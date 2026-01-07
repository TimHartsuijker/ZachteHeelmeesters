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
      </form>

    </div>
  </div>
</template>

<script>
import axios from "axios";

axios.defaults.baseURL = "https://localhost:7240"; // centralize if you have many calls

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
    async login() {
      this.emptyError = false;
      this.loginError = false;
      this.loginErrorMessage = "";
      this.loginSuccess = "";

      if (!this.email || !this.wachtwoord) {
        this.emptyError = true;
        return;
      }

      try {
        const response = await axios.post("/api/login", {
          email: this.email,
          wachtwoord: this.wachtwoord,
        });

        if (response.status === 200) {
          sessionStorage.setItem("isLoggedIn", "true");
          // use router.push('/dashboard') if using Vue Router
          window.location.href = "/dashboard";
        }
      } catch (err) {
        const data = err?.response?.data;
        this.loginErrorMessage = data?.message ?? "Er is iets misgegaan bij het inloggen.";
        this.loginError = true;
        console.log("Login error:", this.loginErrorMessage, err);
      }
    },
  },
};
</script>