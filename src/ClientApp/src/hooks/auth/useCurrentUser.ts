import { useQuery } from '@tanstack/react-query'
import { apiFetch } from '@/lib/apiFetch'
import { queryKeys } from '@/lib/queryKeys'

export interface CurrentUser {
  id: string
  email: string
  firstName: string
  lastName: string,
  avatar?: string
}

export function useCurrentUser() {
  return useQuery<CurrentUser, Error>({
    queryKey: queryKeys.users.current,
    queryFn: () => apiFetch<CurrentUser>('/users/current'),
  })
}
