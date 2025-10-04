export const ChangeStatus = {
  Unchanged: "unchanged",
  Created: "created",
  Updated: "updated",
  Deleted: "deleted",
} as const

type ChangeStatus = (typeof ChangeStatus)[keyof typeof ChangeStatus]

export type LocalCode = {
  id: string
  name: string
  sourceCode: string
  status: ChangeStatus
}
