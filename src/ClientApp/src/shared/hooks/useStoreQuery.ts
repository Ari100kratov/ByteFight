import { useEffect } from "react"
import { useQuery, type UseQueryOptions, type UseQueryResult } from "@tanstack/react-query"

/**
 * Обертка над useQuery, автоматически синхронизирующая данные в Zustand store.
 *
 * @param options — все стандартные опции useQuery
 * @param setStore — функция для записи данных в Zustand
 */
export function useStoreQuery<TData, TError = unknown>(
  options: UseQueryOptions<TData, TError>,
  setStore: (data: TData) => void
): UseQueryResult<TData, TError> {
  const query = useQuery(options)

  useEffect(() => {
    if (query.data) setStore(query.data)
  }, [query.data, setStore])

  return query
}
