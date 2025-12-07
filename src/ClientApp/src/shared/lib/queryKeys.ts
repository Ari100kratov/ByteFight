export const queryKeys = {
  users: {
    current: ['users', 'current'] as const,
  },

  characters: {
    byCurrentUser: ['characters', 'currentUser'] as const,
    byId: (id: string | undefined) => ['characters', 'byId', id] as const,
    details: (id: string | undefined) => ['characters', 'details', id] as const,
  },

  characterCodes: {
    byCharacterId: (characterId: string) => ['character-codes', 'byCharacterId', characterId] as const,
    template: ['character-codes', 'template'] as const,
  },

  gameModes: {
    all: ['game-modes', 'all'] as const,
  },

  arenas: {
    byMode: (mode: string | undefined) => ['arenas', 'byMode', mode] as const,
    byId: (id: string | undefined) => ['arenas', 'byId', id] as const,
  },

  arenaEnemies: {
    byArenaId: (arenaId: string | undefined) => ['arenaEnemies', 'byArenaId', arenaId] as const,
  },

  enemies: {
    byId: (id: string | undefined) => ['enemies', 'byId', id] as const,
  },

  characterClasses: {
    all: ['character-classes', 'all'] as const
  },

  gameSessions: {
    byId: (id: string | undefined) => ['game-sessions', 'byId', id] as const,
  }

} as const
