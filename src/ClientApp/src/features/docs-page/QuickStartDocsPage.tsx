import { Code2, Sparkles } from "lucide-react"

import { DocsCodeBlock } from "./components/DocsCodeBlock"
import { DocsInfoCard } from "./components/DocsInfoCard"
import { defaultScriptExample, quickStartFacts } from "./content/docsContent"

/**
 * Documentation entry page for users writing character behavior scripts.
 */
export default function QuickStartDocsPage() {
  return (
    <div className="mx-auto flex w-full max-w-6xl flex-col gap-6 p-2 md:p-6">
      <header>
        <div className="flex items-center gap-3">
          <Sparkles />

          <h1 className="text-3xl font-bold tracking-tight">
            Быстрый старт
          </h1>
        </div>
      </header>

      <section className="grid gap-3 md:grid-cols-2">
        {quickStartFacts.map(fact => (
          <DocsInfoCard
            key={fact.title}
            title={fact.title}
          >
            {fact.content}
          </DocsInfoCard>
        ))}
      </section>

      <section className="rounded-xl border bg-background p-4">
        <div className="mb-4 flex items-center gap-2">
          <Code2 className="h-5 w-5" />

          <h2 className="text-xl font-semibold">
            Базовый скрипт
          </h2>
        </div>

        <DocsCodeBlock code={defaultScriptExample} />
      </section>
    </div>
  )
}