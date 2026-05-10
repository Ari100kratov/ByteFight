import { AlertTriangle, Code2, Sparkles } from "lucide-react"

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

      <section className="rounded-xl border border-yellow-500/40 bg-yellow-500/10 p-4">
        <div className="flex items-start gap-3">
          <AlertTriangle className="mt-0.5 h-5 w-5 text-yellow-500" />

          <div className="space-y-2 text-sm leading-6">
            <div className="font-semibold text-yellow-600 dark:text-yellow-400">
              Не забудьте сохранить свой код
            </div>

            <p className="text-muted-foreground">
              Сейчас легко потерять изменения после завершения боя.
              Для перезапуска требуется покинуть завершённую сессию
              через кнопку "Выйти из боя" или хлебные крошки, а страница при этом
              полностью перезагружается.
            </p>

            <p className="text-muted-foreground">
              Если код не сохранён — изменения могут потеряться.
              В будущем это поведение планируется исправить.
            </p>
          </div>
        </div>
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