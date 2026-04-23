import React, { useState } from "react"
import { Link, useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Spinner } from "@/components/ui/spinner"
import useLogin from "./useLogin"

export default function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const navigate = useNavigate()

  const { mutate: login, isPending, error } = useLogin()

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()

    login(
      { email, password },
      {
        onSuccess: () => {
          navigate("/")
        },
      }
    )
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/30 px-4 py-8">
      <div className="w-full max-w-5xl overflow-hidden rounded-3xl border bg-card shadow-sm transition-all duration-300 hover:shadow-lg">
        <div className="grid min-h-[520px] lg:grid-cols-2">
          <div className="flex items-center justify-center p-6 sm:p-8 lg:p-10">
            <div className="w-full max-w-sm">
              <div className="mb-8 text-center lg:hidden">
                <img
                  src="/logo.png"
                  alt="ByteFight logo"
                  className="mx-auto h-24 w-24 object-contain"
                />
                <h1 className="mt-4 text-2xl font-bold tracking-tight">ByteFight</h1>
                <p className="mt-2 text-sm text-muted-foreground">
                  Программируй. Сражайся. Побеждай.
                </p>
              </div>

              <Card className="border-0 shadow-none">
                <CardHeader className="px-0 pt-0">
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <CardTitle className="text-2xl">Вход в аккаунт</CardTitle>
                      <CardDescription className="mt-2">
                        Введите email и пароль
                      </CardDescription>
                    </div>

                    <Link
                      to="/register"
                      className="text-sm underline underline-offset-4"
                    >
                      Регистрация
                    </Link>
                  </div>
                </CardHeader>

                <form onSubmit={handleSubmit}>
                  <CardContent className="px-0">
                    <div className="flex flex-col gap-4">
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

                      {error && (
                        <p className="text-sm text-red-500">
                          {error instanceof Error ? error.message : String(error)}
                        </p>
                      )}
                    </div>
                  </CardContent>

                  <CardFooter className="mt-4 flex-col gap-2 px-0 pb-0">
                    <Button
                      type="submit"
                      className="w-full"
                      disabled={isPending}
                    >
                      {isPending ? (
                        <>
                          <Spinner /> Вхожу...
                        </>
                      ) : (
                        "Войти"
                      )}
                    </Button>
                  </CardFooter>
                </form>
              </Card>
            </div>
          </div>

          <div className="relative hidden border-l bg-muted/40 lg:flex items-center justify-center overflow-hidden p-10">
            <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-transparent" />

            <div className="relative z-10 max-w-sm text-center">
              <img
                src="/logo.png"
                alt="ByteFight logo"
                className="mx-auto h-64 w-64 object-contain"
              />
              <h2 className="mt-6 text-4xl font-bold tracking-tight">ByteFight</h2>
              <p className="mt-3 text-base text-muted-foreground">
                Программируй. Сражайся. Побеждай.
              </p>
              <p className="mt-6 text-sm leading-6 text-muted-foreground">
                Создавай боевую логику персонажа, тестируй стратегии и наблюдай,
                как твой код оживает на арене.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}