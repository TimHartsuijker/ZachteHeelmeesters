import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import AgendaView from '../views/AgendaView.vue'

const routes = [
  { 
    path: '/', 
    name: 'login',
    component: LoginView
  },
  {
    path: '/agenda',
    name: 'agenda',
    component: AgendaView
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
