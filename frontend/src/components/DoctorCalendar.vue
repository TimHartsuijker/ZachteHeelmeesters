<template>
  <div class="doctor-calendar">
    <div class="calendar-header">
      <h2>Agenda</h2>
      <div class="calendar-controls">
        <button @click="previousWeek" class="btn-nav">&lt;</button>
        <button @click="goToToday" class="btn-nav btn-nav-secondary">Vandaag</button>
        <span class="current-week">{{ weekRange }}</span>
        <button @click="nextWeek" class="btn-nav">&gt;</button>
      </div>
    </div>

    <div class="quick-actions">
      <button @click="showUnavailablePeriodModal = true" class="btn btn-primary">
        Periode onbeschikbaar maken
      </button>
      <button @click="showAvailablePeriodModal = true" class="btn btn-secondary">
        Periode beschikbaar maken
      </button>
      <span class="user-info user-info-inline">Gebruiker: {{ userName }} ({{ userEmail }})</span>
    </div>

    <div class="week-view">
      <div class="time-grid">
        <div class="time-labels">
          <div class="time-label"></div>
          <div v-for="hour in hours" :key="hour" class="time-label">
            {{ String(hour).padStart(2, '0') }}:00
          </div>
        </div>

        <div class="days-container">
          <div class="day-headers">
            <div v-for="day in weekDays" :key="day.date" class="day-header">
              <div class="day-name">{{ day.name }}</div>
              <div class="day-date">{{ day.date }}</div>
            </div>
          </div>

          <div class="time-slots-grid">
            <div v-for="day in weekDays" :key="day.date" class="day-column">
              <div
                v-for="slot in getSlots(day.date)"
                :key="`${slot.date}-${slot.hour}-${slot.minute}`"
                :class="['time-slot', { unavailable: isSlotUnavailable(slot.date, slot.hour, slot.minute), 'hour-boundary': isHourBoundary(slot.minute) }]"
                @click="selectSlot(slot.date, slot.hour, slot.minute)"
              >
                <span v-if="isSlotUnavailable(slot.date, slot.hour, slot.minute) && getSlotReason(slot.date, slot.hour, slot.minute)" class="unavailable-reason">
                  {{ getSlotReason(slot.date, slot.hour, slot.minute) }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showUnavailablePeriodModal" class="modal">
      <div class="modal-content">
        <span class="close" @click="showUnavailablePeriodModal = false">&times;</span>
        <h3>Periode onbeschikbaar maken</h3>
        <form @submit.prevent="setUnavailablePeriod">
          <div class="form-group">
            <label for="unavail-start">Start datum/tijd:</label>
            <input id="unavail-start" v-model="unavailablePeriod.startDateTime" type="datetime-local" required />
          </div>
          <div class="form-group">
            <label for="unavail-end">Tot en met:</label>
            <select id="unavail-end" v-model="unavailablePeriod.endDateTime" required>
              <option value="">Selecteer eind tijd</option>
              <option v-for="time in getUnavailablePeriodEndTimes()" :key="time" :value="time">
                {{ formatEndTime(time) }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="unavail-reason">Reden (optioneel):</label>
            <input id="unavail-reason" v-model="unavailablePeriod.reason" type="text" placeholder="bijv. Vakantie, Ziekte, Pauze" />
          </div>
          <button type="submit" class="btn btn-primary">Opslaan</button>
        </form>
      </div>
    </div>

    <div v-if="showAvailablePeriodModal" class="modal">
      <div class="modal-content">
        <span class="close" @click="showAvailablePeriodModal = false">&times;</span>
        <h3>Periode beschikbaar maken</h3>
        <form @submit.prevent="setAvailablePeriod">
          <div class="form-group">
            <label for="avail-start">Start datum/tijd:</label>
            <input id="avail-start" v-model="availablePeriod.startDateTime" type="datetime-local" required />
          </div>
          <div class="form-group">
            <label for="avail-end">Eind datum/tijd:</label>
            <input id="avail-end" v-model="availablePeriod.endDateTime" type="datetime-local" required />
          </div>
          <button type="submit" class="btn btn-primary">Opslaan</button>
        </form>
      </div>
    </div>

    <div v-if="showHourModal && selectedSlot" class="modal">
      <div class="modal-content">
        <span class="close" @click="showHourModal = false">&times;</span>
        <h3>Beschikbaarheid bewerken</h3>
        <p>{{ formatSlotTime(selectedSlot.date, selectedSlot.hour, selectedSlot.minute) }}</p>
        <form @submit.prevent="saveSlot">
          <div class="form-group">
            <label>
              <input v-model="selectedSlot.isAvailable" type="checkbox" />
              Dit moment beschikbaar
            </label>
          </div>
          <div class="form-group" v-if="!selectedSlot.isAvailable">
            <label>Reden (optioneel):</label>
            <input v-model="selectedSlot.reason" type="text" />
          </div>
          <div class="form-group">
            <label>Tot en met:</label>
            <select v-model="selectedSlot.endTime">
              <option v-for="time in getEndTimesForSlot()" :key="time" :value="time">{{ time }}</option>
            </select>
          </div>
          <button type="submit" class="btn btn-primary">Opslaan</button>
          <button type="button" @click="deleteSlot" class="btn btn-danger">Verwijderen</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from "vue";
import axios from "axios";
import { 
  getMondayOfWeek, 
  toIso, 
  getSlots, 
  getTimesFrom, 
  toLocalIsoFromDate,
  formatEndTime,
  formatSlotTime,
  getMaxDate,
  pad 
} from '@/utils/dateHelpers';

// --- Gebruikersinformatie ---
const doctorId = ref(sessionStorage.getItem('userId') || '');
const userName = ref(sessionStorage.getItem('userName') || 'Onbekend');
const userEmail = ref(sessionStorage.getItem('userEmail') || 'onbekend@example.com');

// --- Kalender State ---
const currentDate = ref(new Date());
const hours = ref(Array.from({ length: 24 }, (_, i) => i));
const dayNames = ref(['Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrijdag', 'Zaterdag', 'Zondag']);
const availabilities = ref({}); // Reactief object voor snelle lookup
const loading = ref(false);
const error = ref(null);

// --- Modal & Selectie State ---
const showUnavailablePeriodModal = ref(false);
const showAvailablePeriodModal = ref(false);
const showHourModal = ref(false);
const selectedSlot = ref(null);

// --- Formulier Data ---
const unavailablePeriod = ref({ startDateTime: '', endDateTime: '', reason: '' });
const availablePeriod = ref({ startDateTime: '', endDateTime: '' });

// --- Computed Properties ---
const weekDays = computed(() => {
  const monday = getMondayOfWeek(currentDate.value);
  const days = [];
  for (let i = 0; i < 7; i++) {
    const date = new Date(monday);
    date.setDate(date.getDate() + i);
    days.push({
      date: date.toISOString().split('T')[0],
      name: dayNames.value[i], // .value gebruiken
      fullDate: date
    });
  }
  return days;
});

const weekRange = computed(() => {
  const monday = getMondayOfWeek(currentDate.value);
  const sunday = new Date(monday);
  sunday.setDate(sunday.getDate() + 6);
  const fmt = (d) => `${pad(d.getDate())}-${pad(d.getMonth() + 1)}-${d.getFullYear()}`;
  return `${fmt(monday)} tot ${fmt(sunday)}`;
});

// --- Helper Functies ---
const isHourBoundary = (minute) => minute === 0;
const slotKey = (dateString, hour, minute) => `${dateString}T${pad(hour)}:${pad(minute)}:00`;

// Genereert slots voor de database
const buildSlots = (startIso, endIso, isAvailable, reason = '') => {
  const slots = [];
  let current = new Date(startIso);
  const end = new Date(endIso);
  
  // Afronden op 15 min
  const mins = current.getMinutes();
  const rounded = Math.round(mins / 15) * 15;
  if (rounded !== mins) current.setMinutes(rounded);
  current.setSeconds(0);
  current.setMilliseconds(0);
  
  while (current < end) {
    const y = current.getFullYear();
    const m = pad(current.getMonth() + 1);
    const d = pad(current.getDate());
    const h = current.getHours();
    const min = current.getMinutes();
    const iso = toIso(`${y}-${m}-${d}`, h, min);
    
    slots.push({
      doctorId: doctorId.value, // .value gebruiken
      dateTime: iso,
      isAvailable,
      reason: isAvailable ? '' : reason
    });
    current.setMinutes(current.getMinutes() + 15);
  }
  return slots;
};

// --- Watcher voor formulieren ---
watch(() => unavailablePeriod.value.startDateTime, (val) => {
  if (val) {
    const options = getUnavailablePeriodEndTimes();
    if (options.length > 0) unavailablePeriod.value.endDateTime = options[0];
  }
});

// --- Navigatie Methodes ---
const previousWeek = () => {
  const d = new Date(currentDate.value);
  d.setDate(d.getDate() - 7);
  currentDate.value = d;
  loadAvailabilities();
};

const nextWeek = () => {
  const d = new Date(currentDate.value);
  d.setDate(d.getDate() + 7);
  currentDate.value = d;
  loadAvailabilities();
};

const goToToday = () => {
  currentDate.value = new Date();
  loadAvailabilities();
};

// --- Kalender Lookup Methodes ---
const isSlotUnavailable = (date, hour, min) => availabilities.value[slotKey(date, hour, min)]?.isAvailable === false;

const getSlotReason = (date, hour, min) => {
  const res = availabilities.value[slotKey(date, hour, min)]?.reason || '';
  return res.length > 15 ? res.substring(0, 15) + '...' : res;
};

const selectSlot = (date, hour, min) => {
  const key = slotKey(date, hour, min);
  selectedSlot.value = {
    date, hour, minute: min,
    isAvailable: availabilities.value[key]?.isAvailable !== false,
    reason: availabilities.value[key]?.reason || '',
    endTime: ''
  };
  showHourModal.value = true;
};

// --- Tijd opties voor modals ---
const getEndTimesForSlot = () => selectedSlot.value ? getTimesFrom(selectedSlot.value.hour, selectedSlot.value.minute) : [];
const getUnavailableEndTimes = () => getEndTimesForSlot();

const getUnavailablePeriodEndTimes = () => {
  if (!unavailablePeriod.value.startDateTime) return [];
  const start = new Date(unavailablePeriod.value.startDateTime);
  const times = [];
  let current = new Date(start);
  const limit = getMaxDate(unavailablePeriod.value.startDateTime);
  
  while (true) {
    current.setMinutes(current.getMinutes() + 15);
    const dateStr = current.toISOString().split('T')[0];
    if (dateStr > limit) break;
    times.push(toLocalIsoFromDate(current));
  }
  return times;
};

// --- API Interactie ---
const loadAvailabilities = async () => {
  loading.value = true;
  const monday = getMondayOfWeek(currentDate.value);
  const sunday = new Date(monday);
  sunday.setDate(sunday.getDate() + 7);

  try {
    const { data } = await axios.get(`/api/doctoravailability/${doctorId.value}`, {
      params: { startDate: toLocalIsoFromDate(monday), endDate: toLocalIsoFromDate(sunday) }
    });
    const map = {};
    data.forEach(s => { map[s.dateTime] = { ...s, reason: s.reason || '' }; });
    availabilities.value = map;
  } catch (err) { error.value = err.message; }
  finally { loading.value = false; }
};

const saveSlot = async () => {
  if (!selectedSlot.value) return;
  const startIso = toIso(selectedSlot.value.date, selectedSlot.value.hour, selectedSlot.value.minute);
  let endIso;

  if (selectedSlot.value.endTime) {
    const [h, m] = selectedSlot.value.endTime.split(':').map(Number);
    endIso = toIso(selectedSlot.value.date, h, m);
  } else {
    const t = new Date(startIso);
    t.setMinutes(t.getMinutes() + 15);
    endIso = toLocalIsoFromDate(t);
  }

  const slots = buildSlots(startIso, endIso, selectedSlot.value.isAvailable, selectedSlot.value.reason);
  try {
    await axios.post(`/api/doctoravailability/${doctorId.value}/bulk`, slots);
    showHourModal.value = false;
    loadAvailabilities();
  } catch (err) { alert("Fout bij opslaan: " + err.message); }
};

const deleteSlot = async () => {
  if (!selectedSlot.value) return;
  const iso = toIso(selectedSlot.value.date, selectedSlot.value.hour, selectedSlot.value.minute);
  try {
    await axios.delete(`/api/doctoravailability/${doctorId.value}`, { params: { dateTime: iso } });
    showHourModal.value = false;
    loadAvailabilities();
  } catch (err) { alert("Verwijderen mislukt"); }
};

const setUnavailablePeriod = async () => {
  try {
    if (!unavailablePeriod.value.startDateTime || !unavailablePeriod.value.endDateTime) return;
    const slots = buildSlots(unavailablePeriod.value.startDateTime, unavailablePeriod.value.endDateTime, false, unavailablePeriod.value.reason);
    
    console.log('Onbeschikbare slots:', slots);

    const response = await axios.post(`/api/doctoravailability/${doctorId.value}/bulk`, slots);
    console.log('Server antwoord:', response.data);
    showUnavailablePeriodModal.value = false;
    unavailablePeriod.value = { startDateTime: '', endDateTime: '', reason: '' };
    loadAvailabilities();
  } catch (err) { alert(err.message); }
};

const setAvailablePeriod = async () => {
  try {
    if (!availablePeriod.value.startDateTime || !availablePeriod.value.endDateTime) return;
    const slots = buildSlots(availablePeriod.value.startDateTime, availablePeriod.value.endDateTime, true);
    
    console.log('Beschikbare slots:', slots);
  
    const response = await axios.post(`/api/doctoravailability/${doctorId.value}/bulk`, slots);
    console.log('Server antwoord:', response.data);
    showAvailablePeriodModal.value = false;
    availablePeriod.value = { startDateTime: '', endDateTime: '' };
    loadAvailabilities();
  } catch (err) { alert(err.message); }
};

// --- Lifecycle ---
onMounted(loadAvailabilities);
</script>

<style scoped>
.doctor-calendar {
  padding: 0;
  font-family: Arial, sans-serif;
  background-color: #CAE8BD;
  min-height: 100vh;
  margin-top: 100px;
  width: 100%;
}

.calendar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  background: #ECFAE5;
  padding: 1.5rem 2.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  width: 95%;
  box-sizing: border-box;
  margin-left: auto;
  margin-right: auto;
}

.calendar-header h2 {
  margin: 0;
  color: #333;
  font-size: 1.3rem;
}

.calendar-controls {
  display: flex;
  gap: 15px;
  align-items: center;
}

.user-info {
  font-weight: 600;
  color: #333;
  padding: 0.4rem 0.8rem;
  background: #f2f7ed;
  border: 1px solid #b0db9c;
  border-radius: 6px;
}

.user-info-inline {
  margin-left: auto;
}

.btn-nav {
  padding: 0.5rem 1.5rem;
  background-color: #B0DB9C;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: bold;
  transition: background 0.2s;
}

.btn-nav:hover {
  background-color: #8FC97A;
}

.btn-nav.btn-nav-secondary {
  background-color: #A8D994;
}

.btn-nav.btn-nav-secondary:hover {
  background-color: #8FC97A;
}

.btn-nav:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.current-week {
  min-width: 250px;
  text-align: center;
  font-weight: bold;
  font-size: 1rem;
  color: #333;
}

.quick-actions {
  margin-bottom: 1.5rem;
  display: flex;
  gap: 10px;
  width: 95%;
  box-sizing: border-box;
  margin-left: auto;
  margin-right: auto;
  align-items: center;
}

.btn {
  padding: 0.5rem 1.5rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background 0.2s;
}

.btn-primary {
  background-color: #B0DB9C;
  color: white;
}

.btn-primary:hover {
  background-color: #8FC97A;
}

.btn-primary:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.btn-secondary {
  background-color: #A8D994;
  color: white;
}

.btn-secondary:hover {
  background-color: #8FC97A;
}

.btn-secondary:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}

