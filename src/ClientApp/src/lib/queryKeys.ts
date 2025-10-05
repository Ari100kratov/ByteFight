export const queryKeys = {
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

  users: {
    current: ['users', 'current'] as const,
  },
} as const
