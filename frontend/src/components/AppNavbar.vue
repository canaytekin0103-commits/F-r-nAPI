<script setup>
import { RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
</script>

<template>
  <header class="navbar">
    <RouterLink to="/" class="brand">🥖 Fırın Yönetim</RouterLink>
    <nav class="nav-links">
      <RouterLink to="/">Paketler</RouterLink>
      <template v-if="auth.isLoggedIn">
        <RouterLink to="/admin/deliveries">Yönetim</RouterLink>
        <button class="btn-text" @click="auth.logout(); $router.push('/')">
          Çıkış ({{ auth.username }})
        </button>
      </template>
      <RouterLink v-else to="/login" class="btn-login">Giriş</RouterLink>
    </nav>
  </header>
</template>
