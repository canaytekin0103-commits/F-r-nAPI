<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getCustomers } from '../../api/customers'
import { getBreads } from '../../api/breads'
import { getCakes } from '../../api/cakes'
import { createOrder } from '../../api/orders'
import AlertMessage from '../../components/AlertMessage.vue'

const router = useRouter()

const customers = ref([])
const breads = ref([])
const cakes = ref([])
const customerId = ref('')
const productType = ref(1)
const productId = ref('')
const quantity = ref(1)
const error = ref('')
const loading = ref(false)

const products = ref([])

function updateProductList() {
  products.value = productType.value === 1 ? breads.value : cakes.value
  productId.value = products.value[0]?.id ?? ''
}

onMounted(async () => {
  try {
    const [c, b, k] = await Promise.all([getCustomers(), getBreads(), getCakes()])
    customers.value = c
    breads.value = b
    cakes.value = k.filter((x) => x.isAvailable)
    if (c.length) customerId.value = c[0].id
    updateProductList()
  } catch (e) {
    error.value = e.message
  }
})

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    await createOrder({
      customerId: Number(customerId.value),
      items: [
        {
          productType: Number(productType.value),
          productId: Number(productId.value),
          quantity: Number(quantity.value),
        },
      ],
    })
    router.push('/admin/orders')
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <h1>Yeni Sipariş</h1>
    <AlertMessage v-if="error" type="error" :message="error" />

    <form class="card form" @submit.prevent="handleSubmit">
      <label>
        Müşteri
        <select v-model="customerId" required>
          <option v-for="c in customers" :key="c.id" :value="c.id">
            {{ c.fullName }} ({{ c.email }})
          </option>
        </select>
        <small v-if="!customers.length" class="muted">
          Önce müşteri ekleyin.
        </small>
      </label>

      <label>
        Ürün tipi
        <select v-model="productType" @change="updateProductList">
          <option :value="1">Ekmek</option>
          <option :value="2">Pasta</option>
        </select>
      </label>

      <label>
        Ürün
        <select v-model="productId" required>
          <option v-for="p in products" :key="p.id" :value="p.id">
            {{ p.name }}
          </option>
        </select>
      </label>

      <label>
        Adet
        <input v-model.number="quantity" type="number" min="1" required />
      </label>

      <button type="submit" class="btn-primary" :disabled="loading || !customers.length">
        {{ loading ? 'Kaydediliyor...' : 'Sipariş Oluştur' }}
      </button>
    </form>
  </div>
</template>
