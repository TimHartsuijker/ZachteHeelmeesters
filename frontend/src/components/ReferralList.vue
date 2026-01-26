<template>
  <div class="referral-grid">
    <div v-if="referrals.length === 0" class="no-data">
      Geen actieve doorverwijzingen gevonden.
    </div>

    <div 
      v-else 
      v-for="referral in referrals.slice(0, limit)" 
      :key="referral.id" 
      class="referral-card"
    >
      <div class="card-content">
        <div class="card-header">
          <span class="ref-badge">{{ referral.code }}</span>
          <span class="ref-date">{{ formatDate(referral.createdAt) }}</span>
        </div>
        
        <div class="card-body">
          <h3 class="treatment-title">{{ referral.treatmentDescription }}</h3>
          <p class="doctor-name">
            <span class="icon">👤</span> {{ referral.doctorName || 'Huisarts onbekend' }}
          </p>
        </div>
      </div>
      <div class="card-accent"></div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  referrals: { type: Array, required: true },
  limit: { type: Number, default: 3 }
});

const formatDate = (dateString) => {
  if (!dateString) return '-';
  return new Date(dateString).toLocaleDateString('nl-NL', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  });
};
</script>

<style scoped>
.referral-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin: 1rem 0;
}

.referral-card {
  position: relative;
  background: #ffffff;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  transition: all 0.2s ease-in-out;
  overflow: hidden;
  display: flex;
}

/* De kleine groene lijn aan de linkerkant voor herkenbaarheid */
.card-accent {
  width: 4px;
  background-color: #B0DB9C;
  order: -1;
}

.card-content {
  padding: 1rem;
  flex-grow: 1;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.ref-badge {
  background: #f0fdf4;
  color: #166534;
  font-family: monospace;
  font-weight: bold;
  font-size: 0.75rem;
  padding: 2px 8px;
  border-radius: 4px;
  border: 1px solid #bbf7d0;
}

.ref-date {
  font-size: 0.75rem;
  color: #64748b;
}

.treatment-title {
  margin: 0;
  font-size: 1rem;
  color: #1e293b;
  font-weight: 600;
}

.doctor-name {
  margin: 0.25rem 0 0 0;
  font-size: 0.85rem;
  color: #64748b;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.no-data {
  text-align: center;
  color: #94a3b8;
  font-style: italic;
  padding: 2rem 0;
}
</style>