import axios from 'axios'

const api = axios.create({
  // @ts-ignore -- Vite injects env on import.meta at build/runtime
  baseURL: import.meta?.env?.VITE_API_URL || 'http://localhost:7240/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

export default api
