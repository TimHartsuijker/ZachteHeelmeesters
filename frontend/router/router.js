import { createRouter, createWebHistory } from 'vue-router'
import DoctorCalendar from '../src/components/DoctorCalendar.vue'

const routes = [
    { 
        path: '/', 
        name: 'Calendar', 
        component: DoctorCalendar
    }
]

const router = createRouter({
  history: createWebHistory(), // uses the HTML5 history API — no page reloads
  routes,
})

export default router