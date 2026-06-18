<script setup>
import { ref, onMounted } from 'vue'
import { getBreads } from '../api/breads'
import { getCakes } from '../api/cakes'
import { formatPrice } from '../utils/format'
import AlertMessage from '../components/AlertMessage.vue'

const breads = ref([])
const cakes = ref([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    const [breadData, cakeData] = await Promise.all([getBreads(), getCakes()])
    breads.value = breadData
    cakes.value = cakeData
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="page">
    <section class="hero">
      <h1>Fırınımıza Hoş Geldiniz</h1>
      <p>Taze ekmekler ve özel pastalar — günlük üretim.</p>
    </section>

    <AlertMessage v-if="error" type="error" :message="error" />
    <p v-if="loading" class="muted">Yükleniyor...</p>

    <section v-if="!loading">
      <h2>🍞 Ekmekler</h2>
      <div class="grid">
        <article v-for="bread in breads" :key="bread.id" class="card">
          <h3>{{ bread.name }}</h3>
          <p class="price">{{ formatPrice(bread.price) }}</p>
          <p class="muted">Stok: {{ bread.stockQuantity }} adet</p>
        </article>
      </div>
    </section>

    <section v-if="!loading && cakes.length">
      <h2>🎂 Pastalar</h2>
      <div class="grid">
        <article v-for="cake in cakes" :key="cake.id" class="card">
          <h3>{{ cake.name }}</h3>
          <p v-if="cake.description" class="muted">{{ cake.description }}</p>
          <p class="price">{{ formatPrice(cake.price) }}</p>
          <span class="badge" :class="cake.isAvailable ? 'ok' : 'off'">
            {{ cake.isAvailable ? 'Satışta' : 'Tükendi' }}
          </span>
        </article>
      </div>
    </section>
  </div>
</template>
