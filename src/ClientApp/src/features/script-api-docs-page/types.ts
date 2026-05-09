export type UserCodeApiDoc = {
  types: ApiTypeDoc[]
}

export type ApiTypeKind = 1 | 2 | 3 | 4

export const ApiTypeKind = {
  Class: 1,
  Record: 2,
  Struct: 3,
  Enum: 4,
} as const satisfies Record<string, ApiTypeKind>

export function formatApiTypeKind(kind: ApiTypeKind): string {
  switch (kind) {
    case ApiTypeKind.Class:
      return "class"
    case ApiTypeKind.Record:
      return "record"
    case ApiTypeKind.Struct:
      return "struct"
    case ApiTypeKind.Enum:
      return "enum"
  }
}

export type ApiTypeDoc = {
  name: string
  fullName: string
  namespace: string
  kind: ApiTypeKind
  summary?: string | null

  properties: ApiPropertyDoc[]
  methods: ApiMethodDoc[]
  enumValues: ApiEnumValueDoc[]
}

export type ApiPropertyDoc = {
  name: string
  type: string
  typeFullName?: string | null
  summary?: string | null

  isRequired: boolean
  isNullable: boolean
  isComputed: boolean
}

export type ApiMethodDoc = {
  name: string
  returnType: string
  returnTypeFullName?: string | null

  summary?: string | null
  returns?: string | null

  parameters: ApiParameterDoc[]
}

export type ApiParameterDoc = {
  name: string
  type: string
  typeFullName?: string | null

  summary?: string | null

  isNullable: boolean
  isOptional: boolean
  defaultValue?: string | null
}

export type ApiEnumValueDoc = {
  name: string
  value: number
  summary?: string | null
}