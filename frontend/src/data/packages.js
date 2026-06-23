export const pricingPackages = [
  {
    id: 'lite',
    name: 'Başlangıç Paketi',
    tagline: 'Lite / Eko',
    price: 250,
    currency: 'USD',
    highlight: false,
    customerOrder: 'Aylık sipariş sayısı',
    features: ['Temel sipariş takibi', 'Müşteri listesi'],
  },
  {
    id: 'pro',
    name: 'Standart Paket',
    tagline: 'Pro / Büyüyen İşletmeler',
    price: 500,
    currency: 'USD',
    highlight: true,
    customerOrder:
      'Sipariş yönetimi, müşteri geçmişi ve detaylı müşteri profilleri',
    features: [
      'Gelişmiş filtreleme ve arama',
      'Temel ciro / sipariş raporları',
    ],
  },
]
