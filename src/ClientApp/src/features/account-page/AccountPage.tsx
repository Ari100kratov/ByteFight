import { useEffect, useState } from "react"
import { Eye, EyeOff } from "lucide-react"
import { toast } from "sonner"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
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
import { useUpdateProfile } from "./hooks/useUpdateProfile"
import { useChangePassword } from "./hooks/useChangePassword"
import { useCurrentUser } from "../nav-user-menu-item/hooks/useCurrentUser"

type ProfileErrors = Partial<Record<"email" | "firstName" | "lastName", string>>

type PasswordErrors = Partial<Record<"currentPassword" | "newPassword" | "confirmPassword", string>>

export default function AccountPage() {
  const { data: user } = useCurrentUser()
  const updateProfile = useUpdateProfile()
  const changePassword = useChangePassword()

  const [showNewPassword, setShowNewPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)

  const [profileForm, setProfileForm] = useState({
    email: "",
    firstName: "",
    lastName: "",
  })

  const [passwordForm, setPasswordForm] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  })

  const [profileErrors, setProfileErrors] = useState<ProfileErrors>({})
  const [passwordErrors, setPasswordErrors] = useState<PasswordErrors>({})

  useEffect(() => {
    if (!user) return

    setProfileForm({
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
    })
  }, [user])

  function validateProfile(): ProfileErrors {
    const errors: ProfileErrors = {}

    if (!profileForm.firstName.trim()) {
      errors.firstName = "Введите имя"
    } else if (profileForm.firstName.length > MAX_FIRST_NAME_LENGTH) {
      errors.firstName = `Имя должно быть не длиннее ${MAX_FIRST_NAME_LENGTH} символов`
    }

    if (!profileForm.lastName.trim()) {
      errors.lastName = "Введите фамилию"
    } else if (profileForm.lastName.length > MAX_LAST_NAME_LENGTH) {
      errors.lastName = `Фамилия должна быть не длиннее ${MAX_LAST_NAME_LENGTH} символов`
    }

    if (!profileForm.email.trim()) {
      errors.email = "Введите email"
    } else if (profileForm.email.length > MAX_EMAIL_LENGTH) {
      errors.email = `Email должен быть не длиннее ${MAX_EMAIL_LENGTH} символов`
    } else if (!EMAIL_REGEX.test(profileForm.email)) {
      errors.email = "Введите корректный email"
    }

    return errors
  }

  function validatePassword(): PasswordErrors {
    const errors: PasswordErrors = {}

    if (!passwordForm.currentPassword.trim()) {
      errors.currentPassword = "Введите текущий пароль"
    }

    if (!passwordForm.newPassword) {
      errors.newPassword = "Введите новый пароль"
    } else if (passwordForm.newPassword.length < MIN_PASSWORD_LENGTH) {
      errors.newPassword = `Пароль должен быть не короче ${MIN_PASSWORD_LENGTH} символов`
    } else if (passwordForm.newPassword.length > MAX_PASSWORD_LENGTH) {
      errors.newPassword = `Пароль должен быть не длиннее ${MAX_PASSWORD_LENGTH} символов`
    }

    if (!passwordForm.confirmPassword) {
      errors.confirmPassword = "Повторите новый пароль"
    } else if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      errors.confirmPassword = "Пароли не совпадают"
    }

    return errors
  }

  function submitProfile(e: React.SyntheticEvent<HTMLFormElement>) {
    e.preventDefault()

    const errors = validateProfile()
    setProfileErrors(errors)

    if (Object.keys(errors).length > 0) return

    updateProfile.mutate(profileForm, {
      onSuccess: () => {
        toast.success("Профиль обновлён")
      },
      onError: (error) => {
        toast.error(getApiErrorToastMessage(error))
      },
    })
  }

  function submitPassword(e: React.SyntheticEvent<HTMLFormElement>) {
    e.preventDefault()

    const errors = validatePassword()
    setPasswordErrors(errors)

    if (Object.keys(errors).length > 0) return

    changePassword.mutate(
      {
        currentPassword: passwordForm.currentPassword,
        newPassword: passwordForm.newPassword,
      },
      {
        onSuccess: () => {
          setPasswordForm({
            currentPassword: "",
            newPassword: "",
            confirmPassword: "",
          })
          setPasswordErrors({})
          setShowNewPassword(false)
          setShowConfirmPassword(false)

          toast.success("Пароль изменён")
        },
        onError: (error) => {
          toast.error(getApiErrorToastMessage(error))
        },
      },
    )
  }

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-col gap-6 pt-4 pb-8">
      <Card>
        <CardHeader>
          <CardTitle>Аккаунт</CardTitle>
          <CardDescription>Измените основную информацию профиля</CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={submitProfile} className="flex flex-col gap-3">
            <div className="grid gap-2">
              <Label htmlFor="firstName">Имя</Label>
              <Input
                id="firstName"
                value={profileForm.firstName}
                maxLength={MAX_FIRST_NAME_LENGTH + 1}
                aria-invalid={!!profileErrors.firstName}
                onChange={(e) => {
                  setProfileForm({
                    ...profileForm,
                    firstName: e.target.value,
                  })
                  setProfileErrors({
                    ...profileErrors,
                    firstName: undefined,
                  })
                }}
              />
              {profileErrors.firstName && (
                <p className="text-destructive text-sm">{profileErrors.firstName}</p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor="lastName">Фамилия</Label>
              <Input
                id="lastName"
                value={profileForm.lastName}
                maxLength={MAX_LAST_NAME_LENGTH + 1}
                aria-invalid={!!profileErrors.lastName}
                onChange={(e) => {
                  setProfileForm({
                    ...profileForm,
                    lastName: e.target.value,
                  })
                  setProfileErrors({
                    ...profileErrors,
                    lastName: undefined,
                  })
                }}
              />
              {profileErrors.lastName && (
                <p className="text-destructive text-sm">{profileErrors.lastName}</p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                value={profileForm.email}
                maxLength={MAX_EMAIL_LENGTH + 1}
                aria-invalid={!!profileErrors.email}
                onChange={(e) => {
                  setProfileForm({
                    ...profileForm,
                    email: e.target.value,
                  })
                  setProfileErrors({
                    ...profileErrors,
                    email: undefined,
                  })
                }}
              />
              {profileErrors.email && (
                <p className="text-destructive text-sm">{profileErrors.email}</p>
              )}
            </div>

            <FormServerErrors error={updateProfile.error} />

            <Button type="submit" disabled={updateProfile.isPending}>
              {updateProfile.isPending ? (
                <>
                  <Spinner /> Сохраняем...
                </>
              ) : (
                "Сохранить"
              )}
            </Button>
          </form>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Пароль</CardTitle>
          <CardDescription>Измените пароль для входа в аккаунт</CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={submitPassword} className="flex flex-col gap-3">
            <div className="grid gap-2">
              <Label htmlFor="currentPassword">Текущий пароль</Label>
              <Input
                id="currentPassword"
                type="password"
                value={passwordForm.currentPassword}
                aria-invalid={!!passwordErrors.currentPassword}
                onChange={(e) => {
                  setPasswordForm({
                    ...passwordForm,
                    currentPassword: e.target.value,
                  })
                  setPasswordErrors({
                    ...passwordErrors,
                    currentPassword: undefined,
                  })
                }}
              />
              {passwordErrors.currentPassword && (
                <p className="text-destructive text-sm">{passwordErrors.currentPassword}</p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor="newPassword">Новый пароль</Label>
              <div className="relative">
                <Input
                  id="newPassword"
                  type={showNewPassword ? "text" : "password"}
                  value={passwordForm.newPassword}
                  maxLength={MAX_PASSWORD_LENGTH + 1}
                  aria-invalid={!!passwordErrors.newPassword}
                  className="pr-10"
                  onChange={(e) => {
                    setPasswordForm({
                      ...passwordForm,
                      newPassword: e.target.value,
                    })
                    setPasswordErrors({
                      ...passwordErrors,
                      newPassword: undefined,
                    })
                  }}
                />

                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="absolute top-0 right-0 h-full px-3"
                  onClick={() => {
                    setShowNewPassword((value) => !value)
                  }}
                  aria-label={showNewPassword ? "Скрыть новый пароль" : "Показать новый пароль"}
                >
                  {showNewPassword ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
                </Button>
              </div>
              {passwordErrors.newPassword && (
                <p className="text-destructive text-sm">{passwordErrors.newPassword}</p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor="confirmPassword">Повторите новый пароль</Label>
              <div className="relative">
                <Input
                  id="confirmPassword"
                  type={showConfirmPassword ? "text" : "password"}
                  value={passwordForm.confirmPassword}
                  maxLength={MAX_PASSWORD_LENGTH + 1}
                  aria-invalid={!!passwordErrors.confirmPassword}
                  className="pr-10"
                  onChange={(e) => {
                    setPasswordForm({
                      ...passwordForm,
                      confirmPassword: e.target.value,
                    })
                    setPasswordErrors({
                      ...passwordErrors,
                      confirmPassword: undefined,
                    })
                  }}
                />

                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="absolute top-0 right-0 h-full px-3"
                  onClick={() => {
                    setShowConfirmPassword((value) => !value)
                  }}
                  aria-label={
                    showConfirmPassword
                      ? "Скрыть повтор нового пароля"
                      : "Показать повтор нового пароля"
                  }
                >
                  {showConfirmPassword ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
                </Button>
              </div>
              {passwordErrors.confirmPassword && (
                <p className="text-destructive text-sm">{passwordErrors.confirmPassword}</p>
              )}
            </div>

            <FormServerErrors error={changePassword.error} />

            <Button type="submit" disabled={changePassword.isPending}>
              {changePassword.isPending ? (
                <>
                  <Spinner /> Меняем...
                </>
              ) : (
                "Изменить пароль"
              )}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
