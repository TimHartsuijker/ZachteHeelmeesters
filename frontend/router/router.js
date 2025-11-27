import { createRouter, createWebHistory } from 'vue-router'

const routes = [
    { 
        path: '', name: '', //component: 
    }
]

const router = createRouter({
  history: createWebHistory(), // uses the HTML5 history API — no page reloads
  routes,
})

export default router