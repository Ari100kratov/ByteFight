"use client"

import * as React from "react"
import {
  BookOpen,
  Bot,
  // Frame,
  // Map,
  // PieChart,
  Archive,
  Send,
  Settings2,
  FolderGit2,
  Swords,
  // Gamepad2,
  ClipboardList
} from "lucide-react"

import { NavMain } from "@/components/nav-main"
// import { NavProjects } from "@/components/nav-projects"
import { NavSecondary } from "@/components/nav-secondary"
import { NavUser } from "@/features/nav-user-menu-item/nav-user"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar"
import { Link } from "react-router-dom"

const data = {
  user: {
    name: "shadcn",
    email: "m@example.com",
    avatar: "/avatars/shadcn.jpg",
  },
  navMain: [
    {
      title: "Играть",
      url: "/play",
      icon: Swords,
      items: [ // сделать динамическим?
        {
          title: "Тренировка",
          url: "/play/training",
        },
        {
          title: "PvE",
          url: "/play/pve",
        },
        {
          title: "PvP",
          url: "/play/pvp",
        },
      ],
    },
    {
      title: "Мои персонажи",
      url: "/characters",
      icon: Bot,
      // items: [ Здесь хочу выводить персонажи пользователя списком ]
    },
    {
      title: "Боевой архив",
      url: "/battle-archive",
      icon: Archive,
      items: [
        {
          title: "История боев",
          url: "/battle-archive/history"
        },
        {
          title: "Зал славы",
          url: "/battle-archive/hall-of-fame"
        },
        {
          title: "Статистика",
          url: "/battle-archive/stats"
        },
        {
          title: "Рейтинг",
          url: "/battle-archive/leaderboard"
        }
      ]
    },
    {
      title: "Документация",
      url: "/docs",
      icon: BookOpen,
      items: [
        {
          title: "Быстрый старт",
          url: "/docs/quick-start",
        },
        {
          title: "Рецепты",
          url: "/docs/recipes",
        },
        {
          title: "API",
          url: "/docs/script-api",
        },
      ],
    },
    {
      title: "Настройки",
      url: "/settings",
      icon: Settings2
    },
  ],
  navSecondary: [
    {
      title: "Github",
      url: "https://github.com/Ari100kratov/ByteFight",
      icon: FolderGit2,
    },
    {
      title: "Связаться со мной",
      url: "https://t.me/whatislovesir",
      icon: Send,
    },
    {
      title: "Обратная связь",
      url: "https://docs.google.com/forms/d/e/1FAIpQLSd-krD2U1ENQKC0zog9loBzZQvXJMm3sfrzJ-w8HAjb2lGZOw/viewform?usp=dialog",
      icon: ClipboardList,
    },
  ],
  // projects: [
  //   {
  //     name: "Design Engineering",
  //     url: "#",
  //     icon: Frame,
  //   },
  //   {
  //     name: "Sales & Marketing",
  //     url: "#",
  //     icon: PieChart,
  //   },
  //   {
  //     name: "Travel",
  //     url: "#",
  //     icon: Map,
  //   },
  // ],
}

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {

  return (
    <Sidebar variant="inset" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild>
              <Link to="/">
                {/* <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                  <img src="/logo.png" className="size-4" />
                </div> */}
                <img src="/logo.png" className="size-10 object-contain" />
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-medium">ByteFight</span>
                  <span className="truncate text-xs">Online</span>
                </div>
              </Link>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={data.navMain} />
        {/* <NavProjects projects={data.projects} /> */}
        <NavSecondary items={data.navSecondary} className="mt-auto" />
      </SidebarContent>
      <SidebarFooter>
        <NavUser />
      </SidebarFooter>
    </Sidebar>
  )
}
