import { Skeleton } from "@/components/ui/skeleton"

export function ChronicleNominationsSkeleton() {
  return (
    <div className="flex flex-col gap-4 p-2 md:p-4">
      <Skeleton className="h-40 rounded-3xl" />
      <div className="grid gap-4 lg:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: 6 }).map((_, index) => (
          <Skeleton key={index} className="h-80 rounded-xl" />
        ))}
      </div>
    </div>
  )
}
