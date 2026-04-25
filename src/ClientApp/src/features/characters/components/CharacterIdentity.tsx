import { Badge } from "@/components/ui/badge"
import { cn } from "@/shared/lib/utils"

type CharacterIdentityProps = {
  name?: string | null
  className?: string | null
  specName?: string | null
  variant?: "text" | "badge"
  size?: "sm" | "md" | "lg"
}

export function CharacterIdentity({
  name,
  className,
  specName,
  variant = "text",
  size = "md",
}: CharacterIdentityProps) {
  const meta = [className, specName].filter(Boolean).join(" · ")

  return (
    <div className="min-w-0">
      <div
        className={cn(
          "truncate  font-semibold",
          size === "sm" && "text-sm",
          size === "md" && "text-base",
          size === "lg" && "text-xl"
        )}
      >
        {name ?? "—"}
      </div>

      {meta && variant === "text" && (
        <div
          className={cn(
            "truncate text-muted-foreground",
            size === "sm" && "text-xs",
            size === "md" && "text-sm",
            size === "lg" && "text-sm"
          )}
        >
          {meta}
        </div>
      )}

      {meta && variant === "badge" && (
        <Badge variant="secondary" className="mt-1">
          {meta}
        </Badge>
      )}
    </div>
  )
}