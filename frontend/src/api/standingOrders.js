import { apiFetch } from './client'

export const getStandingOrders = (customerId) =>
  apiFetch(`/customers/${customerId}/standing-orders`)

export const addStandingOrder = (customerId, data) =>
  apiFetch(`/customers/${customerId}/standing-orders`, {
    method: 'POST',
    body: JSON.stringify(data),
  })

export const updateStandingOrder = (customerId, id, data) =>
  apiFetch(`/customers/${customerId}/standing-orders/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  })

export const deleteStandingOrder = (customerId, id) =>
  apiFetch(`/customers/${customerId}/standing-orders/${id}`, { method: 'DELETE' })
