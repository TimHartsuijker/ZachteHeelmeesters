<template>
  <div class="login-view-page admin-login-page">
    <div class="login-view-card">

      <h2>Admin login</h2>

      <form @submit.prevent="loginAdmin">
        <div class="login-view-form-group">
          <label for="admin-username">Email:</label>
          <input type="text" id="admin-username" v-model="email" />
        </div>

        <div class="login-view-form-group">
          <label for="admin-password">Wachtwoord:</label>
          <input type="password" id="admin-password" v-model="wachtwoord" />
        </div>

        <!-- Foutmelding -->
        <p v-if="loginError" id="error-text" class="error-text">{{ loginErrorMessage }}</p>

        <button type="submit" id="admin-login-btn">
          Login als beheerder
        </button>
        <div class="login-link">
          <p> <button type="button" @click="goToLogin">terug</button></p>
        </div>
      </form>

    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "AdminLoginView",
  data() {
    return {
      email: "",
      wachtwoord: "",
      loginError: false,
      loginErrorMessage: "",
    };
  },
  methods: {
    async loginAdmin() {
      // Reset foutmeldingen
      this.loginError = false;
      this.loginErrorMessage = "";

      // Check lege velden
      if (!this.email || !this.wachtwoord) {
        this.loginError = true;
        this.loginErrorMessage = "Gegevens moeten ingevuld zijn";
        return;
      }

      try {
        const response = await axios.post(
          "https://localhost:7240/api/login/admin",
          {
            email: this.email,
            wachtwoord: this.wachtwoord,
          }
        );

        if (response.status === 200) {
          // Opslaan sessie
          sessionStorage.setItem("isAdminLoggedIn", "true");

          // Redirect naar admin dashboard
          this.$router.push("/admin/dashboard");
        }
      } catch (error) {
        this.loginError = true;
        this.loginErrorMessage =
          (error.response && error.response.data.message) || "Admin login mislukt";
      }
      
    },
    goToLogin() {
      this.$router.push("/");
    }
  },
};
</script>
