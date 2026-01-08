import axios from 'axios'

// Basis URL instellen
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7240/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

// 🔐 Token automatisch meesturen
api.interceptors.request.use(config => {
  const token = sessionStorage.getItem('token')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api

// ------------------
// Types
// ------------------
export type PatientResponse = {
  patientID: number
  voornaam: string
  achternaam: string
  email: string
  telefoonnummer: string
  straatnaam: string
  huisnummer: string
  postcode: string
  plaats: string | null
  bsn: string | null
  geboortedatum: string | null
  geslacht: string | null
  huisartspraktijk: string | null
  huisartsnaam: string | null
}

// ------------------
// API calls
// ------------------
export async function getMyPatient(): Promise<PatientResponse> {
  const userId = sessionStorage.getItem('userId')
  if (!userId) throw new Error('Niet ingelogd')

  const response = await api.get<PatientResponse>(`/patient/${userId}`)
  return response.data
}
