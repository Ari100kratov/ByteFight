import { useQuery } from '@tanstack/react-query'
import { ApiException, apiFetch } from '@/shared/lib/apiFetch'
import { queryKeys } from '@/shared/lib/queryKeys'

export type CurrentUserResponse = {
  id: string
  email: string
  firstName: string
  lastName: string,
  avatar?: string
}

export function useCurrentUser() {
  return useQuery<CurrentUserResponse, ApiException>({
    queryKey: queryKeys.users.current,
    queryFn: () => apiFetch<CurrentUserResponse>('/users/current'),
  })
}
