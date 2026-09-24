import { createNavigation } from "next-intl/navigation";

import { routing } from "./routing";

// Versões localizadas das APIs de navegação: preservam o prefixo de locale nas URLs
export const { Link, redirect, usePathname, useRouter, getPathname } = createNavigation(routing);
