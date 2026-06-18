import { apiFetch, fetchPagedItems } from './client'

export const getCustomers = (page = 1, size = 100) => fetchPagedItems('/customers', page, size)
export const createCustomer = (data) =>
  apiFetch('/customers', { method: 'POST', body: JSON.stringify(data) })
