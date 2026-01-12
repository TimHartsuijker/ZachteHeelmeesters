import { createRouter, createWebHistory } from 'vue-router'
import MedicalDossier from '../src/views/MedicalDossier.vue'
import DoctorUpload from '../src/views/DoctorUpload.vue'

const routes = [
    { 
        path: '/dossier', 
        name: 'MedicalDossier', 
        component: MedicalDossier
    },
    { 
        path: '/doctor/upload', 
        name: 'DoctorUpload', 
        component: DoctorUpload
    },
    {
        path: '/',
        redirect: '/dossier' // Default to dossier view
    }
]

const router = createRouter({
  history: createWebHistory(), // uses the HTML5 history API — no page reloads
  routes,
})

export default router