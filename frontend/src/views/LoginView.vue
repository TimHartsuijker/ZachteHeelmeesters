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
            <RouterLink id="admin-login-link" class="admin-login-link" to="/admin/login" aria-current="admin-login link">Inloggen als beheerder</RouterLink>
        </div>

        <div class="register-link">
          <RouterLink id="go-to-register" to="/register">Nog geen account? Registreer hier</RouterLink>
        </div>  

      </form>

    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

// Router instance
const router = useRouter();

// State (Vervangt data())
const email = ref("");
const wachtwoord = ref("");
const emptyError = ref(false);
const loginError = ref(false);
const loginErrorMessage = ref("");
const loginSuccess = ref("");

// User login (Vervangt methods)
const login = async () => {
  // Reset meldingen
  emptyError.value = false;
  loginError.value = false;
  loginErrorMessage.value = "";
  loginSuccess.value = "";

  // Check op lege velden
  if (!email.value || !wachtwoord.value) {
    emptyError.value = true;
    return;
  }

  try {
    const response = await axios.post("/api/login", {
      email: email.value,
      wachtwoord: wachtwoord.value,
    }, {
      // DIT IS DE BELANGRIJKE TOEVOEGING:
      withCredentials: true
    });

    if (response.status === 200) {
      const userData = response.data.user;
      console.log("Login gelukt!", response.data);

      // Sessie opslaan
      sessionStorage.setItem("isLoggedIn", "true");
      if (userData?.id) {
        sessionStorage.setItem("userId", String(userData.id));
      }
      if (userData?.role) {
        sessionStorage.setItem("userRole", userData.role);
      }

      // ✅ CORRECTE REDIRECT OP BASIS VAN ROL
      switch (userData?.role) {
        case "Specialist":
          router.push("/patienten");
          break;
        case "Huisarts":
          router.push("/patienten");
          break;
        case "Administratiemedewerker":
          router.push("/administratie/accounts");
          break;
        case "Admin":
          router.push("/admin/dashboard");
          break;
        default: // Patiënt
          router.push("/dashboard");
          break;
      }
    }
  } catch (error) {
    // Als de error is "Gebruik de admin login pagina", probeer dan admin login
    if (error.response?.status === 401 && 
        error.response?.data?.message === "Gebruik de admin login pagina") {
      
      // Probeer via admin login (voor het geval iemand per ongeluk hier probeert)
      try {
        const adminResponse = await axios.post("/api/login/admin", {
          email: email.value,
          wachtwoord: wachtwoord.value,
        }, {
          withCredentials: true
        });

        if (adminResponse.status === 200) {
          const userData = adminResponse.data.user;
          console.log("Admin login gelukt!", userData);

          // Sessie opslaan
          sessionStorage.setItem("isLoggedIn", "true");
          if (userData?.id) {
            sessionStorage.setItem("userId", String(userData.id));
          }
          if (userData?.role) {
            sessionStorage.setItem("userRole", userData.role);
          }

          // Redirect voor admin rollen
          if (userData?.role === "Admin") {
            router.push("/admin/dashboard");
          } else if (userData?.role === "Administratiemedewerker") {
            router.push("/admin/create-user");
          }
        }
      } catch (adminError) {
        handleLoginError(adminError);
      }
    } else {
      handleLoginError(error);
    }
  }
};

// Helper functie voor error handling
const handleLoginError = (error) => {
  console.error("Login error details:", {
    message: error.message,
    status: error.response?.status,
    data: error.response?.data,
  });

  if (error.response?.data?.message) {
    loginErrorMessage.value = error.response.data.message;
  } else if (error.code === 'ERR_NETWORK') {
    loginErrorMessage.value = "Verbindingsfout met server. Zorg dat de backend draait";
  } else {
    loginErrorMessage.value = "Er is iets misgegaan bij het inloggen.";
  }

  loginError.value = true;
};
</script>