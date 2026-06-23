<script setup>
import { pricingPackages } from '../data/packages'
import { RouterLink } from 'vue-router'

function formatUsd(amount) {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0,
  }).format(amount)
}
</script>

<template>
  <div class="page">
    <section class="hero">
      <p class="hero-badge">Fırın & perakende için bulut yazılım</p>
      <h1>İşletmenize uygun paketi seçin</h1>
      <p class="hero-sub">
        Sipariş ve müşteri yönetimini dijitalleştirin. Admin paneliniz hazır — size uygun
        paketle hemen başlayın.
      </p>
    </section>

    <section class="packages-grid">
      <article
        v-for="pkg in pricingPackages"
        :key="pkg.id"
        class="package-card"
        :class="{ featured: pkg.highlight }"
      >
        <div v-if="pkg.highlight" class="popular-badge">Önerilen</div>

        <p class="package-tag">{{ pkg.tagline }}</p>
        <h2>{{ pkg.name }}</h2>

        <div class="package-price">
          <span class="amount">{{ formatUsd(pkg.price) }}</span>
          <span class="period">/ tek seferlik lisans</span>
        </div>

        <div class="package-block">
          <h3>Müşteri & Sipariş</h3>
          <p>{{ pkg.customerOrder }}</p>
        </div>

        <div class="package-block">
          <h3>Özellikler</h3>
          <ul>
            <li v-for="feature in pkg.features" :key="feature">{{ feature }}</li>
          </ul>
        </div>

        <RouterLink
          :to="{ name: 'login' }"
          class="package-cta"
          :class="{ primary: pkg.highlight }"
        >
          {{ pkg.highlight ? 'Pro ile Başla' : 'Lite ile Başla' }}
        </RouterLink>
      </article>
    </section>

    <section class="note card">
      <p>
        <strong>Not:</strong> Paket satın alma ve ödeme entegrasyonu yakında eklenecek.
        Şimdilik demo için <RouterLink to="/login">admin girişi</RouterLink> yapabilirsiniz.
      </p>
    </section>
  </div>
</template>

<style scoped>
.hero-badge {
  display: inline-block;
  margin-bottom: 0.75rem;
  padding: 0.35rem 0.85rem;
  border-radius: 999px;
  background: rgba(196, 92, 38, 0.12);
  color: var(--primary);
  font-size: 0.85rem;
  font-weight: 600;
}

.hero-sub {
  max-width: 560px;
  margin: 0.75rem auto 0;
  color: var(--muted);
}

.packages-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.package-card {
  position: relative;
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  padding: 1.75rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.package-card.featured {
  border-color: var(--primary);
  box-shadow: 0 8px 32px rgba(196, 92, 38, 0.12);
}

.popular-badge {
  position: absolute;
  top: -0.65rem;
  right: 1.25rem;
  background: var(--primary);
  color: white;
  font-size: 0.75rem;
  font-weight: 700;
  padding: 0.3rem 0.75rem;
  border-radius: 999px;
}

.package-tag {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--primary);
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.package-card h2 {
  margin: 0;
  font-size: 1.35rem;
}

.package-price {
  display: flex;
  flex-wrap: wrap;
  align-items: baseline;
  gap: 0.35rem;
}

.amount {
  font-size: 2.25rem;
  font-weight: 800;
  color: var(--text);
  line-height: 1;
}

.period {
  font-size: 0.9rem;
  color: var(--muted);
}

.package-block h3 {
  margin: 0 0 0.5rem;
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--muted);
}

.package-block p {
  margin: 0;
  line-height: 1.55;
}

.package-block ul {
  margin: 0;
  padding-left: 1.2rem;
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.package-cta {
  margin-top: auto;
  text-align: center;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-weight: 600;
  border: 2px solid var(--primary);
  color: var(--primary);
  transition: background 0.15s, color 0.15s;
}

.package-cta:hover {
  background: #fff3e8;
}

.package-cta.primary {
  background: var(--primary);
  color: white;
  border-color: var(--primary);
}

.package-cta.primary:hover {
  background: var(--primary-dark);
}

.note {
  text-align: center;
  color: var(--muted);
  line-height: 1.6;
}

.note strong {
  color: var(--text);
}
</style>
