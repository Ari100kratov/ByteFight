import { Card, CardHeader, CardTitle, CardContent, CardDescription } from "@/components/ui/card"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { useCharacters, type Character } from "@/features/characters-page/useCharacters"

type Props = {
  selectedCharacter?: Character
  onSelect: (character: Character) => void
}

export function SelectCharacterCard({ selectedCharacter, onSelect }: Props) {
  const { data: characters, isLoading, error } = useCharacters()

  const handleCreateClick = () => {
    window.open("/characters/create", "_blank")
  }

  const hasCharacters = characters && characters.length > 0

  return (
    <LoaderState
      isLoading={isLoading}
      error={error}
      skeletonClassName="w-full h-full rounded-2xl"
      loadingFallback={<Skeleton className="w-full h-full rounded-2xl" />}
    >
      <Card className="h-full rounded-none md:rounded-tl-2xl border-0 border-b md:border-b-0 md:border-r">
        <CardHeader>
          <CardTitle>Персонаж</CardTitle>
          <CardDescription>
            {hasCharacters && (
              <Select
                value={selectedCharacter?.id}
                onValueChange={(id) => {
                  const char = characters.find((c) => c.id === id)
                  if (char) onSelect(char)
                }}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Выберите персонажа" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectLabel>Ваши персонажи</SelectLabel>
                    {characters.map((char) => (
                      <SelectItem key={char.id} value={char.id}>
                        {char.name}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
            )}
          </CardDescription>
        </CardHeader>

        <CardContent className="flex flex-col gap-4">
          {!hasCharacters && (
            <div className="flex flex-1 items-center justify-center py-8">
              <Button onClick={handleCreateClick} className="gap-2">
                <Plus className="w-4 h-4" /> Создать персонажа
              </Button>
            </div>
          )}

          {selectedCharacter && (
            <div className="flex items-center gap-4 p-2 border rounded-md bg-muted">
              <div className="w-12 h-12 bg-gray-300 rounded-full flex items-center justify-center text-white">
                {selectedCharacter.name[0]?.toUpperCase()}
              </div>
              <div className="text-sm font-medium">{selectedCharacter.name}</div>
            </div>
          )}
        </CardContent>
      </Card>
    </LoaderState>
  )
}
