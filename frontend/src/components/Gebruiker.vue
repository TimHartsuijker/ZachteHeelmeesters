<template>
  <div class="user-row">
    <span class="user-field"><strong>Naam:</strong> {{ name }}</span>
    <span class="user-field"><strong>Email:</strong> {{ email }}</span>
    <span class="user-field">
      <label for="role-select"><strong>Rol:</strong></label>
      <select v-model="selectedRole" class="role-select" id="role-select">
        <option value="patiënt">Patiënt</option>
        <option value="huisarts">Huisarts</option>
        <option value="specialist">Specialist</option>
        <option value="admin">Admin</option>
      </select>
    </span>
    <span class="user-field">
      <strong>SB:</strong>
      <input type="checkbox" v-model="permissionSB" style="margin-left:0.5rem; accent-color:#B0DB9C;" />
    </span>
    <button class="save-btn" @click="saveUser">Save</button>
  </div>
</template>

<script>
export default {
  name: "Gebruiker",
  props: {
    name: { type: String, required: true },
    email: { type: String, required: true },
    role: { type: String, required: true },
    extraPermissionSB: { type: Boolean, required: true }
  },
  data() {
    return {
      selectedRole: this.mapRole(this.role),
      permissionSB: this.extraPermissionSB
    }
  },
  methods: {
    mapRole(role) {
      // Accept both backend and display values for compatibility
      switch (role.toLowerCase()) {
        case "patiënt":
        case "patient":
          return "patiënt";
        case "huisarts":
          return "huisarts";
        case "specialist":
          return "specialist";
        case "admin":
          return "admin";
        default:
          return "patiënt";
      }
    },
    saveUser() {
      // Emit updated values to parent or handle API call here
      this.$emit('update-user', {
        name: this.name,
        email: this.email,
        role: this.selectedRole,
        extraPermissionSB: this.permissionSB
      });
    }
  },
  watch: {
    extraPermissionSB(newVal) {
      this.permissionSB = newVal;
    }
  },
  mounted() {
    console.log('Gebruiker.vue mounted');
  }
}
</script>


<style scoped>
.user-row {
  display: flex;
  gap: 2rem;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem 2.5rem;
  background: #ECFAE5;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  font-size: 1.1rem;
  width: 95%;
  max-width: none;
  margin: 1.5rem auto 0 auto;
}
.user-field {
  white-space: nowrap;
}
.save-btn {
  margin-left: auto;
  padding: 0.5rem 1.5rem;
  background: #B0DB9C;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.2s;
}
.save-btn:hover {
  background: #8FC97A;
}
.role-select {
  margin-left: 0.5rem;
  padding: 0.2rem 0.7rem;
  border-radius: 4px;
  border: 1px solid #B0DB9C;
  background: #fff;
  font-size: 1rem;
}
</style>