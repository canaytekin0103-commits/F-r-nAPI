import { apiFetch, fetchPagedItems } from './client'

export const getBreads = (page = 1, size = 100) => fetchPagedItems('/breads', page, size)
export const createBread = (data) =>
  apiFetch('/breads', { method: 'POST', body: JSON.stringify(data) })
