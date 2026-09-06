import { apiFetch } from '@/shared/lib/apiFetch'

export interface TalentNodeResponse {
  id: string
  name: string
  description: string
  tier: number
  maxRank: number
  currentRank: number
  isPassive: boolean
  canLearn: boolean
}

export interface TalentBranchResponse {
  id: string
  name: string
  description: string
  nodes: TalentNodeResponse[]
}

export interface CharacterTalentsResponse {
  level: number
  talentPointsTotal: number
  talentPointsSpent: number
  talentPointsAvailable: number
  classType: string
  branches: TalentBranchResponse[]
}

export function fetchCharacterTalents(characterId: string): Promise<CharacterTalentsResponse> {
  return apiFetch(`/characters/${characterId}/talents`)
}

export function learnTalent(characterId: string, talentId: string): Promise<void> {
  return apiFetch(`/characters/${characterId}/talents`, {
    method: 'POST',
    body: JSON.stringify({ talentId }),
  })
}

export function resetTalents(characterId: string): Promise<void> {
  return apiFetch(`/characters/${characterId}/talents`, { method: 'DELETE' })
}
