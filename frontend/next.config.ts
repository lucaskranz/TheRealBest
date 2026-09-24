import type { NextConfig } from "next";
import createNextIntlPlugin from "next-intl/plugin";

const withNextIntl = createNextIntlPlugin("./src/i18n/request.ts");

const nextConfig: NextConfig = {
  images: {
    // Fotos de jogadores e escudos vêm da CDN da API-Football
    remotePatterns: [new URL("https://media.api-sports.io/football/**")],
  },
};

export default withNextIntl(nextConfig);
