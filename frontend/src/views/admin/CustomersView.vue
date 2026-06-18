<script setup>
import { ref, onMounted } from 'vue'
import { getCustomers, createCustomer } from '../../api/customers'
import AlertMessage from '../../components/AlertMessage.vue'

const customers = ref([])
const loading = ref(true)
const error = ref('')
const success = ref('')

const form = ref({ fullName: '', email: '', phone: '' })

async function load() {
  loading.value = true
  error.value = ''
  try {
    customers.value = await getCustomers()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  error.value = ''
  success.value = ''
  try {
    await createCustomer(form.value)
    success.value = 'Müşteri eklendi.'
    form.value = { fullName: '', email: '', phone: '' }
    await load()
  } catch (e) {
    error.value = e.message
  }
}

onMounted(load)
</script>

<template>
  <div>
    <h1>Müşteriler</h1>

    <AlertMessage v-if="error" type="error" :message="error" />
    <AlertMessage v-if="success" type="success" :message="success" />

    <form class="card form" @submit.prevent="handleSubmit">
      <h3>Yeni Müşteri</h3>
      <label>
        Ad Soyad
        <input v-model="form.fullName" required />
      </label>
      <label>
        E-posta
        <input v-model="form.email" type="email" required />
      </label>
      <label>
        Telefon
        <input v-model="form.phone" />
      </label>
      <button type="submit" class="btn-primary">Ekle</button>
    </form>

    <p v-if="loading" class="muted">Yükleniyor...</p>
    <div v-else class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Ad Soyad</th>
            <th>E-posta</th>
            <th>Telefon</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in customers" :key="c.id">
            <td>{{ c.id }}</td>
            <td>{{ c.fullName }}</td>
            <td>{{ c.email }}</td>
            <td>{{ c.phone || '—' }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
