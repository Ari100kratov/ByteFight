export function formatBattleDuration(startedAt?: string, endedAt?: string | null) {
  if (!startedAt || !endedAt) {
    return "—"
  }

  const start = new Date(startedAt).getTime()
  const end = new Date(endedAt).getTime()

  if (Number.isNaN(start) || Number.isNaN(end) || end < start) {
    return "—"
  }

  const totalSeconds = Math.floor((end - start) / 1000)

  const minutes = Math.floor(totalSeconds / 60)
  const seconds = totalSeconds % 60

  if (minutes <= 0) {
    return `${String(seconds)} сек`
  }

  if (seconds === 0) {
    return `${String(minutes)} мин`
  }

  return `${String(minutes)} мин ${String(seconds)} сек`
}
