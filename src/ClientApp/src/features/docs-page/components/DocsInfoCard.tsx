import type { ReactNode } from "react"

interface Props {
  title: string
  children: ReactNode
}

/**
 * Displays a short highlighted documentation note.
 */
export function DocsInfoCard({ title, children }: Props) {
  return (
    <div className="bg-background rounded-xl border p-4">
      <div className="font-semibold">{title}</div>

      <div className="text-muted-foreground [&_code]:bg-muted [&_code]:text-foreground mt-2 text-sm leading-6 [&_code]:rounded-md [&_code]:border [&_code]:px-1.5 [&_code]:py-0.5 [&_code]:font-mono">
        {children}
      </div>
    </div>
  )
}
