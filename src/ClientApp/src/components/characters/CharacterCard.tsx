import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { useNavigate } from "react-router-dom"

type CharacterCardProps = {
  id: string
  name: string
}

export function CharacterCard({ id, name }: CharacterCardProps) {
  const navigate = useNavigate()

  return (
    <Card
      className="rounded-2xl shadow-sm hover:shadow-md transition"
      onClick={() => navigate(`/characters/${id}`)}
    >
      <CardHeader>
        <CardTitle className="text-lg font-semibold">{name}</CardTitle>
      </CardHeader>
      <CardContent>
        {/* здесь потом добавим аватар, класс персонажа и т.п. */}
        <p className="text-muted-foreground">Пока только имя</p>
      </CardContent>
    </Card>
  )
}
