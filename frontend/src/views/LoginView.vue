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
        <p v-if="emptyError" id="empty-input-error" class="error-text">
          Gegevens moeten ingevuld zijn
        </p>
        <p v-if="loginError" id="login-error" class="error-text">
          inloggegevens zijn incorrect
        </p>
        <button type="submit" id="login-btn">Login</button>
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
    };
  },

  methods: {
    async login() {
      // Reset foutmeldingen
      this.emptyError = false;
      this.loginError = false;

      // Check voor lege velden
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
          // Optioneel: redirect naar dashboard
          // window.location.href = "/dashboard";
          console.log("Login gelukt!");
        }
      } catch (error) {
        // Backend geeft 401 of 400 → foutmelding tonen
        this.loginError = true;
        console.log("Fout bij inloggen:", error.response?.data?.message);
      }
    },
  },
};
</script>

