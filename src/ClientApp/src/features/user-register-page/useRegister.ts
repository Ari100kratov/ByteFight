import { useMutation } from '@tanstack/react-query'
import { apiFetch } from '@/shared/lib/apiFetch'

export interface RegisterRequest {
  email: string
  firstName: string
  lastName: string
  password: string
}

export default function useRegister() {
  return useMutation({
    mutationFn: async (data: RegisterRequest) => {
      if (!data.email?.trim() || !data.firstName?.trim() || !data.lastName?.trim() || !data.password?.trim()) {
        throw new Error("Все поля регистрации обязательны")
      }

      return apiFetch('/users/register', {
        method: 'POST',
        body: JSON.stringify(data),
      })
    },
  })
}
