import { Card, CardContent } from "@/components/ui/card"
import { useNavigate } from "react-router-dom"
import { CharacterIdentity } from "@/features/characters/components/CharacterIdentity"
import { UserRound } from "lucide-react"
import { getAssetUrl } from "@/shared/api/loadActionAssets"

interface CharacterCardProps {
  id: string
  name: string
  className: string
  specName: string
  portraitUrl?: string | null
  priority?: boolean
}

export function CharacterCard({
  id,
  name,
  className,
  specName,
  portraitUrl,
  priority = false,
}: CharacterCardProps) {
  const navigate = useNavigate()
  const imageSrc = portraitUrl ? getAssetUrl(portraitUrl) : undefined

  return (
    <Card
      className="group min-h-72 cursor-pointer overflow-hidden rounded-2xl shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-md"
      onClick={() => {
        void navigate(`/characters/${id}`)
      }}
    >
      <div className="bg-muted relative aspect-square overflow-hidden">
        {imageSrc ? (
          <>
            <img
              src={imageSrc}
              alt={name}
              loading={priority ? "eager" : "lazy"}
              decoding="async"
              fetchPriority={priority ? "high" : "auto"}
              draggable={false}
              className="h-full w-full object-cover object-center transition-transform duration-700 ease-out will-change-transform group-hover:scale-[1.04]"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-black/5 to-transparent" />
          </>
        ) : (
          <div className="flex h-full items-center justify-center">
            <div className="bg-background/70 text-muted-foreground flex h-24 w-24 items-center justify-center rounded-full border">
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
