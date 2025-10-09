export const queryKeys = {
  users: {
    current: ['users', 'current'] as const,
  },
  
  characters: {
    byCurrentUser: ['characters', 'currentUser'] as const,
    byId: (id: string | undefined) => ['characters', 'byId', id] as const,
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

} as const
