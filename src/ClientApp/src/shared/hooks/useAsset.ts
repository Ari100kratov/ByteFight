import { useQuery } from "@tanstack/react-query"
import { apiUrl } from "@/shared/config/api"

export function useAssetBlob(assetKey: string | undefined) {
  return useQuery<Blob>({
    queryKey: ["asset", assetKey],
    queryFn: async () => {
      if (!assetKey) {
        throw new Error("Не указан ключ ассета")
      }

      const res = await fetch(apiUrl(`/assets/${encodeURIComponent(assetKey)}`))
      if (!res.ok) {
        let msg = "Ошибка при загрузке ассета"
        try {
          const json: unknown = await res.json()
          msg = getAssetErrorMessage(json, msg)
        } catch {
          // Keep the default message when the error response is not JSON.
        }
        throw new Error(msg)
      }

      return await res.blob()
    },
    enabled: !!assetKey,
  })
}

function getAssetErrorMessage(value: unknown, fallback: string) {
  if (!isRecord(value)) {
    return fallback
  }

  return getOptionalString(value, "message") ?? getOptionalString(value, "detail") ?? fallback
}

function getOptionalString(record: Record<string, unknown>, key: string) {
  const value = record[key]

  return typeof value === "string" ? value : undefined
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null
}
