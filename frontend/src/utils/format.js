export const orderStatusLabel = {
  0: 'Beklemede',
  1: 'Onaylandı',
  2: 'Tamamlandı',
  3: 'İptal',
}

export function formatPrice(value) {
  return new Intl.NumberFormat('tr-TR', {
    style: 'currency',
    currency: 'TRY',
  }).format(value)
}

export function formatDate(value) {
  return new Date(value).toLocaleString('tr-TR')
}