.btn-danger:focus {
  outline: 2px solid #222;
  outline-offset: 2px;
}

.week-view {
  background-color: #CAE8BD;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  overflow: hidden;
  width: 95%;
  box-sizing: border-box;
  margin-left: auto;
  margin-right: auto;
  margin-bottom: 2rem;
}

.time-grid {
  display: flex;
  height: 100%;
}

.time-labels {
  display: flex;
  flex-direction: column;
  width: 80px;
  border-right: 2px solid #B0DB9C;
  background-color: #F5F9F3;
  flex-shrink: 0;
}

.time-label {
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 12px;
  border-bottom: 1px solid #B0DB9C;
  color: #666;
  flex: 1;
  min-height: 60px;
}

.time-label:first-child {
  border-bottom: 2px solid #B0DB9C;
  font-size: 0;
  min-height: 78px;
}

.days-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.day-headers {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  border-bottom: 2px solid #B0DB9C;
  background-color: #F5F9F3;
  flex-shrink: 0;
}

.day-header {
  padding: 15px 10px;
  text-align: center;
  border-right: 1px solid #B0DB9C;
  min-height: 50px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.day-header:last-child {
  border-right: none;
}

.day-name {
  font-weight: bold;
  color: #333;
  font-size: 14px;
}

.day-date {
  color: #666;
  font-size: 12px;
  margin-top: 5px;
}

.time-slots-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  flex: 1;
  overflow-y: auto;
}

