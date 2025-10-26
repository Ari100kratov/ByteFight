import { useMutation } from '@tanstack/react-query'
import { apiFetch } from '@/shared/lib/apiFetch'
import { clearAuth } from '@/shared/lib/auth'

export default function useLogout() {
  return useMutation({
    mutationFn: async () => {
      try {
        await apiFetch('/users/logout', { method: 'POST' })
      } catch {
        // даже если запрос упал — всё равно чистим локальное хранилище
      }

      clearAuth();
      window.location.href = '/login'
    },
  })
}
