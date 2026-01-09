<template>
  <div class="user-row">
    <span class="user-field"><strong>Naam:</strong> {{ name }}</span>
    <span class="user-field"><strong>Email:</strong> {{ email }}</span>
    <span class="user-field">
      <label for="role-select"><strong>Rol:</strong></label>
      <select v-model="selectedRoleId" class="role-select" id="role-select">
        <option v-for="rol in rollen" :key="rol.id" :value="rol.id">
          {{ rol.roleName.charAt(0).toUpperCase() + rol.roleName.slice(1) }}
        </option>
      </select>
    </span>
    <button class="save-btn" @click="saveUser" :disabled="saving">
      {{ saving ? 'Saving...' : 'Save' }}
    </button>
  </div>
</template>

<script>
export default {
  name: "Gebruiker",
  props: {
    userId: { type: Number, required: true },
    name: { type: String, required: true },
    email: { type: String, required: true },
    roleId: { type: Number, required: true } // we gebruiken rolID, niet rolnaam
  },
  data() {
    return {
      rollen: [],
      selectedRoleId: this.roleId,
      originalRoleId: this.roleId,
      saving: false
    }
  },
  methods: {
    async fetchRollen() {
      try {
        const response = await fetch('https://localhost:7240/api/roles');
        if (!response.ok) throw new Error('Failed to fetch roles');
        this.rollen = await response.json();
      } catch (error) {
        console.error('Error fetching roles:', error);
        // fallback indien backend niet werkt
        this.rollen = [
          { id: 1, roleName: 'Patiënt' },
          { id: 2, roleName: 'Huisarts' },
          { id: 3, roleName: 'Specialist' },
          { id: 4, roleName: 'Administrator' }
        ];
      }
    },
    async saveUser() {
      if (this.selectedRoleId === this.originalRoleId) {
        alert('Geen wijzigingen om op te slaan.');
        return;
      }

      this.saving = true;
      try {
        const response = await fetch(`https://localhost:7240/api/users/${this.userId}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ roleId: Number(this.selectedRoleId) })
        });

        if (!response.ok) throw new Error('Failed to update user');

        this.originalRoleId = this.selectedRoleId;
        alert('Wijzigingen zijn opgeslagen.');
      } catch (error) {
        console.error('Error updating user:', error);
        alert('Failed to update user: ' + error.message);
      } finally {
        this.saving = false;
      }
    }
  },
  async mounted() {
    await this.fetchRollen();
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
.save-btn:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}
.save-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
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
