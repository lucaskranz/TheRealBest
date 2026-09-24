import "./globals.css";

// O <html> é renderizado em app/[locale]/layout.tsx, onde o locale é conhecido.
// Este layout existe apenas para que app/not-found.tsx funcione fora das rotas localizadas.
export default function RootLayout({ children }: LayoutProps<"/">) {
  return children;
}
