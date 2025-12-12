<template>
  <div class="user-row">
    <span class="user-field"><strong>Naam:</strong> {{ name }}</span>
    <span class="user-field"><strong>Email:</strong> {{ email }}</span>
    <span class="user-field">
      <label for="role-select"><strong>Rol:</strong></label>
      <select v-model="selectedRole" class="role-select" id="role-select">
        <option v-for="rol in rollen" :key="rol.rolID" :value="rol.rolnaam">
          {{ rol.rolnaam.charAt(0).toUpperCase() + rol.rolnaam.slice(1) }}
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
    role: { type: String, required: true },
    roleId: { type: Number, required: true }
  },
  data() {
    return {
      selectedRole: this.role,
      selectedRoleId: this.roleId,
      originalRoleId: this.roleId,
      rollen: [],
      saving: false
    }
  },
  methods: {
    async fetchRollen() {
      try {
        const response = await fetch('http://localhost:5016/api/rollen');
        if (!response.ok) throw new Error('Failed to fetch roles');
        this.rollen = await response.json();
      } catch (error) {
        console.error('Error fetching roles:', error);
        // Fallback to default roles if API fails
        this.rollen = [
          { rolID: 1, rolnaam: 'patiënt' },
          { rolID: 2, rolnaam: 'huisarts' },
          { rolID: 3, rolnaam: 'specialist' },
          { rolID: 4, rolnaam: 'admin' }
        ];
      }
    },
    async saveUser() {
      // Check if any changes were made
      if (this.selectedRoleId === this.originalRoleId) {
        alert('Geen wijzigingen om op te slaan.');
        return;
      }

      this.saving = true;
      try {
        const response = await fetch(`http://localhost:5016/api/gebruikers/${this.userId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            rol: this.selectedRoleId
          })
        });

        if (!response.ok) throw new Error('Failed to update user');
        
        // Update original values after successful save
        this.originalRoleId = this.selectedRoleId;
        
        alert('Wijzigingen zijn opgeslagen.');
      } catch (error) {
        console.error('Error updating user:', error);
        alert('Failed to update user: ' + error.message);
      } finally {
        this.saving = false;
      }
    },
    onRoleChange() {
      const selected = this.rollen.find(r => r.rolnaam === this.selectedRole);
      if (selected) {
        this.selectedRoleId = selected.rolID;
      }
    }
  },
  watch: {
    selectedRole() {
      this.onRoleChange();
    }
  },
  async mounted() {
    console.log('Gebruiker.vue mounted');
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
.sb-label {
  white-space: nowrap;
}
.sb-checkbox {
  margin-left: 0.5rem;
  accent-color: #B0DB9C;
  cursor: pointer;
  width: 1.2rem;
  height: 1.2rem;
}
.sb-checkbox:focus {
  outline: 2px solid #8FC97A;
  outline-offset: 2px;
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