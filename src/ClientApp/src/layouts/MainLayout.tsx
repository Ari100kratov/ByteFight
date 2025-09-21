import { Outlet, useLocation, Link } from "react-router-dom"
import { AppSidebar } from "@/components/app-sidebar"
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb"
import { Separator } from "@/components/ui/separator"
import {
  SidebarInset,
  SidebarProvider,
  SidebarTrigger,
} from "@/components/ui/sidebar"
import { useBreadcrumbNames } from "./BreadcrumbProvider"

const routeNames: Record<string, string> = {
  play: "Играть",

  characters: "Мои персонажи",
  create: "Создание персонажа",

  docs: "Документация",
  settings: "Настройки",
}

export default function MainLayout() {
  const location = useLocation()
  const parts = location.pathname.split("/").filter(Boolean)
  const { names } = useBreadcrumbNames()

  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarInset>
        <header className="flex h-16 shrink-0 items-center gap-2 border-b px-4">
          <div className="flex items-center gap-2 px-4">
            <SidebarTrigger className="-ml-1" />
            <Separator orientation="vertical" className="mr-2 data-[orientation=vertical]:h-4" />
            <Breadcrumb>
              <BreadcrumbList>
                {parts.map((part, idx) => {
                  const path = "/" + parts.slice(0, idx + 1).join("/")
                  const isLast = idx === parts.length - 1
                  const label = names[path] || routeNames[part] || decodeURIComponent(part)

                  return (
                    <BreadcrumbItem key={path}>
                      {isLast ? (
                        <BreadcrumbPage>{label}</BreadcrumbPage>
                      ) : (
                        <>
                          <BreadcrumbLink asChild>
                            <Link to={path}>{label}</Link>
                          </BreadcrumbLink>
                          <BreadcrumbSeparator />
                        </>
                      )}
                    </BreadcrumbItem>
                  )
                })}
              </BreadcrumbList>
            </Breadcrumb>
          </div>
        </header>

        <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
          <Outlet />
        </div>
      </SidebarInset>
    </SidebarProvider>
  )
}
