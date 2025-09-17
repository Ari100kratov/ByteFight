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

export default function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const navigate = useNavigate()

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError("")

    if (!email || !password) {
      setError("Заполните все поля")
      return
    }

    setLoading(true)
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/users/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      })

      if (!res.ok) {
        const text = await res.text()
        try {
          const json = JSON.parse(text)
          setError(json.message ?? json.detail ?? text ?? "Ошибка входа")
        } catch {
          setError(text || "Ошибка входа")
        }
        return
      }

      // API возвращает строковый токен
      const token = await res.text()
      localStorage.setItem("token", token)
      navigate("/main")
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
          <CardTitle>Вход в аккаунт</CardTitle>
          <CardDescription>Введите email и пароль</CardDescription>
          <CardAction>
            <Link to="/register" className="text-sm underline">
              Зарегистрироваться
            </Link>
          </CardAction>
        </CardHeader>

        <form onSubmit={handleSubmit}>
          <CardContent>
            <div className="flex flex-col gap-6">
              <div className="grid gap-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="password">Пароль</Label>
                <Input
                  id="password"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>

              {error && <p className="text-sm text-red-500">{error}</p>}
            </div>
          </CardContent>

          <CardFooter className="flex-col gap-2 mt-4">
            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? "Вхожу..." : "Войти"}
            </Button>
          </CardFooter>
        </form>
      </Card>
    </div>
  )
}
