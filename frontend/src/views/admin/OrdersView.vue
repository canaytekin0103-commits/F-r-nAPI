<script setup>
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { getOrders, cancelOrder } from '../../api/orders'
import { orderStatusLabel, formatPrice, formatDate } from '../../utils/format'
import AlertMessage from '../../components/AlertMessage.vue'

const orders = ref([])
const loading = ref(true)
const error = ref('')
const success = ref('')

async function loadOrders() {
  loading.value = true
  error.value = ''
  try {
    orders.value = await getOrders()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function handleCancel(id) {
  if (!confirm('Sipariş iptal edilsin mi? Ekmek stokları geri iade edilir.')) return
  success.value = ''
  error.value = ''
  try {
    await cancelOrder(id)
    success.value = 'Sipariş iptal edildi, stok güncellendi.'
    await loadOrders()
  } catch (e) {
    error.value = e.message
  }
}

onMounted(loadOrders)
</script>

<template>
  <div>
    <div class="page-header">
      <h1>Siparişler</h1>
      <RouterLink to="/admin/orders/new" class="btn-primary">+ Yeni Sipariş</RouterLink>
    </div>

    <AlertMessage v-if="error" type="error" :message="error" />
    <AlertMessage v-if="success" type="success" :message="success" />
    <p v-if="loading" class="muted">Yükleniyor...</p>

    <div v-else-if="orders.length === 0" class="card muted">Henüz sipariş yok.</div>

    <div v-else class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Müşteri</th>
            <th>Tarih</th>
            <th>Durum</th>
            <th>Tutar</th>
            <th>Ürünler</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td>{{ order.id }}</td>
            <td>{{ order.customerName }}</td>
            <td>{{ formatDate(order.orderDate) }}</td>
            <td>
              <span class="badge" :class="'status-' + order.status">
                {{ orderStatusLabel[order.status] }}
              </span>
            </td>
            <td>{{ formatPrice(order.totalAmount) }}</td>
            <td>
              <small v-for="item in order.items" :key="item.id">
                {{ item.productName }} × {{ item.quantity }}<br />
              </small>
            </td>
            <td>
              <button
                v-if="order.status !== 3 && order.status !== 2"
                class="btn-danger btn-sm"
                @click="handleCancel(order.id)"
              >
                İptal
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
