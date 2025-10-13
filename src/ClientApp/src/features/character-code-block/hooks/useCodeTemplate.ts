import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type CodeTemplate = {
  id: string
  name: string
  sourceCode: string
}

export function useCodeTemplate() {
  return useQuery<CodeTemplate, ApiException>({
    queryKey: queryKeys.characterCodes.template,
    queryFn: () => apiFetch('/characters/codes/template'),
  })
}
