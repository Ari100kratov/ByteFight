interface Props {
  code: string
}

/**
 * Displays a C# code sample in documentation pages.
 */
export function DocsCodeBlock({ code }: Props) {
  return (
    <pre className="bg-muted/40 overflow-x-auto rounded-xl border p-4 text-sm leading-6">
      <code>{code}</code>
    </pre>
  )
}
