import { useMutation } from '@tanstack/react-query'
import { apiFetch } from '@/shared/lib/apiFetch'
import { saveAuthTokens } from '@/shared/lib/auth'

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
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

      saveAuthTokens(res.accessToken, res.refreshToken)

      return res.accessToken
    },
  })
}
