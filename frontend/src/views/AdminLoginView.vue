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
      </form>

    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

const router = useRouter();

// State (Vervangt data())
const email = ref("");
const wachtwoord = ref("");
const loginError = ref(false);
const loginErrorMessage = ref("");

async function loginAdmin() {

  // Check lege velden
  if (!email.value || !wachtwoord.value) {
    loginError.value = true;
    loginErrorMessage.value = "Gegevens moeten ingevuld zijn";
    return;
  }

  try {
    const response = await axios.post(
      "/api/login/admin",
      {
        email: email.value,
        wachtwoord: wachtwoord.value,
      }
    );

    if (response.status === 200) {
      const userData = response.data.user;
      console.log("Login gelukt!", response.data);

      // Opslaan sessie
      sessionStorage.setItem("isAdminLoggedIn", "true");
      sessionStorage.setItem("userId", userData.id);
      sessionStorage.setItem("userEmail", userData.email);
      sessionStorage.setItem("userName", `${userData.firstName} ${userData.lastName}`);
      sessionStorage.setItem("userRole", userData.role);

      // Redirect naar admin dashboard
      router.push("/admin/dashboard");
    }
  } catch (error) {
    console.log(error);
    loginError.value = true;
    loginErrorMessage.value =
      (error.response && error.response.data.message) || "Admin login mislukt";
  }
};
</script>
