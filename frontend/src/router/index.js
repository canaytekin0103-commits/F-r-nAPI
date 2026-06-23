import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'menu', component: () => import('../views/MenuView.vue') },
    { path: '/login', name: 'login', component: () => import('../views/LoginView.vue') },
    {
      path: '/admin',
      component: () => import('../views/admin/AdminLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        { path: '', redirect: '/admin/deliveries' },
        { path: 'deliveries', name: 'deliveries', component: () => import('../views/admin/DailyDeliveriesView.vue') },
        { path: 'orders', name: 'orders', component: () => import('../views/admin/OrdersView.vue') },
        { path: 'orders/new', name: 'new-order', component: () => import('../views/admin/NewOrderView.vue') },
        { path: 'customers', name: 'customers', component: () => import('../views/admin/CustomersView.vue') },
      ],
    },
  ],
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  if (requiresAuth && !auth.isLoggedIn) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
})

export default router
