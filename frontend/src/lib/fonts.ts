import { Inter } from "next/font/google";
import localFont from "next/font/local";

// Cabinet Grotesk não está no Google Fonts: arquivos self-hosted (Fontshare, ITF Free Font License)
export const cabinetGrotesk = localFont({
  src: [
    { path: "../fonts/CabinetGrotesk-Medium.woff2", weight: "500", style: "normal" },
    { path: "../fonts/CabinetGrotesk-Bold.woff2", weight: "700", style: "normal" },
    { path: "../fonts/CabinetGrotesk-Extrabold.woff2", weight: "800", style: "normal" },
    { path: "../fonts/CabinetGrotesk-Black.woff2", weight: "900", style: "normal" },
  ],
  variable: "--font-cabinet",
  display: "swap",
});

export const inter = Inter({
  subsets: ["latin"],
  variable: "--font-inter",
  display: "swap",
});
