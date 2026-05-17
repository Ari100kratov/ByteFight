import { BadgeInfo, Braces, FunctionSquare } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"

import {
  formatApiTypeKind,
  type ApiMethodDoc,
  type ApiPropertyDoc,
  type ApiTypeDoc,
} from "../types"

interface Props {
  type: ApiTypeDoc
}

export function ScriptApiTypeCard({ type }: Props) {
  return (
    <div className="space-y-6">
      <header className="space-y-3">
        <div className="flex items-center gap-3">
          <h1 className="text-3xl font-bold">{type.name}</h1>

          <Badge variant="secondary">{formatApiTypeKind(type.kind)}</Badge>
        </div>

        <div className="text-muted-foreground font-mono text-sm">{type.fullName}</div>

        {type.summary && <p className="text-muted-foreground max-w-3xl">{type.summary}</p>}
      </header>

      {type.properties.length > 0 && (
        <section className="space-y-4">
          <div className="flex items-center gap-2">
            <BadgeInfo className="h-5 w-5" />
            <h2 className="text-xl font-semibold">Свойства</h2>
          </div>

          <div className="space-y-3">
            {type.properties.map((property) => (
              <PropertyCard key={property.name} property={property} />
            ))}
          </div>
        </section>
      )}

      {type.methods.length > 0 && (
        <section className="space-y-4">
          <div className="flex items-center gap-2">
            <FunctionSquare className="h-5 w-5" />

            <h2 className="text-xl font-semibold">Методы</h2>
          </div>

          <div className="space-y-3">
            {type.methods.map((method) => (
              <MethodCard key={method.name} method={method} />
            ))}
          </div>
        </section>
      )}

      {type.enumValues.length > 0 && (
        <section className="space-y-4">
          <div className="flex items-center gap-2">
            <Braces className="h-5 w-5" />

            <h2 className="text-xl font-semibold">Значения</h2>
          </div>

          <div className="space-y-3">
            {type.enumValues.map((value) => (
              <div key={value.name} className="rounded-xl border p-4">
                <div className="flex items-center gap-3">
                  <code className="font-semibold">{value.name}</code>

                  <Badge variant="outline">{value.value}</Badge>
                </div>

                {value.summary && (
                  <p className="text-muted-foreground mt-2 text-sm">{value.summary}</p>
                )}
              </div>
            ))}
          </div>
        </section>
      )}
    </div>
  )
}

function PropertyCard({ property }: { property: ApiPropertyDoc }) {
  return (
    <div className="rounded-xl border p-4">
      <div className="flex flex-wrap items-center gap-2">
        <code className="font-semibold">{property.name}</code>

        <Separator orientation="vertical" className="h-4" />

        <span className="text-primary font-mono text-sm">{property.type}</span>

        {property.isRequired && <Badge>required</Badge>}

        {property.isNullable && <Badge variant="secondary">nullable</Badge>}

        {property.isComputed && <Badge variant="outline">computed</Badge>}
      </div>

      {property.summary && <p className="text-muted-foreground mt-2 text-sm">{property.summary}</p>}
    </div>
  )
}

function MethodCard({ method }: { method: ApiMethodDoc }) {
  return (
    <div className="rounded-xl border p-4">
      <div className="overflow-x-auto">
        <code className="font-semibold">
          {method.name}(
          {method.parameters.map((p, index) => (
            <span key={p.name}>
              {index > 0 && ", "}
              {p.name}: {p.type}
            </span>
          ))}
          ) : {method.returnType}
        </code>
      </div>

      {method.summary && <p className="text-muted-foreground mt-2 text-sm">{method.summary}</p>}

      {method.parameters.length > 0 && (
        <div className="mt-4 space-y-2">
          {method.parameters.map((parameter) => (
            <div key={parameter.name} className="bg-muted/50 rounded-md p-2">
              <div className="font-mono text-sm">
                {parameter.name}: {parameter.type}
              </div>

              {parameter.summary && (
                <div className="text-muted-foreground text-xs">{parameter.summary}</div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
