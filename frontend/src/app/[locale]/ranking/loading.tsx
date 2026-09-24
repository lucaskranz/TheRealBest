import { Skeleton } from "@/components/ui/Skeleton";

// Esqueleto enquanto a API responde: mesmo layout da página, sem textos (evita piscar idioma)
export default function RankingLoading() {
  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-6 px-4 py-12 sm:px-6" aria-busy="true">
      <div className="flex flex-col gap-3">
        <Skeleton className="h-10 w-64" />
        <Skeleton className="h-5 w-96 max-w-full" />
      </div>
      <div className="flex gap-2">
        {Array.from({ length: 9 }, (_, i) => (
          <Skeleton key={i} className="h-8 w-14 rounded-full" />
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
