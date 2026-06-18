const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5220/api'

export async function apiFetch(path, options = {}) {
  const token = localStorage.getItem('token')
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  }

  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers,
  })

  if (!response.ok) {
    let message = 'Bir hata oluştu.'
    try {
      const data = await response.json()
      message = data.error || data.title || JSON.stringify(data.errors) || message
    } catch {
      message = response.statusText
    }
    throw new Error(message)
  }

  if (response.status === 204) return null
  return response.json()
}

/** Sayfalanmış API yanıtından items dizisini döner */
export async function fetchPagedItems(path, page = 1, size = 100) {
  const data = await apiFetch(`${path}?page=${page}&size=${size}`)
  return data.items ?? data
}
