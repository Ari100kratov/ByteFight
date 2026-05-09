type Props = {
  code: string
}

/**
 * Displays a C# code sample in documentation pages.
 */
export function DocsCodeBlock({ code }: Props) {
  return (
    <pre className="overflow-x-auto rounded-xl border bg-muted/40 p-4 text-sm leading-6">
      <code>{code}</code>
    </pre>
  )
}