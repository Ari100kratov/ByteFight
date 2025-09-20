import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

type CharacterCardProps = {
  name: string
}

export function CharacterCard({ name }: CharacterCardProps) {
  return (
    <Card className="rounded-2xl shadow-sm hover:shadow-md transition">
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
