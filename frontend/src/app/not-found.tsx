import Link from "next/link";

// Fallback para requisições fora do matcher do proxy (sem locale resolvido).
// Não há locale disponível aqui, então a página não exibe textos traduzíveis — só o código e a marca.
export default function GlobalNotFound() {
  return (
    <html lang="en">
      <body className="flex min-h-dvh flex-col items-center justify-center gap-6">
        <p className="font-display text-7xl font-black text-gradient-gold">404</p>
        <Link href="/" className="font-display text-lg font-black tracking-wide text-secondary uppercase hover:text-main">
          The Real Best
        </Link>
      </body>
    </html>
  );
}
