import { useMemo, useState } from "react"
import { useQuery } from "@tanstack/react-query"
import { BookOpen, Search } from "lucide-react"

import { Input } from "@/components/ui/input"
import { ScrollArea } from "@/components/ui/scroll-area"
import { LoaderState } from "@/components/common/LoaderState"

import { getScriptApiDocs } from "./api/getScriptApiDocs"
import { ScriptApiTypeCard } from "./components/ScriptApiTypeCard"
import { formatApiTypeKind, type ApiTypeDoc } from "./types"

type SearchResult = {
  type: ApiTypeDoc
  matches: string[]
}

/**
 * Builds searchable text chunks for a user script API type.
 */
function getSearchChunks(type: ApiTypeDoc): string[] {
  return [
    type.name,
    type.fullName,
    type.namespace,
    formatApiTypeKind(type.kind),
    type.summary,

    ...type.properties.flatMap((property) => [
      property.name,
      property.type,
      property.typeFullName,
      property.summary,
    ]),

    ...type.methods.flatMap((method) => [
      method.name,
      method.returnType,
      method.returnTypeFullName,
      method.summary,
      method.returns,
      ...method.parameters.flatMap((parameter) => [
        parameter.name,
        parameter.type,
        parameter.typeFullName,
        parameter.summary,
        parameter.defaultValue,
      ]),
    ]),

    ...type.enumValues.flatMap((value) => [value.name, String(value.value), value.summary]),
  ].filter((x): x is string => Boolean(x))
}

/**
 * Returns short match labels for the sidebar.
 */
function getSearchMatches(type: ApiTypeDoc, normalizedSearch: string): string[] {
  if (!normalizedSearch) {
    return []
  }

  const matches: string[] = []

  for (const property of type.properties) {
    if (includesSearch(property.name, normalizedSearch)) {
      matches.push(`свойство ${property.name}`)
    }

    if (includesSearch(property.summary, normalizedSearch)) {
      matches.push(`описание свойства ${property.name}`)
    }
  }

  for (const method of type.methods) {
    if (includesSearch(method.name, normalizedSearch)) {
      matches.push(`метод ${method.name}`)
    }

    if (includesSearch(method.summary, normalizedSearch)) {
      matches.push(`описание метода ${method.name}`)
    }

    for (const parameter of method.parameters) {
      if (
        includesSearch(parameter.name, normalizedSearch) ||
        includesSearch(parameter.summary, normalizedSearch)
      ) {
        matches.push(`параметр ${method.name}.${parameter.name}`)
      }
    }
  }

  for (const value of type.enumValues) {
    if (
      includesSearch(value.name, normalizedSearch) ||
      includesSearch(value.summary, normalizedSearch)
    ) {
      matches.push(`значение ${value.name}`)
    }
  }

  return [...new Set(matches)].slice(0, 3)
}

function includesSearch(value: string | null | undefined, normalizedSearch: string): boolean {
  return value?.toLowerCase().includes(normalizedSearch) ?? false
}

function createSearchResults(types: ApiTypeDoc[], search: string): SearchResult[] {
  const normalizedSearch = search.trim().toLowerCase()

  if (!normalizedSearch) {
    return types.map((type) => ({
      type,
      matches: [],
    }))
  }

  return types
    .map((type) => ({
      type,
      matches: getSearchMatches(type, normalizedSearch),
      searchText: getSearchChunks(type).join("\n").toLowerCase(),
    }))
    .filter((result) => result.searchText.includes(normalizedSearch))
    .map(({ type, matches }) => ({
      type,
      matches,
    }))
}

export default function ScriptApiDocsPage() {
  const [search, setSearch] = useState("")
  const [selectedType, setSelectedType] = useState<string>()

  const { data, isLoading, error } = useQuery({
    queryKey: ["script-api-docs"],
    queryFn: getScriptApiDocs,
  })

  const searchResults = useMemo(() => {
    return createSearchResults(data?.types ?? [], search)
  }, [data, search])

  const activeType =
    searchResults.find((x) => x.type.fullName === selectedType)?.type ?? searchResults[0]?.type

  return (
    <div className="flex h-full min-h-0 overflow-hidden">
      <LoaderState isLoading={isLoading} error={error}>
        <aside className="bg-background/50 flex w-80 shrink-0 flex-col border-r">
          <div className="shrink-0 border-b p-3">
            <div className="relative">
              <Search className="text-muted-foreground absolute top-2.5 left-3 h-4 w-4" />

              <Input
                value={search}
                onChange={(e) => {
                  setSearch(e.target.value)
                }}
                placeholder="Поиск по API..."
                className="pl-9"
              />
            </div>

            <div className="text-muted-foreground mt-2 text-xs">
              Найдено: {searchResults.length}
            </div>
          </div>

          <ScrollArea className="min-h-0 flex-1">
            <div className="space-y-2 p-2 pb-6">
              {searchResults.map(({ type, matches }) => (
                <button
                  key={type.fullName}
                  onClick={() => {
                    setSelectedType(type.fullName)
                  }}
                  className={[
                    "w-full rounded-lg border px-3 py-2 text-left transition-colors",
                    activeType?.fullName === type.fullName
                      ? "border-primary bg-primary/10"
                      : "hover:bg-muted",
                  ].join(" ")}
                >
                  <div className="font-medium">{type.name}</div>

                  <div className="text-muted-foreground text-xs">
                    {formatApiTypeKind(type.kind)}
                  </div>

                  {matches.length > 0 && (
                    <div className="mt-2 flex flex-wrap gap-1">
                      {matches.map((match) => (
                        <span
                          key={match}
                          className="bg-muted text-muted-foreground rounded-md px-1.5 py-0.5 text-[11px]"
                        >
                          {match}
                        </span>
                      ))}
                    </div>
                  )}
                </button>
              ))}

              {searchResults.length === 0 && (
                <div className="text-muted-foreground rounded-lg border border-dashed p-4 text-sm">
                  Ничего не найдено. Попробуй искать по названию типа, свойства, метода,
                  enum-значения или описанию.
                </div>
              )}
            </div>
          </ScrollArea>
        </aside>

        <main className="min-h-0 flex-1 overflow-auto">
          <div className="bg-background/80 border-b px-6 py-5 backdrop-blur">
            <div className="max-w-5xl">
              <div className="flex items-center gap-3">
                <BookOpen />

                <h1 className="text-2xl font-bold tracking-tight">API</h1>
              </div>

              <p className="text-muted-foreground mt-2 max-w-3xl text-sm leading-6">
                Автоматически собираемый справочник типов, свойств, методов и enum, доступных в
                пользовательском C#-коде.
                <br />
                Документация синхронизируется напрямую с игровым API и обновляется при изменениях в
                коде.
              </p>
            </div>
          </div>

          {activeType && (
            <div className="mx-auto max-w-5xl p-6">
              <ScriptApiTypeCard type={activeType} />
            </div>
          )}
        </main>
      </LoaderState>
    </div>
  )
}
