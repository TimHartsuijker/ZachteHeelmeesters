import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';
import MedicalDossier from '../views/MedicalDossier.vue';
import DoctorUpload from '../views/DoctorUpload.vue';
import RegisterView from '../views/RegisterView.vue';
import DashboardView from '../views/DashboardView.vue';
import AgendaView from '../views/AgendaView.vue';
import AdminLoginView from "../views/AdminLoginView.vue";
import AdminDashboardView from "../views/AdminDashboardView.vue";
import UsersView from "../views/AdminUserManagementView.vue";
import CreateReferralView from '../views/CreateReferralView.vue' 
import DoctorPatientsView from "../views/DoctorPatientsView.vue";
import DoorverwijzingenInzien from '../views/DoorverwijzingenInzien.vue';
import Patientprofiel from '../views/Patientprofiel.vue';
import AccountsOverviewView from "../views/AccountsOverviewView.vue";
import AdminCreateUserView from "../views/AdminCreateUserView.vue"; 

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
    path: '/dossier',
    name: 'dossier',
    component: MedicalDossier,
    meta: { requiresAuth: true, allowedRoles: ['Patiënt'] }
  },
  {
    path: '/dossier/:patientId',
    name: 'dossier-patient',
    component: MedicalDossier,
    meta: { requiresAuth: true, allowedRoles: ['Huisarts', 'Specialist'] }
  },
  {
    path: '/doctor/upload',
    name: 'doctor-upload',
    component: DoctorUpload,
    meta: { requiresAuth: true, allowedRoles: ['Huisarts', 'Specialist'] }
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
    path: '/patientprofiel',
    name: 'patientprofiel',
    component: Patientprofiel,
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
  },
  {
    path: "/patienten",
    name: "DoctorPatients",
    component: DoctorPatientsView,
    meta: { requiresAuth: true, allowedRoles: ['Huisarts', 'Specialist'] },
  },
  {
    path: "/doorverwijzingen-inzien",
    name: "DoorverwijzingenInzien",
    component: DoorverwijzingenInzien,
    meta: { requiresAuth: true, allowedRoles: ['Patiënt'] },
  },
  {
    path: "/administratie/accounts",
    name: "AccountsOverview",
    component: AccountsOverviewView,
    meta: { requiresAuth: true, allowedRoles: ['Administratiemedewerker'] },
  },
  {
    path: "/administratie/create-user",
    name: "AdminCreateUser",
    component: AdminCreateUserView,
    meta: { requiresAuth: true, allowedRoles: ['Administratiemedewerker'] },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// --- DE NAVIGATIE GUARD ---
router.beforeEach((to, from, next) => {
  const userRole = sessionStorage.getItem('userRole');
  const isLoggedIn = sessionStorage.getItem('userId') !== null;

  if (to.path == '/') {
    return next({ name: isLoggedIn ? 'dashboard' : 'login' });
  }

  if (to.meta.requiresAuth) {
    
    if (!isLoggedIn) {
      // Voor administratie: naar normale login
      if (to.path.startsWith('/administratie')) {
        return next({ name: 'login' });
      }
      // Voor admin: naar admin login
      if (to.path.startsWith('/admin')) {
        return next({ name: 'AdminLogin' });
      }
      return next({ name: 'login' });
    }

    if (to.meta.allowedRoles && !to.meta.allowedRoles.includes(userRole)) {
      console.warn(`Toegang geweigerd voor rol: ${userRole} tot ${to.path}`);
      return router.back();
    }
  } else if ((to.name === 'login' || to.name === 'AdminLogin') && isLoggedIn) {
    return router.back();
  }
  next();
})

export default router