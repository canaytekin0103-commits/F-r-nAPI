<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { getDailyDeliveries, recordDayDeliveries } from '../../api/deliveries'
import { getCustomers } from '../../api/customers'
import { getBreads } from '../../api/breads'
import {
  getStandingOrders,
  addStandingOrder,
  deleteStandingOrder,
} from '../../api/standingOrders'
import { formatPrice } from '../../utils/format'
import AlertMessage from '../../components/AlertMessage.vue'

const today = new Date().toISOString().slice(0, 10)
const selectedDate = ref(today)
const summary = ref(null)
const loading = ref(true)
const recording = ref(false)
const error = ref('')
const success = ref('')

const customers = ref([])
const breads = ref([])
const selectedCustomerId = ref('')
const standingOrders = ref([])
const standingLoading = ref(false)
const standingForm = ref({ breadId: '', quantity: 1 })

const hasDeliveries = computed(() => (summary.value?.deliveries?.length ?? 0) > 0)

async function loadCustomersAndProducts() {
  const [c, b] = await Promise.all([getCustomers(), getBreads()])
  customers.value = c
  breads.value = b
  if (!selectedCustomerId.value && c.length) selectedCustomerId.value = String(c[0].id)
  if (b.length) standingForm.value.breadId = String(b[0].id)
}

async function loadStandingOrders() {
  if (!selectedCustomerId.value) {
    standingOrders.value = []
    return
  }
  standingLoading.value = true
  try {
    standingOrders.value = await getStandingOrders(Number(selectedCustomerId.value))
  } catch (e) {
    error.value = e.message
  } finally {
    standingLoading.value = false
  }
}

