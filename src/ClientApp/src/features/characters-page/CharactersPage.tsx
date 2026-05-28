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
        <Button
          onClick={() => {
            void navigate("/characters/create")
          }}
        >
          <Plus /> Создать персонажа
        </Button>
      </div>

      <LoaderState
        isLoading={isLoading}
        error={error}
        isEmpty={!characters || characters.length === 0}
        empty={<div className="text-muted-foreground text-center">У вас пока нет персонажей.</div>}
        loadingFallback={
          <div className="grid gap-4 md:grid-cols-3">
            {Array.from({ length: 3 }, (_, i) => (
              <Skeleton key={i} className="h-48 rounded-2xl" />
            ))}
          </div>
        }
      >
        {characters && (
          <div className="grid gap-4 md:grid-cols-4">
            {characters.map((c, index) => (
              <CharacterCard
                key={c.id}
                id={c.id}
                name={c.name}
                className={c.className}
                specName={c.specName}
                portraitUrl={c.portraitUrl}
                priority={index < 4}
              />
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}
