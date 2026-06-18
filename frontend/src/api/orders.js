import { apiFetch, fetchPagedItems } from './client'

export const getOrders = (page = 1, size = 100) => fetchPagedItems('/orders', page, size)
export const createOrder = (data) =>
  apiFetch('/orders', { method: 'POST', body: JSON.stringify(data) })
export const cancelOrder = (id) =>
  apiFetch(`/orders/${id}/cancel`, { method: 'POST' })
