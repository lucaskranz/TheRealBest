import { Skeleton } from "@/components/ui/Skeleton";

// Esqueleto do perfil (também usado pelo recibo, que está abaixo desta rota)
export default function PlayerLoading() {
  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-8 px-4 py-12 sm:px-6" aria-busy="true">
      <div className="flex items-center gap-5">
        <Skeleton className="size-24 rounded-full" />
        <div className="flex flex-col gap-3">
          <Skeleton className="h-10 w-72 max-w-full" />
          <Skeleton className="h-6 w-48" />
        </div>
      </div>
      <div className="grid gap-6 lg:grid-cols-2">
        <Skeleton className="h-72 rounded-lg" />
        <Skeleton className="h-72 rounded-lg" />
      </div>
      <Skeleton className="h-96 rounded-lg" />
    </section>
  );
}
