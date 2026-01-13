import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import MedicalDossier from '../views/MedicalDossier.vue'
import DoctorUpload from '../views/DoctorUpload.vue'

const routes = [
  { 
    path: '/', 
    name: 'login',
    component: LoginView
  },
  {
    path: '/dossier',
    name: 'dossier',
    component: MedicalDossier
  },
  {
    path: '/doctor/upload',
    name: 'doctor-upload',
    component: DoctorUpload
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
