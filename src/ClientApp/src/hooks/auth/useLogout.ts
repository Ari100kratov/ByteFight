import { useMutation } from '@tanstack/react-query'
import { apiFetch } from '@/lib/apiFetch'

export default function useLogout() {
  return useMutation({
    mutationFn: async () => {
      try {
        await apiFetch('/users/logout', { method: 'POST' })
      } catch {
        // даже если запрос упал — всё равно чистим локальное хранилище
      }

      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      window.location.href = '/login'
    },
  })
}
