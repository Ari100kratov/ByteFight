import type { ReactNode } from "react"

type Props = {
  title: string
  children: ReactNode
}

/**
 * Displays a short highlighted documentation note.
 */
export function DocsInfoCard({ title, children }: Props) {
  return (
    <div className="rounded-xl border bg-background p-4">
      <div className="font-semibold">{title}</div>

      <div className="mt-2 text-sm leading-6 text-muted-foreground [&_code]:rounded-md [&_code]:border [&_code]:bg-muted [&_code]:px-1.5 [&_code]:py-0.5 [&_code]:font-mono [&_code]:text-foreground">
        {children}
      </div>
    </div>
  )
}