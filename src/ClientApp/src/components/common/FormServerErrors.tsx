import { getApiErrorMessages } from "@/shared/lib/apiErrors"

type Props = {
  error: unknown
}

export function FormServerErrors({ error }: Props) {
  const messages = getApiErrorMessages(error)

  if (messages.length === 0) return null

  return (
    <div className="border-destructive/30 bg-destructive/5 text-destructive rounded-md border px-3 py-2 text-sm">
      {messages.length === 1 ? (
        <p>{messages[0]}</p>
      ) : (
        <ul className="list-disc space-y-1 pl-4">
          {messages.map((message, index) => (
            <li key={index}>{message}</li>
          ))}
        </ul>
      )}
    </div>
  )
}
