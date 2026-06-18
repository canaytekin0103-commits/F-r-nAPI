<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="admin-layout">
    <aside class="sidebar">
      <h2>Yönetim Paneli</h2>
      <nav>
        <RouterLink to="/admin/orders">Siparişler</RouterLink>
        <RouterLink to="/admin/orders/new">Yeni Sipariş</RouterLink>
        <RouterLink to="/admin/customers">Müşteriler</RouterLink>
      </nav>
      <RouterLink to="/" class="back-link">← Menüye dön</RouterLink>
    </aside>
    <main class="admin-content">
      <RouterView />
    </main>
  </div>
</template>
