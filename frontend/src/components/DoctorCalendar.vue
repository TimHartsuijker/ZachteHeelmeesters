<template>
  <div class="doctor-calendar">
    <div class="calendar-header">
      <h2>Beschikbaarheidskalender</h2>
      <div class="calendar-controls">
        <button @click="previousWeek" class="btn-nav">&lt;</button>
        <button @click="goToToday" class="btn-nav btn-nav-secondary">Vandaag</button>
        <span class="current-week">{{ weekRange }}</span>
        <button @click="nextWeek" class="btn-nav">&gt;</button>
      </div>
    </div>

    <!-- Quick actions for setting unavailable periods -->
    <div class="quick-actions">
      <button @click="showUnavailablePeriodModal = true" class="btn btn-primary">
        Periode onbeschikbaar maken
      </button>
      <button @click="showAvailablePeriodModal = true" class="btn btn-secondary">
        Periode beschikbaar maken
      </button>
    </div>

    <!-- Week view -->
    <div class="week-view">
      <div class="time-grid">
        <!-- Time labels -->
        <div class="time-labels">
          <div class="time-label"></div>
          <div v-for="hour in hours" :key="hour" class="time-label">
            {{ String(hour).padStart(2, '0') }}:00
          </div>
        </div>

        <!-- Days with time slots -->
        <div class="days-container">
          <!-- Day headers -->
          <div class="day-headers">
            <div v-for="day in weekDays" :key="day.date" class="day-header">
              <div class="day-name">{{ day.name }}</div>
              <div class="day-date">{{ day.date }}</div>
            </div>
          </div>

          <!-- Time slots grid -->
          <div class="time-slots-grid">
            <div
              v-for="day in weekDays"
              :key="day.date"
              class="day-column"
            >
              <div
                v-for="slot in getSlots(day.date)"
                :key="`${slot.date}-${slot.hour}-${slot.minute}`"
                :class="['time-slot', { unavailable: isSlotUnavailable(slot.date, slot.hour, slot.minute) }]"
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

    <!-- Unavailable Period Modal -->
    <div v-if="showUnavailablePeriodModal" class="modal">
      <div class="modal-content">
        <span class="close" @click="showUnavailablePeriodModal = false">&times;</span>
        <h3>Periode onbeschikbaar maken</h3>
        <form @submit.prevent="setUnavailablePeriod">
          <div class="form-group">
            <label for="unavail-start">Start datum/tijd:</label>
            <input
              id="unavail-start"
              v-model="unavailablePeriod.startDateTime"
              type="datetime-local"
              required
            />
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
            <input
              id="unavail-reason"
              v-model="unavailablePeriod.reason"
              type="text"
              placeholder="bijv. Vakantie, Ziekte, Pauze"
            />
          </div>
          <button type="submit" class="btn btn-primary">Opslaan</button>
        </form>
      </div>
    </div>

    <!-- Available Period Modal -->
    <div v-if="showAvailablePeriodModal" class="modal">
      <div class="modal-content">
        <span class="close" @click="showAvailablePeriodModal = false">&times;</span>
        <h3>Periode beschikbaar maken</h3>
        <form @submit.prevent="setAvailablePeriod">
          <div class="form-group">
            <label for="avail-start">Start datum/tijd:</label>
            <input
              id="avail-start"
              v-model="availablePeriod.startDateTime"
              type="datetime-local"
              required
            />
          </div>
          <div class="form-group">
            <label for="avail-end">Eind datum/tijd:</label>
            <input
              id="avail-end"
              v-model="availablePeriod.endDateTime"
              type="datetime-local"
              required
            />
          </div>
          <button type="submit" class="btn btn-primary">Opslaan</button>
        </form>
      </div>
    </div>

    <!-- Hour Selection Modal -->
    <div v-if="showHourModal && selectedSlot" class="modal">
      <div class="modal-content">
        <span class="close" @click="showHourModal = false">&times;</span>
        <h3>Beschikbaarheid bewerken</h3>
        <p>{{ formatSlotTime(selectedSlot.date, selectedSlot.hour, selectedSlot.minute) }}</p>
        <form @submit.prevent="saveSlot">
          <div class="form-group">
            <label for="slot-available">
              <input
                id="slot-available"
                v-model="selectedSlot.isAvailable"
                type="checkbox"
              />
              Dit moment beschikbaar
            </label>
          </div>
          <div class="form-group" v-if="!selectedSlot.isAvailable">
            <label for="slot-reason">Reden (optioneel):</label>
            <input
              id="slot-reason"
              v-model="selectedSlot.reason"
              type="text"
              placeholder="bijv. Pauze, Afspraak, Vergadering"
            />
          </div>
          <div class="form-group" v-if="!selectedSlot.isAvailable">
            <label for="slot-end-time-unavail">Tot en met:</label>
            <select id="slot-end-time-unavail" v-model="selectedSlot.endTime">
              <option value="">Alleen dit moment</option>
                <option v-for="time in getUnavailableEndTimes()" :key="time" :value="time">
                {{ time }}
              </option>
            </select>
          </div>
          <div class="form-group" v-if="selectedSlot.isAvailable">
            <label for="slot-end-time">Tot en met:</label>
            <select id="slot-end-time" v-model="selectedSlot.endTime" required>
              <option value="">Selecteer eind tijd</option>
              <option v-for="time in getEndTimesForSlot()" :key="time" :value="time">
                {{ time }}
              </option>
            </select>
          </div>
          <button type="submit" class="btn btn-primary">Opslaan</button>
          <button
            type="button"
            @click="deleteSlot"
            class="btn btn-danger"
          >
            Verwijderen
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DoctorCalendar',
  props: {
    doctorId: {
      type: Number,
      required: true
    },
    apiBaseUrl: {
      type: String,
      default: 'http://localhost:5000/api'
    }
  },
  data() {
    return {
      currentDate: new Date(),
      hours: Array.from({ length: 24 }, (_, i) => i),
      dayNames: ['Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrijdag', 'Zaterdag', 'Zondag'],
      availabilities: {},
      showUnavailablePeriodModal: false,
      showAvailablePeriodModal: false,
      showHourModal: false,
      selectedSlot: null,
      unavailablePeriod: {
        startDateTime: '',
        endDateTime: '',
        reason: ''
      },
      availablePeriod: {
        startDateTime: '',
        endDateTime: ''
      },
      loading: false,
      error: null
    };
  },
  computed: {
    weekRange() {
      const monday = this.getMondayOfWeek(this.currentDate);
      const sunday = new Date(monday);
      sunday.setDate(sunday.getDate() + 6);
      
      const dateFormat = (date) => {
        return `${String(date.getDate()).padStart(2, '0')}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getFullYear()}`;
      };
      
      return `${dateFormat(monday)} tot ${dateFormat(sunday)}`;
    },
    weekDays() {
      const monday = this.getMondayOfWeek(this.currentDate);
      const days = [];
      
      for (let i = 0; i < 7; i++) {
        const date = new Date(monday);
        date.setDate(date.getDate() + i);
        const dateString = date.toISOString().split('T')[0];
        
        days.push({
          date: dateString,
          name: this.dayNames[i],
          fullDate: date
        });
      }
      
      return days;
    }
  },
  methods: {
    getMondayOfWeek(date) {
      const d = new Date(date);
      const day = d.getDay();
      const diff = d.getDate() - day + (day === 0 ? -6 : 1); // Adjust to Monday
      return new Date(d.setDate(diff));
    },
    previousWeek() {
      const newDate = new Date(this.currentDate);
      newDate.setDate(newDate.getDate() - 7);
      this.currentDate = newDate;
      this.loadAvailabilities();
    },
    nextWeek() {
      const newDate = new Date(this.currentDate);
      newDate.setDate(newDate.getDate() + 7);
      this.currentDate = newDate;
      this.loadAvailabilities();
    },
    goToToday() {
      this.currentDate = new Date();
      this.loadAvailabilities();
    },
    getSlots(dateString) {
      const slots = [];
      for (let hour = 0; hour < 24; hour++) {
        for (let minute = 0; minute < 60; minute += 15) {
          slots.push({
            date: dateString,
            hour: hour,
            minute: minute
          });
        }
      }
      return slots;
    },
    isSlotUnavailable(dateString, hour, minute) {
      const key = `${dateString}T${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}:00`;
      return this.availabilities[key]?.isAvailable === false;
    },
    getSlotReason(dateString, hour, minute) {
      const key = `${dateString}T${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}:00`;
      const reason = this.availabilities[key]?.reason || '';
      if (reason.length > 15) {
        return reason.substring(0, 15) + '...';
      }
      return reason;
    },
    selectSlot(dateString, hour, minute) {
      const key = `${dateString}T${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}:00`;
      this.selectedSlot = {
        date: dateString,
        hour: hour,
        minute: minute,
        isAvailable: this.availabilities[key]?.isAvailable !== false,
        reason: this.availabilities[key]?.reason || '',
        endTime: '',
        availabilityId: this.availabilities[key]?.availabilityId
      };
      this.showHourModal = true;
    },
    getEndTimesForSlot() {
      if (!this.selectedSlot || !this.selectedSlot.isAvailable) {
        return [];
      }
      const times = [];
      const startHour = this.selectedSlot.hour;
      const startMinute = this.selectedSlot.minute;
      
      // Generate end times from current slot to 23:45 in 15-minute intervals
      for (let hour = startHour; hour < 24; hour++) {
        for (let minute = (hour === startHour ? startMinute + 15 : 0); minute < 60; minute += 15) {
          times.push(`${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`);
        }
      }
      return times;
    },
    getUnavailableEndTimes() {
      if (!this.selectedSlot) {
        return [];
      }
      const times = [];
      const startHour = this.selectedSlot.hour;
      const startMinute = this.selectedSlot.minute;
      
      // Generate end times from current slot to 23:45 in 15-minute intervals
      for (let hour = startHour; hour < 24; hour++) {
        for (let minute = (hour === startHour ? startMinute + 15 : 0); minute < 60; minute += 15) {
          times.push(`${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`);
        }
      }
      return times;
    },
    getUnavailablePeriodEndTimes() {
      if (!this.unavailablePeriod.startDateTime) {
        return [];
      }
      
      const startDate = new Date(this.unavailablePeriod.startDateTime);
      const times = [];
      const current = new Date(startDate);
      
      // Generate times from start + 15 minutes to 23:45 on the same day, or to end of next day
      while (current.toISOString().split('T')[0] <= this.getMaxDate()) {
        // Skip to 15 minutes after start if same hour
        if (current.getHours() === startDate.getHours() && current.getMinutes() <= startDate.getMinutes()) {
          current.setMinutes(startDate.getMinutes() + 15);
        }
        
        if (current > startDate) {
          times.push(current.toISOString());
        }
        
        current.setMinutes(current.getMinutes() + 15);
      }
      
      return times;
    },
    getMaxDate() {
      const date = new Date(this.unavailablePeriod.startDateTime);
      date.setDate(date.getDate() + 1); // Next day
      return date.toISOString().split('T')[0];
    },
    formatEndTime(isoString) {
      const date = new Date(isoString);
      const dateStr = date.toLocaleDateString('nl-NL');
      const timeStr = `${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
      return `${dateStr} ${timeStr}`;
    },
    saveSlot() {
      if (this.selectedSlot.isAvailable && !this.selectedSlot.endTime) {
        alert('Selecteer alstublieft een eind tijd');
        return;
      }
      
      const startKey = `${this.selectedSlot.date}T${String(this.selectedSlot.hour).padStart(2, '0')}:${String(this.selectedSlot.minute).padStart(2, '0')}:00`;
      
      if (this.selectedSlot.isAvailable) {
        // Mark all slots from start to end as available
        const [endHour, endMinute] = this.selectedSlot.endTime.split(':').map(Number);
        let current = new Date(`${this.selectedSlot.date}T${String(this.selectedSlot.hour).padStart(2, '0')}:${String(this.selectedSlot.minute).padStart(2, '0')}:00`);
        const end = new Date(`${this.selectedSlot.date}T${String(endHour).padStart(2, '0')}:${String(endMinute).padStart(2, '0')}:00`);
        
        while (current < end) {
          const h = current.getHours();
          const m = current.getMinutes();
          const slotKey = `${this.selectedSlot.date}T${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:00`;
          this.availabilities[slotKey] = {
            dateTime: slotKey,
            isAvailable: true,
            reason: '',
            availabilityId: this.availabilities[slotKey]?.availabilityId || null
          };
          current.setMinutes(current.getMinutes() + 15);
        }
      } else {
        // Mark slots as unavailable
        if (this.selectedSlot.endTime) {
          // Mark from start to end time as unavailable
          const [endHour, endMinute] = this.selectedSlot.endTime.split(':').map(Number);
          let current = new Date(`${this.selectedSlot.date}T${String(this.selectedSlot.hour).padStart(2, '0')}:${String(this.selectedSlot.minute).padStart(2, '0')}:00`);
          const end = new Date(`${this.selectedSlot.date}T${String(endHour).padStart(2, '0')}:${String(endMinute).padStart(2, '0')}:00`);
          
          while (current <= end) {
            const h = current.getHours();
            const m = current.getMinutes();
            const slotKey = `${this.selectedSlot.date}T${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:00`;
            this.availabilities[slotKey] = {
              dateTime: slotKey,
              isAvailable: false,
              reason: this.selectedSlot.reason,
              availabilityId: this.availabilities[slotKey]?.availabilityId || null
            };
            current.setMinutes(current.getMinutes() + 15);
          }
        } else {
          // Mark only single slot as unavailable
          this.availabilities[startKey] = {
            dateTime: startKey,
            isAvailable: false,
            reason: this.selectedSlot.reason,
            availabilityId: this.selectedSlot.availabilityId
          };
        }
      }
      
      this.showHourModal = false;
    },
    deleteSlot() {
      const key = `${this.selectedSlot.date}T${String(this.selectedSlot.hour).padStart(2, '0')}:${String(this.selectedSlot.minute).padStart(2, '0')}:00`;
      delete this.availabilities[key];
      this.showHourModal = false;
    },
    formatSlotTime(dateString, hour, minute) {
      const date = new Date(dateString + 'T00:00:00');
      return `${date.toLocaleDateString('nl-NL', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })} van ${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`;
    },
    setUnavailablePeriod() {
      const startDate = new Date(this.unavailablePeriod.startDateTime);
      const endDate = new Date(this.unavailablePeriod.endDateTime);
      
      // Mark all hours within the period as unavailable
      const current = new Date(startDate);
      while (current < endDate) {
        const dateString = current.toISOString().split('T')[0];
        const hour = current.getHours();
        
        for (let minute = 0; minute < 60; minute += 15) {
          const slotKey = `${dateString}T${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}:00`;
          this.availabilities[slotKey] = {
            dateTime: slotKey,
            isAvailable: false,
            reason: this.unavailablePeriod.reason,
            availabilityId: null
          };
        }
        
        current.setHours(current.getHours() + 1);
      }
      
      this.unavailablePeriod = {
        startDateTime: '',
        endDateTime: '',
        reason: ''
      };
      this.showUnavailablePeriodModal = false;
    },
    setAvailablePeriod() {
      // TODO: Call API endpoint to set available period
      console.log('Setting available period:', this.availablePeriod);
      this.availablePeriod = {
        startDateTime: '',
        endDateTime: ''
      };
      this.showAvailablePeriodModal = false;
      this.loadAvailabilities();
    },
    loadAvailabilities() {
      this.loading = true;
      const monday = this.getMondayOfWeek(this.currentDate);
      const sunday = new Date(monday);
      sunday.setDate(sunday.getDate() + 6);
      
      const startDate = monday.toISOString();
      const endDate = sunday.toISOString();

      // TODO: Implement API call to fetch availabilities
      // fetch(`${this.apiBaseUrl}/doctoravailability/${this.doctorId}?startDate=${startDate}&endDate=${endDate}`)
      //   .then(response => response.json())
      //   .then(data => {
      //     this.availabilities = {};
      //     data.forEach(slot => {
      //       this.availabilities[slot.dateTime] = slot;
      //     });
      //     this.loading = false;
      //   })
      //   .catch(error => {
      //     this.error = error.message;
      //     this.loading = false;
      //   });

      this.loading = false;
    }
  },
  mounted() {
    this.loadAvailabilities();
  }
};
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
