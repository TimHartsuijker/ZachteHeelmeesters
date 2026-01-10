<template>
  <div class="filter-container" role="region" aria-label="Gebruikers filters">
    <fieldset class="filter-fieldset">
      <legend class="filter-legend">Filters</legend>
      
      <div class="filter-group">
        <label for="search"><strong>Zoeken:</strong></label>
        <input
          id="search"
          v-model="searchQuery"
          type="text"
          placeholder="Zoek op naam of email..."
          class="search-input"
          aria-label="Zoeken naar gebruiker op naam of email"
        />
      </div>

      <div class="filter-group">
        <label for="role-filter"><strong>Rol:</strong></label>
        <select v-model="selectedRole" id="role-filter" class="filter-select" aria-label="Filteren op rol">
          <option value="">Alle rollen</option>
          <option v-for="rol in rollen" :key="rol.id" :value="rol.roleName">
            {{ rol && rol.roleName ? rol.roleName.charAt(0).toUpperCase() + rol.roleName.slice(1) : 'Laden...' }}
          </option>
        </select>
      </div>
    </fieldset>

    <button v-if="hasActiveFilters" @click="clearFilters" class="clear-btn" aria-label="Alle filters wissen">
      Filters wissen
    </button>
  </div>
</template>

<script>
export default {
  name: "Filter",
  props: {
    rollen: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      searchQuery: "",
      selectedRole: ""
    };
  },
  computed: {
    hasActiveFilters() {
      return this.searchQuery !== "" || this.selectedRole !== "";
    }
  },
  watch: {
    searchQuery() {
      this.emitFilters();
    },
    selectedRole() {
      this.emitFilters();
    }
  },
  methods: {
    emitFilters() {
      this.$emit("filter-change", {
        search: this.searchQuery,
        role: this.selectedRole
      });
    },
    clearFilters() {
      this.searchQuery = "";
      this.selectedRole = "";
    }
  }
};
</script>

<style scoped>
.filter-container {
  display: flex;
  gap: 1.5rem;
  align-items: center;
  padding: 1.5rem 2.5rem;
  background: #ECFAE5;
  border-radius: 8px;
  margin: 1.5rem auto 0 auto;
  width: 95%;
  max-width: none;
  flex-wrap: wrap;
}

.filter-fieldset {
  border: none;
  padding: 0;
  margin: 0;
  display: flex;
  gap: 1.5rem;
  align-items: center;
  flex-wrap: wrap;
  flex: 1;
}

.filter-legend {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border-width: 0;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
  min-width: 200px;
}

.filter-group label {
  white-space: nowrap;
  font-size: 1rem;
}

.search-input,
.filter-select {
  padding: 0.5rem 0.7rem;
  border-radius: 4px;
  border: 1px solid #B0DB9C;
  background: #fff;
  font-size: 1rem;
  flex: 1;
  min-width: 150px;
}

.search-input:focus,
.filter-select:focus {
  outline: none;
  border-color: #8FC97A;
  box-shadow: 0 0 4px rgba(176, 219, 156, 0.3);
}

.clear-btn {
  padding: 0.5rem 1rem;
  background: #8FC97A;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: background 0.2s;
  white-space: nowrap;
}

.clear-btn:hover {
  background: #7ab566;
}
</style>
