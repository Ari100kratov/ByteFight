import { useState } from "react"

export function useAuth() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function login(email: string, password: string) {
    if (!email.trim() || !password.trim()) {
      const msg = "Email и пароль обязательны"
      setError(msg)
      throw new Error(msg)
    }

    setLoading(true)
    setError(null)
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/users/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      })

      if (!res.ok) {
        let msg = "Ошибка входа"
        try {
          const json = await res.json()
          msg = json.message ?? json.detail ?? msg
        } catch {}
        throw new Error(msg)
      }

      const data = await res.json()
      localStorage.setItem("token", data.token)
      return data.token as string
    } catch (err) {
      setError(err instanceof Error ? err.message : "Неизвестная ошибка")
      throw err
    } finally {
      setLoading(false)
    }
  }

  async function register(form: {
    email: string
    firstName: string
    lastName: string
    password: string
  }) {
    if (!form.email.trim() || !form.firstName.trim() || !form.lastName.trim() || !form.password.trim()) {
      const msg = "Все поля регистрации обязательны"
      setError(msg)
      throw new Error(msg)
    }

    setLoading(true)
    setError(null)
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/users/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(form),
      })

      if (!res.ok) {
        let msg = "Ошибка регистрации"
        try {
          const json = await res.json()
          msg = json.message ?? json.detail ?? msg
        } catch {}
        throw new Error(msg)
      }

      return await res.json()
    } catch (err) {
      setError(err instanceof Error ? err.message : "Неизвестная ошибка")
      throw err
    } finally {
      setLoading(false)
    }
  }

  return { login, register, loading, error }
}
