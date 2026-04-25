import { Card, CardContent } from "@/components/ui/card"
import { useNavigate } from "react-router-dom"
import { CharacterIdentity } from "@/features/characters/components/CharacterIdentity"
import { UserRound } from "lucide-react"

type CharacterCardProps = {
  id: string
  name: string
  className: string
  specName: string
  portraitUrl?: string | null
}

export function CharacterCard({
  id,
  name,
  className,
  specName,
  portraitUrl,
}: CharacterCardProps) {
  const navigate = useNavigate()

  return (
    <Card
      className="min-h-72 cursor-pointer overflow-hidden rounded-2xl shadow-sm transition hover:shadow-md"
      onClick={() => navigate(`/characters/${id}`)}
    >
      <div className="flex h-44 items-center justify-center bg-muted/40">
        {portraitUrl ? (
          <img
            src={portraitUrl}
            alt={name}
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="flex h-24 w-24 items-center justify-center rounded-full border bg-background/70 text-muted-foreground">
            <UserRound className="h-12 w-12" />
          </div>
        )}
      </div>

      <CardContent className="p-4">
        <CharacterIdentity
          name={name}
          className={className}
          specName={specName}
          variant="text"
          size="lg"
        />
      </CardContent>
    </Card>
  )
}