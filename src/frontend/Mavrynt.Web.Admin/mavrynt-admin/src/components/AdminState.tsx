type AdminStateProps = {
  readonly type: "loading" | "error" | "empty";
  readonly message?: string | undefined;
};

export const AdminState = ({ type, message }: AdminStateProps) => {
  const defaults = {
    loading: "Loading…",
    error: "Something went wrong.",
    empty: "No items found.",
  };

  return (
    <div
      className="flex min-h-[120px] items-center justify-center rounded-lg border border-border bg-bg p-8 text-sm text-fg-muted"
      role={type === "error" ? "alert" : undefined}
      aria-live={type === "loading" ? "polite" : undefined}
      aria-busy={type === "loading" ? "true" : undefined}
    >
      {message ?? defaults[type]}
    </div>
  );
};
