import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue' // <-- importeer de nieuwe view
import DashboardView from '../views/DashboardView.vue'
import AgendaView from '../views/AgendaView.vue'
import AdminLoginView from "../views/AdminLoginView.vue";
import AdminDashboardView from "../views/AdminDashboardView.vue";
import UsersView from "../views/AdminUserManagementView.vue";
import CreateReferralView from '../views/CreateReferralView.vue' 

const routes = [
  { 
    path: '/login', 
    name: 'login',
    component: LoginView,
    meta: { hideNavbar: true }
  },
  { 
    path: '/register',
    name: 'register',
    component: RegisterView,
    meta: { hideNavbar: true }
  },
  {
    path: "/admin/login",
    name: "AdminLogin",
    component: AdminLoginView,
    meta: { hideNavbar: true }
  },
  { 
    path: '/dashboard', 
    name: 'dashboard',
    component: DashboardView,
    meta: { requiresAuth: true, allowedRoles: ['Patiënt'] }
  },
  {
    path: '/agenda',
    name: 'agenda',
    component: AgendaView,
    meta: { requiresAuth: true, allowedRoles: ['Specialist'] }
  },
  {
    path: "/doorverwijzing-aanmaken",
    name: "CreateReferral",
    component: CreateReferralView,
    meta: { requiresAuth: true, allowedRoles: ['Huisarts'] },
  },
  {
    path: "/admin/dashboard",
    name: "AdminDashboard",
    component: AdminDashboardView,
    meta: { requiresAuth: true, allowedRoles: ['Admin'] },
  },
  {
    path: "/admin/users",
    name: "AdminUsers",
    component: UsersView,
    meta: { requiresAuth: true, allowedRoles: ['Admin'] },
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// --- DE NAVIGATIE GUARD ---
router.beforeEach((to, from, next) => {
  const userRole = sessionStorage.getItem('userRole');
  const isLoggedIn = sessionStorage.getItem('userId') !== null;
  const isAdminLoggedIn = sessionStorage.getItem('adminId') !== null;

  if (to.path == '/') {
    return next({ name: isLoggedIn || isAdminLoggedIn ? 'dashboard' : 'login' });
  }

  if (to.meta.requiresAuth) {
    
    if (!isLoggedIn && !isAdminLoggedIn) {
      if (to.path.startsWith('/admin')) {
        return next({ name: 'AdminLogin' });
      }
      return next({ name: 'login' });
    }

    if (to.meta.allowedRoles && !to.meta.allowedRoles.includes(userRole)) {
      console.warn(`Toegang geweigerd voor rol: ${userRole}`);

      return router.back();
    }
  } else if ((to.name === 'login' || to.name === 'AdminLogin') && (isLoggedIn || isAdminLoggedIn)) {
    return router.back();
  }
  next();
})

export default router