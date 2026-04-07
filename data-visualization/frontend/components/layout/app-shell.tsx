export function AppShell({
  title,
  subtitle,
  children
}: {
  title: string;
  subtitle: string;
  children: React.ReactNode;
}) {
  return (
    <main className="min-h-screen px-3 py-4 sm:px-4 sm:py-6 md:px-8 md:py-8">
      <div className="mx-auto max-w-7xl space-y-5 sm:space-y-6 md:space-y-8">
        <section className="rounded-[28px] border border-white/60 bg-white/70 p-5 shadow-panel backdrop-blur sm:rounded-[32px] sm:p-6 md:p-8">
          <div className="inline-flex rounded-full bg-orange-100 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.2em] text-orange-700 sm:text-xs">
            Analytics Lab
          </div>
          <h1 className="mt-3 text-3xl font-semibold tracking-tight text-slate-950 sm:mt-4 sm:text-4xl">{title}</h1>
          <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600 sm:mt-3 sm:text-base">{subtitle}</p>
        </section>
        {children}
      </div>
    </main>
  );
}
