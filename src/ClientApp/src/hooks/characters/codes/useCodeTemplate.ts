import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"

export type CodeTemplate = {
  id: string
  name: string
  sourceCode: string
}

export function useCodeTemplate() {
  return useQuery<CodeTemplate, Error>({
    queryKey: ['code-template'],
    queryFn: () => apiFetch('/characters/codes/template'),
  })
}
