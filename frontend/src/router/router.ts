import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/dashboard.vue'
import Patiëntprofiel from '../views/Patiëntprofiel.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/dashboard'
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
    },
    {
      path: '/Patiëntprofiel',
      name: 'patiëntprofiel',
      component: Patiëntprofiel,
    }
  ],
})

export default router
