import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue' // <-- importeer de nieuwe view
import AdminLoginView from "../views/AdminLoginView.vue";
import AdminDashboardView from "../views/AdminDashboardView.vue";
import UsersView from "../views/AdminUserManagementView.vue";

const routes = [
  { 
    path: '/', 
    name: 'login',
    component: LoginView
  },
  { 
    path: '/register',
    name: 'register',
    component: RegisterView
  },
  {
    path: "/admin/login",
    name: "AdminLogin",
    component: AdminLoginView
  },
  {
    path: "/admin/dashboard",
    name: "AdminDashboard",
    component: AdminDashboardView,
    meta: { requiresAdmin: true }, // optioneel, handig voor route guards
  },
  {
    path: "/admin/users",
    name: "AdminUsers",
    component: UsersView,
    meta: { requiresAdmin: true },
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
