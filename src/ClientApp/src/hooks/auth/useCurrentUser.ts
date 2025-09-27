import { useQuery } from '@tanstack/react-query'
import { apiFetch } from '@/lib/apiFetch'

export interface CurrentUser {
  id: string
  email: string
  firstName: string
  lastName: string,
  avatar?: string
}

export function useCurrentUser() {
  return useQuery<CurrentUser, Error>({
    queryKey: ['currentUser'],
    queryFn: () => apiFetch<CurrentUser>('/users/current'),
  })
}
