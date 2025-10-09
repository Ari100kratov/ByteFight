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
import { useCharacters, type Character } from "@/features/characters/useCharacters"

type Props = {
  selectedCharacter?: Character
  onSelect: (character: Character) => void
}

export function SelectCharacterCard({ selectedCharacter, onSelect }: Props) {
  const { data: characters, isLoading, error } = useCharacters()

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
            {/* Селект */}
            {characters?.length ? (
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
            ) : (
              <>Персонажи отсутствуют</>
            )}
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          {/* Информация о выбранном персонаже */}
          {selectedCharacter && (
            <div className="flex items-center gap-4 p-2 border rounded-md bg-muted">
              {/* Аватар-заглушка */}
              <div className="w-12 h-12 bg-gray-300 rounded-full flex items-center justify-center text-white">
                {/* Можно вставить иконку или инициалы */}
                {selectedCharacter.name[0]?.toUpperCase()}
              </div>
              {/* Имя персонажа */}
              <div className="text-sm font-medium">{selectedCharacter.name}</div>
            </div>
          )}
        </CardContent>
      </Card>
    </LoaderState>
  )
}
