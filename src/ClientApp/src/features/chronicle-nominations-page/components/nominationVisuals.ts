import {
  Ambulance,
  BadgeAlert,
  Clock,
  Footprints,
  Handshake,
  HeartPulse,
  Hourglass,
  PackageOpen,
  Shield,
  Swords,
  Ticket,
  Timer,
  Trophy,
  type LucideIcon,
} from "lucide-react"

export type NominationVisual = {
  Icon: LucideIcon
  accent: string
  glow: string
}

const nominationVisuals: Record<string, NominationVisual> = {
  arena_speedrun: {
    Icon: Timer,
    accent: "from-amber-500/25 via-orange-400/15 to-transparent",
    glow: "bg-amber-500/10 text-amber-700 dark:text-amber-300",
  },
  battle_telenovela: {
    Icon: Hourglass,
    accent: "from-rose-500/20 via-red-400/10 to-transparent",
    glow: "bg-rose-500/10 text-rose-700 dark:text-rose-300",
  },
  fight_pass: {
    Icon: Ticket,
    accent: "from-orange-500/24 via-red-400/12 to-transparent",
    glow: "bg-orange-500/10 text-orange-700 dark:text-orange-300",
  },
  clockwork_victory: {
    Icon: Clock,
    accent: "from-yellow-500/25 via-lime-400/10 to-transparent",
    glow: "bg-yellow-500/10 text-yellow-700 dark:text-yellow-300",
  },
  useful_experience_master: {
    Icon: BadgeAlert,
    accent: "from-stone-500/18 via-zinc-400/10 to-transparent",
    glow: "bg-stone-500/10 text-stone-700 dark:text-stone-300",
  },
  fist_diplomat: {
    Icon: Handshake,
    accent: "from-teal-500/20 via-emerald-400/10 to-transparent",
    glow: "bg-teal-500/10 text-teal-700 dark:text-teal-300",
  },
  downsizing_specialist: {
    Icon: Swords,
    accent: "from-red-600/24 via-rose-500/12 to-transparent",
    glow: "bg-red-500/10 text-red-700 dark:text-red-300",
  },
  budget_ambulance: {
    Icon: Ambulance,
    accent: "from-emerald-500/22 via-green-400/10 to-transparent",
    glow: "bg-emerald-500/10 text-emerald-700 dark:text-emerald-300",
  },
  armored_cardio: {
    Icon: Footprints,
    accent: "from-indigo-500/20 via-blue-400/10 to-transparent",
    glow: "bg-indigo-500/10 text-indigo-700 dark:text-indigo-300",
  },
  kleptomancer: {
    Icon: PackageOpen,
    accent: "from-fuchsia-500/20 via-pink-400/10 to-transparent",
    glow: "bg-fuchsia-500/10 text-fuchsia-700 dark:text-fuchsia-300",
  },
  strategic_idle: {
    Icon: Shield,
    accent: "from-slate-500/20 via-gray-400/10 to-transparent",
    glow: "bg-slate-500/10 text-slate-700 dark:text-slate-300",
  },
  damage_sponge: {
    Icon: HeartPulse,
    accent: "from-cyan-500/20 via-blue-300/10 to-transparent",
    glow: "bg-cyan-500/10 text-cyan-700 dark:text-cyan-300",
  },
}

const defaultNominationVisual: NominationVisual = {
  Icon: Trophy,
  accent: "from-primary/15 via-muted to-transparent",
  glow: "bg-muted text-foreground",
}

export function getNominationVisual(code: string): NominationVisual {
  return nominationVisuals[code] ?? defaultNominationVisual
}
