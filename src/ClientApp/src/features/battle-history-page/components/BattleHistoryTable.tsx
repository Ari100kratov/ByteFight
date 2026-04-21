import {
  flexRender,
  getCoreRowModel,
  useReactTable,
  type ColumnDef,
} from "@tanstack/react-table"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

type BattleHistoryTableProps<TData> = {
  columns: ColumnDef<TData>[]
  data: TData[]
  onRowClick?: (row: TData) => void
}

export function BattleHistoryTable<TData>({
  columns,
  data,
  onRowClick,
}: BattleHistoryTableProps<TData>) {
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <div className="h-full min-h-0 overflow-auto rounded-xl border bg-background">
      <Table className="table-fixed">
        <TableHeader className="bg-background">
          {table.getHeaderGroups().map(headerGroup => (
            <TableRow key={headerGroup.id} className="hover:bg-transparent">
              {headerGroup.headers.map(header => (
                <TableHead
                  key={header.id}
                  className="sticky top-0 z-10 bg-background"
                >
                  {header.isPlaceholder
                    ? null
                    : flexRender(
                        header.column.columnDef.header,
                        header.getContext()
                      )}
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>

        <TableBody>
          {table.getRowModel().rows.length ? (
            table.getRowModel().rows.map(row => (
              <TableRow
                key={row.id}
                onClick={onRowClick ? () => onRowClick(row.original) : undefined}
                className={
                  onRowClick
                    ? "cursor-pointer transition-colors hover:bg-muted/50"
                    : undefined
                }
              >
                {row.getVisibleCells().map(cell => (
                  <TableCell key={cell.id}>
                    {flexRender(
                      cell.column.columnDef.cell,
                      cell.getContext()
                    )}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell
                colSpan={columns.length}
                className="h-24 text-center text-muted-foreground"
              >
                История боев пока пуста
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  )
}