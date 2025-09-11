import React, { useState } from "react"
import { Link, useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

export default function RegisterPage() {
  const [form, setForm] = useState({
    email: "",
    firstName: "",
    lastName: "",
    password: "",
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const navigate = useNavigate()

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError("")

    if (!form.email || !form.firstName || !form.lastName || !form.password) {
      setError("Заполните все поля")
      return
    }

    if (form.password.length < 8) {
      setError("Пароль должен содержать не менее 8 символов")
      return
    }

    setLoading(true)
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/users/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(form),
      })

      if (!res.ok) {
        const text = await res.text()
        try {
          const json = JSON.parse(text)
          setError(json.message ?? json.detail ?? text ?? "Ошибка регистрации")
        } catch {
          setError(text || "Ошибка регистрации")
        }
        return
      }

      // Успех — идём на страницу входа
      await res.json().catch(() => {})
      navigate("/login")
    } catch {
      setError("Сервер недоступен. Попробуйте позже.")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex h-screen items-center justify-center">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Регистрация</CardTitle>
          <CardDescription>Заполните поля для создания аккаунта</CardDescription>
          <CardAction>
            <Link to="/login" className="text-sm underline">
              Войти
            </Link>
          </CardAction>
        </CardHeader>

        <form onSubmit={handleSubmit}>
          <CardContent>
            <div className="flex flex-col gap-6">
              <div className="grid gap-2">
                <Label htmlFor="firstName">Имя</Label>
                <Input
                  id="firstName"
                  value={form.firstName}
                  onChange={(e) => setForm({ ...form, firstName: e.target.value })}
                />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="lastName">Фамилия</Label>
                <Input
                  id="lastName"
                  value={form.lastName}
                  onChange={(e) => setForm({ ...form, lastName: e.target.value })}
                />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  value={form.email}
                  onChange={(e) => setForm({ ...form, email: e.target.value })}
                />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="password">Пароль</Label>
                <Input
                  id="password"
                  type="password"
                  value={form.password}
                  onChange={(e) => setForm({ ...form, password: e.target.value })}
                />
              </div>

              {error && <p className="text-sm text-red-500">{error}</p>}
            </div>
          </CardContent>

          <CardFooter className="flex-col gap-2 mt-4">
            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? "Регистрируем..." : "Зарегистрироваться"}
            </Button>
          </CardFooter>
        </form>
      </Card>
    </div>
  )
}
