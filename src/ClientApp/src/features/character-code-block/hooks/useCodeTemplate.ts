import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

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
