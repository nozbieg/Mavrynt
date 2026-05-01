import type { ReactNode } from "react";
import { cn } from "@mavrynt/ui";

export type Column<T> = {
  key: string;
  header: string;
  render: (row: T) => ReactNode;
  className?: string;
};

interface AdminTableProps<T> {
  readonly columns: Column<T>[];
  readonly rows: T[];
  readonly getKey: (row: T) => string;
}

export const AdminTable = <T,>({
  columns,
  rows,
  getKey,
}: AdminTableProps<T>) => (
  <div className="overflow-x-auto rounded-lg border border-border">
    <table className="w-full text-sm">
      <thead>
        <tr className="border-b border-border bg-bg-subtle">
          {columns.map((col) => (
            <th
              key={col.key}
              scope="col"
              className={cn(
                "px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-fg-muted",
                col.className,
              )}
            >
              {col.header}
            </th>
          ))}
        </tr>
      </thead>
      <tbody className="divide-y divide-border bg-bg">
        {rows.map((row) => (
          <tr
            key={getKey(row)}
            className="transition-colors hover:bg-bg-subtle"
          >
            {columns.map((col) => (
              <td
                key={col.key}
                className={cn("px-4 py-3 text-fg", col.className)}
              >
                {col.render(row)}
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  </div>
);
