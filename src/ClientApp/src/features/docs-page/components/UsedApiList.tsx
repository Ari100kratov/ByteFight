type Props = {
  items: string[]
}

/**
 * Displays APIs used by a documentation recipe.
 */
export function UsedApiList({ items }: Props) {
  return (
    <div className="flex flex-wrap gap-2">
      {items.map((item) => (
        <code key={item} className="bg-muted/50 rounded-md border px-2 py-1 text-xs">
          {item}
        </code>
      ))}
    </div>
  )
}
