import { useQuery } from '@tanstack/react-query'
import { ApiException, apiFetch } from '@/lib/apiFetch'
import { queryKeys } from '@/lib/queryKeys'

export type CurrentUser = {
  id: string
  email: string
  firstName: string
  lastName: string,
  avatar?: string
}

export function useCurrentUser() {
  return useQuery<CurrentUser, ApiException>({
    queryKey: queryKeys.users.current,
    queryFn: () => apiFetch<CurrentUser>('/users/current'),
  })
}