.day-column {
  display: flex;
  flex-direction: column;
  border-right: 1px solid #B0DB9C;
}

.day-column:last-child {
  border-right: none;
}

.time-slot {
  flex: 1;
  border-bottom: 1px solid #D4E8CE;
  background-color: #ffffff;
  cursor: pointer;
  transition: background-color 0.2s;
  min-height: 15px;
}

.time-slot.hour-boundary {
  border-top: 2px solid #B0DB9C;
}

.time-slot:hover {
  background-color: #E8F5E0;
}

.time-slot.unavailable {
  background-color: #FFE8E8;
}

.time-slot.unavailable:hover {
  background-color: #FFD1D1;
}

.unavailable-reason {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  font-size: 0.65rem;
  color: #c41e3a;
  font-weight: bold;
  text-align: center;
  padding: 2px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
}

.modal {
  display: block;
  position: fixed;
  z-index: 1000;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.4);
}

.modal-content {
  background-color: #ECFAE5;
  margin: 10% auto;
  padding: 2.5rem;
  border: none;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

.close {
  color: #666;
  float: right;
  font-size: 28px;
  font-weight: bold;
  cursor: pointer;
}

.close:hover {
  color: #333;
}

.modal-content h3 {
  margin-top: 0;
  color: #333;
  font-size: 1.2rem;
}

.modal-content p {
  color: #666;
  margin: 10px 0;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #333;
  font-size: 1rem;
}

.form-group input[type='text'],
.form-group input[type='datetime-local'],
.form-group select {
  width: 100%;
  padding: 0.5rem 0.7rem;
  border: 1px solid #B0DB9C;
  border-radius: 4px;
  box-sizing: border-box;
  font-size: 1rem;
  background-color: #fff;
}

.form-group input[type='text']:focus,
.form-group input[type='datetime-local']:focus,
.form-group select:focus {
  outline: 2px solid #8FC97A;
  outline-offset: 2px;
}

.form-group input[type='checkbox'] {
  margin-right: 0.5rem;
  accent-color: #B0DB9C;
  cursor: pointer;
  width: 1.2rem;
  height: 1.2rem;
}

.form-group input[type='checkbox']:focus {
  outline: 2px solid #8FC97A;
  outline-offset: 2px;
}

.modal-content form {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.modal-content form button {
  padding: 0.5rem 1.5rem;
  margin-top: 10px;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  font-weight: 500;
  transition: background 0.2s;
}

.modal-content form .btn-primary {
  background-color: #B0DB9C;
  color: white;
}

.modal-content form .btn-primary:hover {
  background-color: #8FC97A;
}

.modal-content form .btn-danger {
  background-color: #dc3545;
  color: white;
  margin-left: auto;
}

.modal-content form .btn-danger:hover {
  background-color: #c82333;
}
</style>
