"use client"

import * as React from "react"
import {
  BookOpen,
  Bot,
  // Frame,
  // Map,
  // PieChart,
  Send,
  Settings2,
  Github,
  Swords,
  Gamepad2
} from "lucide-react"

import { NavMain } from "@/components/nav-main"
// import { NavProjects } from "@/components/nav-projects"
import { NavSecondary } from "@/components/nav-secondary"
import { NavUser } from "@/features/nav-user/nav-user"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar"

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
      isActive: true,
      // items: [
      //   {
      //     title: "PvE",
      //     url: "#",
      //   },
      //   {
      //     title: "PvP",
      //     url: "#",
      //   },
      //   {
      //     title: "Тренировка",
      //     url: "#",
      //   },
      // ],
    },
    {
      title: "Мои персонажи",
      url: "/characters",
      icon: Bot,
      // items: [ Здесь хочу выводить персонажи пользователя списком ]
    },
    {
      title: "Документация",
      url: "/docs",
      icon: BookOpen
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
      icon: Github,
    },
    {
      title: "Обратная связь",
      url: "https://t.me/whatislovesir",
      icon: Send,
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
              <a href="/">
                <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                  <Gamepad2 className="size-4" />
                </div>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-medium">ByteFight</span>
                  <span className="truncate text-xs">Online</span>
                </div>
              </a>
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
