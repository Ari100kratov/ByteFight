import { apiFetch } from "@/shared/lib/apiFetch"
import type { UserCodeApiDoc } from "../types"

export async function getScriptApiDocs(): Promise<UserCodeApiDoc> {
  return await apiFetch<UserCodeApiDoc>(
    "/docs/user-code-api"
  )
}