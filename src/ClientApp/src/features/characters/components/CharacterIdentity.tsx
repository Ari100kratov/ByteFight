import { Badge } from "@/components/ui/badge"
import { cn } from "@/shared/lib/utils"

interface CharacterIdentityProps {
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
          "truncate font-semibold",
          size === "sm" && "text-sm",
          size === "md" && "text-base",
          size === "lg" && "text-xl",
        )}
      >
        {name ?? "—"}
      </div>

      {meta && variant === "text" && (
        <div
          className={cn(
            "text-muted-foreground truncate",
            size === "sm" && "text-xs",
            size === "md" && "text-sm",
            size === "lg" && "text-sm",
          )}
        >
          {meta}
        </div>
      )}

      {meta && variant === "badge" && (
        <Badge
          variant="secondary"
          className={cn(
            size === "sm" && "mt-1 px-2 py-0.5 text-xs",
            size === "md" && "mt-1.5 px-3 py-1 text-sm",
            size === "lg" && "mt-1.5 px-3 py-1 text-sm",
          )}
        >
          {meta}
        </Badge>
      )}
    </div>
  )
}
