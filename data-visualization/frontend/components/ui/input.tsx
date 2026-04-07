import { cn } from "@/lib/utils";

export function Input(props: React.InputHTMLAttributes<HTMLInputElement>) {
  return (
    <input
      {...props}
      className={cn(
        "h-10 w-full rounded-2xl border border-border bg-white px-3 text-sm text-slate-900 outline-none placeholder:text-slate-400 focus:border-slate-300",
        props.className
      )}
    />
  );
}
