import React, { useState } from "react"
import { Link, useNavigate } from "react-router-dom"
import { Eye, EyeOff } from "lucide-react"
import { toast } from "sonner"
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
import { FormServerErrors } from "@/components/common/FormServerErrors"
import { getApiErrorToastMessage } from "@/shared/lib/apiErrors"
import {
  EMAIL_REGEX,
  MAX_EMAIL_LENGTH,
  MAX_FIRST_NAME_LENGTH,
  MAX_LAST_NAME_LENGTH,
  MAX_PASSWORD_LENGTH,
  MIN_PASSWORD_LENGTH,
} from "@/shared/lib/validation"
import useRegister from "./useRegister"

type RegisterErrors = Partial<
  Record<"email" | "firstName" | "lastName" | "password" | "confirmPassword", string>
>

export default function RegisterPage() {
  const [form, setForm] = useState({
    email: "",
    firstName: "",
    lastName: "",
    password: "",
    confirmPassword: "",
  })

  const [errors, setErrors] = useState<RegisterErrors>({})
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)

  const navigate = useNavigate()
  const { mutate: register, isPending, error } = useRegister()

  function validate(): RegisterErrors {
    const nextErrors: RegisterErrors = {}

    if (!form.firstName.trim()) {
      nextErrors.firstName = "Введите имя"
    } else if (form.firstName.length > MAX_FIRST_NAME_LENGTH) {
      nextErrors.firstName = `Имя должно быть не длиннее ${MAX_FIRST_NAME_LENGTH} символов`
    }

    if (!form.lastName.trim()) {
      nextErrors.lastName = "Введите фамилию"
    } else if (form.lastName.length > MAX_LAST_NAME_LENGTH) {
      nextErrors.lastName = `Фамилия должна быть не длиннее ${MAX_LAST_NAME_LENGTH} символов`
    }

    if (!form.email.trim()) {
      nextErrors.email = "Введите email"
    } else if (form.email.length > MAX_EMAIL_LENGTH) {
      nextErrors.email = `Email должен быть не длиннее ${MAX_EMAIL_LENGTH} символов`
    } else if (!EMAIL_REGEX.test(form.email)) {
      nextErrors.email = "Введите корректный email"
    }

    if (!form.password) {
      nextErrors.password = "Введите пароль"
    } else if (form.password.length < MIN_PASSWORD_LENGTH) {
      nextErrors.password = `Пароль должен быть не короче ${MIN_PASSWORD_LENGTH} символов`
    } else if (form.password.length > MAX_PASSWORD_LENGTH) {
      nextErrors.password = `Пароль должен быть не длиннее ${MAX_PASSWORD_LENGTH} символов`
    }

    if (!form.confirmPassword) {
      nextErrors.confirmPassword = "Повторите пароль"
    } else if (form.password !== form.confirmPassword) {
      nextErrors.confirmPassword = "Пароли не совпадают"
    }

    return nextErrors
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()

    const nextErrors = validate()
    setErrors(nextErrors)

    if (Object.keys(nextErrors).length > 0) return

    register(
      {
        email: form.email,
        firstName: form.firstName,
        lastName: form.lastName,
        password: form.password,
      },
      {
        onSuccess: () => {
          toast.success("Аккаунт создан")
          navigate("/login")
        },
        onError: (error) => {
          toast.error(getApiErrorToastMessage(error))
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
                <h1 className="mt-4 text-2xl font-bold tracking-tight">
                  ByteFight
                </h1>
                <p className="mt-2 text-sm text-muted-foreground">
                  Программируй. Сражайся. Побеждай.
                </p>
              </div>

              <Card className="border-0 shadow-none">
                <CardHeader className="px-0 pt-0">
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <CardTitle className="text-2xl">Регистрация</CardTitle>
                      <CardDescription className="mt-2">
                        Заполните поля для создания аккаунта
                      </CardDescription>
                    </div>

                    <Link
                      to="/login"
                      className="text-sm underline underline-offset-4"
                    >
                      Войти
                    </Link>
                  </div>
                </CardHeader>

                <form onSubmit={handleSubmit}>
                  <CardContent className="px-0">
                    <div className="flex flex-col gap-3">
                      <div className="grid gap-2">
                        <Label htmlFor="firstName">Имя</Label>
                        <Input
                          id="firstName"
                          value={form.firstName}
                          maxLength={MAX_FIRST_NAME_LENGTH + 1}
                          aria-invalid={!!errors.firstName}
                          onChange={(e) => {
                            setForm({ ...form, firstName: e.target.value })
                            setErrors({ ...errors, firstName: undefined })
                          }}
                        />
                        {errors.firstName && (
                          <p className="text-sm text-destructive">
                            {errors.firstName}
                          </p>
                        )}
                      </div>

                      <div className="grid gap-2">
                        <Label htmlFor="lastName">Фамилия</Label>
                        <Input
                          id="lastName"
                          value={form.lastName}
                          maxLength={MAX_LAST_NAME_LENGTH + 1}
                          aria-invalid={!!errors.lastName}
                          onChange={(e) => {
                            setForm({ ...form, lastName: e.target.value })
                            setErrors({ ...errors, lastName: undefined })
                          }}
                        />
                        {errors.lastName && (
                          <p className="text-sm text-destructive">
                            {errors.lastName}
                          </p>
                        )}
                      </div>

                      <div className="grid gap-2">
                        <Label htmlFor="email">Email</Label>
                        <Input
                          id="email"
                          value={form.email}
                          maxLength={MAX_EMAIL_LENGTH + 1}
                          aria-invalid={!!errors.email}
                          onChange={(e) => {
                            setForm({ ...form, email: e.target.value })
                            setErrors({ ...errors, email: undefined })
                          }}
                        />
                        {errors.email && (
                          <p className="text-sm text-destructive">
                            {errors.email}
                          </p>
                        )}
                      </div>

                      <div className="grid gap-2">
                        <Label htmlFor="password">Пароль</Label>
                        <div className="relative">
                          <Input
                            id="password"
                            type={showPassword ? "text" : "password"}
                            value={form.password}
                            maxLength={MAX_PASSWORD_LENGTH + 1}
                            aria-invalid={!!errors.password}
                            className="pr-10"
                            onChange={(e) => {
                              setForm({ ...form, password: e.target.value })
                              setErrors({ ...errors, password: undefined })
                            }}
                          />

                          <Button
                            type="button"
                            variant="ghost"
                            size="icon"
                            className="absolute right-0 top-0 h-full px-3"
                            onClick={() => setShowPassword((value) => !value)}
                            aria-label={
                              showPassword ? "Скрыть пароль" : "Показать пароль"
                            }
                          >
                            {showPassword ? (
                              <EyeOff className="size-4" />
                            ) : (
                              <Eye className="size-4" />
                            )}
                          </Button>
                        </div>
                        {errors.password && (
                          <p className="text-sm text-destructive">
                            {errors.password}
                          </p>
                        )}
                      </div>

                      <div className="grid gap-2">
                        <Label htmlFor="confirmPassword">Повторите пароль</Label>
                        <div className="relative">
                          <Input
                            id="confirmPassword"
                            type={showConfirmPassword ? "text" : "password"}
                            value={form.confirmPassword}
                            maxLength={MAX_PASSWORD_LENGTH + 1}
                            aria-invalid={!!errors.confirmPassword}
                            className="pr-10"
                            onChange={(e) => {
                              setForm({
                                ...form,
                                confirmPassword: e.target.value,
                              })
                              setErrors({
                                ...errors,
                                confirmPassword: undefined,
                              })
                            }}
                          />

                          <Button
                            type="button"
                            variant="ghost"
                            size="icon"
                            className="absolute right-0 top-0 h-full px-3"
                            onClick={() =>
                              setShowConfirmPassword((value) => !value)
                            }
                            aria-label={
                              showConfirmPassword
                                ? "Скрыть повтор пароля"
                                : "Показать повтор пароля"
                            }
                          >
                            {showConfirmPassword ? (
                              <EyeOff className="size-4" />
                            ) : (
                              <Eye className="size-4" />
                            )}
                          </Button>
                        </div>
                        {errors.confirmPassword && (
                          <p className="text-sm text-destructive">
                            {errors.confirmPassword}
                          </p>
                        )}
                      </div>

                      <FormServerErrors error={error} />
                    </div>
                  </CardContent>

                  <CardFooter className="mt-4 flex-col gap-2 px-0 pb-0">
                    <Button type="submit" className="w-full" disabled={isPending}>
                      {isPending ? (
                        <>
                          <Spinner /> Регистрируем...
                        </>
                      ) : (
                        "Зарегистрироваться"
                      )}
                    </Button>
                  </CardFooter>
                </form>
              </Card>
            </div>
          </div>

          <div className="relative hidden items-center justify-center overflow-hidden border-l bg-muted/40 p-10 lg:flex">
            <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-transparent" />

            <div className="relative z-10 max-w-sm text-center">
              <img
                src="/logo.png"
                alt="ByteFight logo"
                className="mx-auto h-64 w-64 object-contain"
              />
              <h2 className="mt-6 text-4xl font-bold tracking-tight">
                ByteFight
              </h2>
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