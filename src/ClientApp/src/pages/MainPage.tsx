import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"

export default function MainPage() {
  const navigate = useNavigate()

  function handleLogout() {
    localStorage.removeItem("token")
    navigate("/login")
  }

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Добро пожаловать!</h1>
      <Button onClick={handleLogout}>Выйти</Button>
    </div>
  )
}