async function loadDeliveries() {
  loading.value = true
  error.value = ''
  try {
    summary.value = await getDailyDeliveries(selectedDate.value)
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

async function loadAll() {
  loading.value = true
  error.value = ''
  try {
    await loadCustomersAndProducts()
    await Promise.all([loadStandingOrders(), loadDeliveries()])
  } catch (e) {
    error.value = e.message
    loading.value = false
  }
}

async function handleAddStanding() {
  error.value = ''
  success.value = ''
  try {
    await addStandingOrder(Number(selectedCustomerId.value), {
      breadId: Number(standingForm.value.breadId),
      quantity: Number(standingForm.value.quantity),
    })
    success.value = 'Düzenli ürün eklendi.'
    standingForm.value.quantity = 1
    await loadStandingOrders()
  } catch (e) {
    error.value = e.message
  }
}

async function handleDeleteStanding(id) {
  if (!confirm('Bu düzenli ürün silinsin mi?')) return
  error.value = ''
  try {
    await deleteStandingOrder(Number(selectedCustomerId.value), id)
    success.value = 'Düzenli ürün silindi.'
    await loadStandingOrders()
  } catch (e) {
    error.value = e.message
  }
}

async function handleRecordDay() {
  const label = summary.value?.dayLabel ?? selectedDate.value
  if (
    !confirm(
      `${label} için tüm düzenli müşterilerin teslimatı kaydedilsin mi?\n(Stok düşülür, durum "Verildi" olur.)`
    )
  )
    return

  recording.value = true
  error.value = ''
  success.value = ''
  try {
    const result = await recordDayDeliveries(selectedDate.value)
    const parts = []
    if (result.created?.length) parts.push(`${result.created.length} müşteri kaydedildi`)
    if (result.skippedCustomers?.length)
      parts.push(`${result.skippedCustomers.length} müşteri zaten kayıtlıydı (atlandı)`)
    success.value = parts.join('. ') || 'Kayıt tamamlandı.'
    await loadDeliveries()
  } catch (e) {
    error.value = e.message
  } finally {
    recording.value = false
  }
}

watch(selectedCustomerId, loadStandingOrders)
watch(selectedDate, loadDeliveries)
onMounted(loadAll)
</script>

<template>
  <div>
    <h1>Günlük Teslimat</h1>
    <p class="muted intro">
      Önce müşterilerin düzenli siparişini tanımlayın, sonra her gün teslimatı tek tıkla kaydedin.
    </p>

    <AlertMessage v-if="error" type="error" :message="error" />
    <AlertMessage v-if="success" type="success" :message="success" />

    <!-- 1. Düzenli günlük sipariş -->
    <section class="card module-section">
      <h2>1. Düzenli Günlük Sipariş</h2>
      <p class="muted">Her müşterinin her gün aldığı ürünleri buraya ekleyin.</p>

      <label>
        Müşteri seç
        <select v-model="selectedCustomerId" :disabled="!customers.length">
          <option v-for="c in customers" :key="c.id" :value="String(c.id)">
            {{ c.fullName }}
          </option>
        </select>
      </label>
      <p v-if="!customers.length" class="muted">
        Henüz müşteri yok. Sol menüden <strong>Müşteriler</strong> bölümüne ekleyin.
      </p>

      <form v-if="selectedCustomerId" class="standing-form" @submit.prevent="handleAddStanding">
        <label>
          Ürün
          <select v-model="standingForm.breadId" required>
            <option v-for="b in breads" :key="b.id" :value="String(b.id)">
              {{ b.name }}
            </option>
          </select>
        </label>
        <label>
          Günlük adet
          <input v-model.number="standingForm.quantity" type="number" min="1" required />
        </label>
        <button type="submit" class="btn-primary">Listeye Ekle</button>
      </form>

      <p v-if="standingLoading" class="muted">Yükleniyor...</p>
      <ul v-else-if="standingOrders.length" class="standing-list">
        <li v-for="s in standingOrders" :key="s.id">
          <span>{{ s.productName }} × {{ s.quantity }} adet/gün</span>
          <button class="btn-danger btn-sm" @click="handleDeleteStanding(s.id)">Sil</button>
        </li>
      </ul>
      <p v-else-if="selectedCustomerId" class="muted">Bu müşteri için henüz düzenli ürün yok.</p>
    </section>

    <!-- 2. Günlük teslimat kaydı -->
    <section class="card module-section">
      <div class="section-header">
        <h2>2. Günlük Teslimat Kaydı</h2>
        <button class="btn-primary" :disabled="recording" @click="handleRecordDay">
          {{ recording ? 'Kaydediliyor...' : 'Bugünü Kaydet (Verildi)' }}
        </button>
      </div>

      <label class="form-row">
        Tarih
        <input v-model="selectedDate" type="date" />
      </label>

      <p v-if="loading" class="muted">Yükleniyor...</p>

      <template v-else-if="summary">
        <div class="summary-banner">
          <h3>{{ summary.dayLabel }}</h3>
          <p v-if="hasDeliveries" class="summary-stats">
            <strong>{{ summary.customerCount }}</strong> müşteriye teslimat ·
            <strong>{{ formatPrice(summary.totalAmount) }}</strong> toplam
          </p>
          <p v-else class="muted">Bu gün için henüz teslimat kaydı yok.</p>

          <div v-if="summary.totals?.length" class="totals">
            <span v-for="t in summary.totals" :key="t.productName" class="total-chip">
              {{ t.productName }}: <strong>{{ t.quantity }}</strong> adet
            </span>
          </div>
        </div>

        <div v-if="hasDeliveries" class="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Müşteri</th>
                <th>Verilen Ürünler</th>
                <th>Tutar</th>
                <th>Durum</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="d in summary.deliveries" :key="d.id">
                <td>{{ d.customerName }}</td>
                <td>
                  <small v-for="item in d.items" :key="item.id">
                    {{ item.productName }} × {{ item.quantity }}<br />
                  </small>
                </td>
                <td>{{ formatPrice(d.totalAmount) }}</td>
                <td><span class="badge status-2">Verildi</span></td>
              </tr>
            </tbody>
          </table>
        </div>
      </template>
    </section>
  </div>
</template>

<style scoped>
.intro {
  margin-bottom: 1.25rem;
}
.module-section {
  margin-bottom: 1.5rem;
}
.module-section h2 {
  margin: 0 0 0.35rem;
  font-size: 1.15rem;
}
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}
.section-header h2 {
  margin: 0;
}
.standing-form {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  align-items: flex-end;
  margin: 1rem 0;
}
.standing-list {
  list-style: none;
  margin-top: 1rem;
}
.standing-list li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-bottom: 1px solid var(--border);
}
.form-row {
  display: block;
  max-width: 240px;
  margin-bottom: 1rem;
}
.summary-banner {
  margin-bottom: 1rem;
  padding: 1rem;
  background: linear-gradient(135deg, #fff8f0, #fdebd0);
  border-radius: var(--radius);
}
.summary-banner h3 {
  margin: 0 0 0.5rem;
  font-size: 1.1rem;
}
.summary-stats {
  margin-bottom: 0.75rem;
}
.totals {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}
.total-chip {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 999px;
  padding: 0.35rem 0.85rem;
  font-size: 0.9rem;
}
</style>
