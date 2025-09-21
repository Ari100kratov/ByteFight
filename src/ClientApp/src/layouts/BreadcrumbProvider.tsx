import { createContext, useState, useContext, type ReactNode } from "react"

type DynamicNames = Record<string, string>

const BreadcrumbContext = createContext<{
  names: DynamicNames
  setName: (key: string, value: string) => void
}>({
  names: {},
  setName: () => {},
})

export function useBreadcrumbNames() {
  return useContext(BreadcrumbContext)
}

export function BreadcrumbProvider({ children }: { children: ReactNode }) {
  const [names, setNames] = useState<DynamicNames>({})

  function setName(key: string, value: string) {
    setNames(prev => (prev[key] === value ? prev : { ...prev, [key]: value }))
  }

  return (
    <BreadcrumbContext.Provider value={{ names, setName }}>
      {children}
    </BreadcrumbContext.Provider>
  )
}
