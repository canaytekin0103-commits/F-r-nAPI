import { fetchPagedItems } from './client'

export const getCakes = (page = 1, size = 100) => fetchPagedItems('/cakes', page, size)
