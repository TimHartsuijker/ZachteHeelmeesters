import axios from 'axios'

const api = axios.create({
  // @ts-ignore
  baseURL: import.meta?.env?.VITE_API_URL || 'https://localhost:7240/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

console.log('[API baseURL]', api.defaults.baseURL)

export default api

// Type definitie voor patient response
export type PatientResponse = {
  patientID: number;
  voornaam: string;
  achternaam: string;
  email: string;
  telefoonnummer: string;
  straatnaam: string;
  huisnummer: string;
  postcode: string;
  plaats: string | null;
  bsn: string | null;
  geboortedatum: string | null;
  geslacht: string | null;
  huisartspraktijk: string | null;
  huisartsnaam: string | null;
};

// Patient ophalen via axios instance
export async function getPatient(id: number): Promise<PatientResponse> {
  const response = await api.get<PatientResponse>(`/patient/${id}`);
  return response.data;
}
