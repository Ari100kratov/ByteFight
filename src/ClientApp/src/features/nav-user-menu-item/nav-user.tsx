import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { SidebarMenu, SidebarMenuButton, SidebarMenuItem, useSidebar } from "@/components/ui/sidebar"
import { BadgeCheck, Bell, ChevronsUpDown, LogOut } from "lucide-react"
import { Spinner } from "../../components/ui/spinner"
import { Skeleton } from "@/components/ui/skeleton"
import { useCurrentUser } from "./hooks/useCurrentUser"
import useLogout from "./hooks/useLogout"

export function NavUser() {
  const { isMobile } = useSidebar()

  const { data: user } = useCurrentUser()
  const logout = useLogout()

  async function handleLogout() {
    await logout.mutateAsync()
  }

  if (!user)
    return (
      <div className="flex items-center gap-3 px-2 py-2">
        <Skeleton className="h-8 w-8 rounded-lg" /> {/* аватар */}
        <div className="flex flex-col flex-1 gap-1">
          <Skeleton className="h-4 w-32 rounded-md" /> {/* имя */}
          <Skeleton className="h-3 w-40 rounded-md" /> {/* email */}
        </div>
      </div>
    )

  const initials = `${user.firstName[0] ?? ""}${user.lastName[0] ?? ""}`.toUpperCase()

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <Avatar className="h-8 w-8 rounded-lg">
                {user.avatar
                  ? <AvatarImage src={user.avatar} alt={`${user.firstName} ${user.lastName}`} />
                  : <AvatarFallback>{initials}</AvatarFallback>}
              </Avatar>
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-medium">{`${user.firstName} ${user.lastName}`}</span>
                <span className="truncate text-xs">{user.email}</span>
              </div>
              <ChevronsUpDown className="ml-auto size-4" />
            </SidebarMenuButton>
          </DropdownMenuTrigger>

          <DropdownMenuContent
            className="w-(--radix-dropdown-menu-trigger-width) min-w-56 rounded-lg"
            side={isMobile ? "bottom" : "right"}
            align="end"
            sideOffset={4}
          >
            <DropdownMenuLabel className="p-0 font-normal">
              <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
                <Avatar className="h-8 w-8 rounded-lg">
                  {user.avatar
                    ? <AvatarImage src={user.avatar} alt={`${user.firstName} ${user.lastName}`} />
                    : <AvatarFallback>{initials}</AvatarFallback>}
                </Avatar>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-medium">{`${user.firstName} ${user.lastName}`}</span>
                  <span className="truncate text-xs">{user.email}</span>
                </div>
              </div>
            </DropdownMenuLabel>

            <DropdownMenuSeparator />

            <DropdownMenuGroup>
              <DropdownMenuItem>
                <BadgeCheck />
                Аккаунт
              </DropdownMenuItem>
              <DropdownMenuItem>
                <Bell />
                Уведомления
              </DropdownMenuItem>
            </DropdownMenuGroup>

            <DropdownMenuSeparator />

            <DropdownMenuItem
              onClick={handleLogout}
              disabled={logout.isPending}
              className={logout.isPending ? "opacity-50 cursor-not-allowed" : ""}
            >
              {logout.isPending ? (
                <>
                  <Spinner /> Выходим...
                </>
              ) : (
                <>
                  <LogOut /> Выйти
                </>
              )}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
