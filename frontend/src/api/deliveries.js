import { apiFetch } from './client'

export const getDailyDeliveries = (date) => {
  const query = date ? `?date=${date}` : ''
  return apiFetch(`/deliveries/daily${query}`)
}

export const recordDayDeliveries = (date) =>
  apiFetch('/deliveries/record-day', {
    method: 'POST',
    body: JSON.stringify({ date: date || null }),
  })
