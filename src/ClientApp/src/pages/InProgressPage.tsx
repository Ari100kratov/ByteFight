export default function InProgressPage({ title }: { title: string }) {
  return (
    <div className="flex h-full items-center justify-center">
      <h1 className="text-2xl font-bold">{title} – страница в разработке</h1>
    </div>
  )
}
