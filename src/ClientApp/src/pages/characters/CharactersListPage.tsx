import { CharacterCard } from "@/components/characters/CharacterCard"
import { Skeleton } from "@/components/ui/skeleton"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { useNavigate } from "react-router-dom"
import { useCharacters } from "@/hooks/characters/useCharacters"

export default function CharactersListPage() {
  const { data: characters, isLoading, error } = useCharacters()
  const navigate = useNavigate()

  return (
    <div className="flex flex-col gap-4 p-4">
      <div className="flex justify-end">
        <Button onClick={() => navigate("/characters/create")}>
          Создать персонажа
        </Button>
      </div>

      {isLoading && (
        <div className="grid gap-4 md:grid-cols-3">
          {[...Array(3)].map((_, i) => (
            <Skeleton key={i} className="h-40 rounded-2xl" />
          ))}
        </div>
      )}

      {error && (
        <Alert variant="destructive">
          <AlertTitle>Ошибка</AlertTitle>
          <AlertDescription>
            {error instanceof Error ? error.message : String(error)}
          </AlertDescription>
        </Alert>
      )}

      {!isLoading && !error && characters?.length === 0 && (
        <div className="text-muted-foreground">У вас пока нет персонажей.</div>
      )}

      {!isLoading && !error && (characters?.length ?? 0) > 0 && (
        <div className="grid gap-4 md:grid-cols-3">
          {characters?.map((ch) => (
            <CharacterCard key={ch.id} id={ch.id} name={ch.name} />
          ))}
        </div>
      )}
    </div>
  )
}
