/**
 * Utility functies voor datum- en tijdbeheer in de DoctorCalendar
 */

export const pad = (value) => String(value).padStart(2, '0');

export const getMondayOfWeek = (date) => {
  const d = new Date(date);
  const day = d.getDay();
  const diff = d.getDate() - day + (day === 0 ? -6 : 1); // Corrigeer naar Maandag
  return new Date(d.setDate(diff));
};

export const toLocalIsoFromDate = (dateObj) => {
  const y = dateObj.getFullYear();
  const m = pad(dateObj.getMonth() + 1);
  const d = pad(dateObj.getDate());
  const hh = pad(dateObj.getHours());
  const mm = pad(dateObj.getMinutes());
  const ss = pad(dateObj.getSeconds());
  return `${y}-${m}-${d}T${hh}:${mm}:${ss}`;
};

export const toIso = (dateString, hour, minute) => {
  const [year, month, day] = dateString.split('-');
  return `${year}-${month}-${day}T${pad(hour)}:${pad(minute)}:00`;
};

export const getSlots = (dateString) => {
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
};

export const getTimesFrom = (startHour, startMinute) => {
  const times = [];
  for (let hour = startHour; hour < 24; hour++) {
    const minStart = (hour === startHour ? startMinute + 15 : 0);
    for (let minute = minStart; minute < 60; minute += 15) {
      times.push(`${pad(hour)}:${pad(minute)}`);
    }
  }
  return times;
};

export const formatEndTime = (isoString) => {
  const date = new Date(isoString);
  const dateStr = date.toLocaleDateString('nl-NL');
  const timeStr = `${pad(date.getHours())}:${pad(date.getMinutes())}`;
  return `${dateStr} ${timeStr}`;
};

export const formatSlotTime = (dateString, hour, minute) => {
  const date = new Date(dateString + 'T00:00:00');
  const dayName = date.toLocaleDateString('nl-NL', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });
  return `${dayName} van ${pad(hour)}:${pad(minute)}`;
};

export const getMaxDate = (startDateTime) => {
  const date = new Date(startDateTime);
  date.setDate(date.getDate() + 1);
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};