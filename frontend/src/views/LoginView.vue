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
        <button type="submit">Login</button>
      </form>
      
    </div>
  </div>
</template>
