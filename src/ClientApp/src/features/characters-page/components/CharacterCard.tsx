import { Card, CardContent } from "@/components/ui/card"
import { useNavigate } from "react-router-dom"
import { CharacterIdentity } from "@/features/characters/components/CharacterIdentity"
import { UserRound } from "lucide-react"
import { getAssetUrl } from "@/shared/api/loadActionAssets"

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
  const imageSrc = portraitUrl ? getAssetUrl(portraitUrl) : undefined

  return (
    <Card
      className="group min-h-72 cursor-pointer overflow-hidden rounded-2xl shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-md"
      onClick={() => navigate(`/characters/${id}`)}
    >
      <div className="relative aspect-square overflow-hidden bg-muted">
        {imageSrc ? (
          <>
            <img
              src={imageSrc}
              alt={name}
              loading="lazy"
              draggable={false}
              className="h-full w-full object-cover object-center will-change-transform transition-transform duration-700 ease-out group-hover:scale-[1.04]"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-black/5 to-transparent" />
          </>
        ) : (
          <div className="flex h-full items-center justify-center">
            <div className="flex h-24 w-24 items-center justify-center rounded-full border bg-background/70 text-muted-foreground">
              <UserRound className="h-12 w-12" />
            </div>
          </div>
        )}
      </div>

      <CardContent className="p-4">
        <CharacterIdentity
          name={name}
          className={className}
          specName={specName}
          variant="badge"
          size="lg"
        />
      </CardContent>
    </Card>
  )
}