import { useQuery } from "@tanstack/react-query"
import { apiUrl } from "@/shared/config/api"

export function useAssetBlob(assetKey: string | undefined) {
  return useQuery<Blob, Error>({
    queryKey: ["asset", assetKey],
    queryFn: async () => {
      const res = await fetch(apiUrl(`/assets/${assetKey}`))
      if (!res.ok) {
        let msg = "Ошибка при загрузке ассета"
        try {
          const json = await res.json()
          msg = json.message ?? json.detail ?? msg
        } catch {}
        throw new Error(msg)
      }

      return await res.blob()
    },
    enabled: !!assetKey,
  })
}
