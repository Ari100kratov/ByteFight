import { useQuery } from "@tanstack/react-query"

export function useAsset(assetKey: string | undefined) {
  return useQuery<string, Error>({
    queryKey: ["asset", assetKey],
    queryFn: async () => {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/assets/${assetKey}`)
      if (!res.ok) {
        let msg = "Ошибка при загрузке ассета"
        try {
          const json = await res.json()
          msg = json.message ?? json.detail ?? msg
        } catch { }
        throw new Error(msg)
      }

      const blob = await res.blob()
      return URL.createObjectURL(blob)
    },
    enabled: !!assetKey,
    staleTime: Infinity,
  })
}
