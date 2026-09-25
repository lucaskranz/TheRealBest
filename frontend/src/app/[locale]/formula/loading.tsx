import { Skeleton } from "@/components/ui/Skeleton";

// Esqueleto enquanto a API responde: mesmo layout da página, sem textos (evita piscar idioma)
export default function Loading() {
  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-8 px-4 py-12 sm:px-6" aria-busy="true">
      <div className="flex flex-col gap-3">
        <Skeleton className="h-10 w-72 max-w-full" />
        <Skeleton className="h-5 w-[32rem] max-w-full" />
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        {Array.from({ length: 3 }, (_, i) => (
          <Skeleton key={i} className="h-28 w-full rounded-lg" />
        ))}
      </div>
      <div className="glass flex flex-col gap-1 rounded-lg p-4">
        {Array.from({ length: 10 }, (_, i) => (
          <Skeleton key={i} className="h-12 w-full" />
        ))}
      </div>
    </section>
  );
}
