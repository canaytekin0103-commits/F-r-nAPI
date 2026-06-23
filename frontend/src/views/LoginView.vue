<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import AlertMessage from '../components/AlertMessage.vue'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const username = ref('admin')
const password = ref('Admin123!')
const error = ref('')
const loading = ref(false)

async function handleLogin() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(username.value, password.value)
    const redirect = route.query.redirect || '/admin/deliveries'
    router.push(redirect)
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="page narrow">
    <div class="card login-card">
      <h1>Admin Girişi</h1>
      <p class="muted">Sipariş ve müşteri yönetimi için giriş yapın.</p>

      <AlertMessage v-if="error" type="error" :message="error" />

      <form @submit.prevent="handleLogin" class="form">
        <label>
          Kullanıcı adı
          <input v-model="username" type="text" required autocomplete="username" />
        </label>
        <label>
          Şifre
          <input v-model="password" type="password" required autocomplete="current-password" />
        </label>
        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Giriş yapılıyor...' : 'Giriş Yap' }}
        </button>
      </form>
    </div>
  </div>
</template>
