import { Button } from "@/components/ui/button"
import { useNavigate } from "react-router-dom"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { Plus } from "lucide-react"
import { useCharacters } from "./useCharacters"
import { CharacterCard } from "./components/CharacterCard"

export default function CharactersPage() {
  const { data: characters, isLoading, error } = useCharacters()
  const navigate = useNavigate()

  return (
    <div className="flex flex-col gap-4 p-4">
      <div className="flex justify-end">
        <Button onClick={() => navigate("/characters/create")}>
          <Plus /> Создать персонажа
        </Button>
      </div>

      <LoaderState
        isLoading={isLoading}
        error={error}
        empty={<div className="text-muted-foreground">У вас пока нет персонажей.</div>}
        loadingFallback={
          <div className="grid gap-4 md:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <Skeleton key={i} className="h-40 rounded-2xl" />
            ))}
          </div>
        }
      >
        {characters && (
          <div className="grid gap-4 md:grid-cols-3">
            {characters.map((ch) => (
              <CharacterCard key={ch.id} id={ch.id} name={ch.name} />
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}
