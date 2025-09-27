import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

type CharacterCodeBlockProps = {
  characterId: string
}

export default function CharacterCodeBlock({ characterId }: CharacterCodeBlockProps) {

  const handleSave = () => {
    // TODO: запрос к API для сохранения
  }

  return (
    <Card className="md:col-span-2 flex flex-col w-full">
      <CardHeader>
        <CardTitle>Код персонажа</CardTitle>
        <CardDescription>
          Здесь можно редактировать поведение персонажа
        </CardDescription>
      </CardHeader>
      <CardContent className="flex-1">
        <textarea
          className="w-full h-64 border rounded-md p-2 font-mono text-sm resize-none"
        />
      </CardContent>
      <CardFooter className="justify-end">
        <Button onClick={handleSave}>Сохранить код</Button>
      </CardFooter>
    </Card>
  )
}
