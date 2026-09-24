import { notFound } from "next/navigation";

// Qualquer rota localizada desconhecida cai no not-found do locale (com header, footer e traduções)
export default function CatchAllPage() {
  notFound();
}
