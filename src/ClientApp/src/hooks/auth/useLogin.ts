import { useMutation } from '@tanstack/react-query'
import { apiFetch } from '@/lib/apiFetch'

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  token: string
}

export default function useLogin() {
  return useMutation({
    mutationFn: async (data: LoginRequest): Promise<string> => {
      if (!data.email?.trim() || !data.password?.trim()) {
        throw new Error("Email и пароль обязательны")
      }

      const res = await apiFetch<LoginResponse>('/users/login', {
        method: 'POST',
        body: JSON.stringify(data),
      })

      localStorage.setItem('token', res.token)
      return res.token
    },
  })
}
