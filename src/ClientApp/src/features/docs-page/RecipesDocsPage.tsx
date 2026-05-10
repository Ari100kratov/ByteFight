import { BookOpenCheck } from "lucide-react"

import { DocsCodeBlock } from "./components/DocsCodeBlock"
import { UsedApiList } from "./components/UsedApiList"
import { recipes } from "./content/docsContent"

/**
 * Displays practical user-script recipes for common character behavior scenarios.
 */
export default function RecipesDocsPage() {
  return (
    <div className="mx-auto flex w-full max-w-6xl flex-col gap-6 p-2 md:p-6">
      <header className="space-y-2">
        <div className="flex items-center gap-3">
          <BookOpenCheck />

          <h1 className="text-3xl font-bold tracking-tight">
            Рецепты
          </h1>
        </div>

        <p className="max-w-3xl text-muted-foreground [&_code]:rounded-md [&_code]:border [&_code]:bg-muted [&_code]:px-1.5 [&_code]:py-0.5 [&_code]:font-mono [&_code]:text-foreground">
          Готовые логические блоки, которые можно копировать и дорабатывать
          под свою стратегию.
        </p>
      </header>

      <div className="space-y-4">
        {recipes.map(recipe => (
          <article
            key={recipe.title}
            className="rounded-xl border bg-background p-4"
          >
            <div className="mb-4 space-y-2">
              <h2 className="text-xl font-semibold">
                {recipe.title}
              </h2>

              <p className="text-sm leading-6 text-muted-foreground [&_code]:rounded-md [&_code]:border [&_code]:bg-muted [&_code]:px-1.5 [&_code]:py-0.5 [&_code]:font-mono [&_code]:text-foreground">
                {recipe.description}
              </p>
            </div>

            <DocsCodeBlock code={recipe.code} />

            <div className="mt-4 space-y-2">
              <div className="text-sm font-medium">
                Используемые API
              </div>

              <UsedApiList items={recipe.usedApi} />
            </div>
          </article>
        ))}
      </div>
    </div>
  )
}