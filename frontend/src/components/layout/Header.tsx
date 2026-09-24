import { BrandMark } from "./BrandMark";
import { LocaleSwitcher } from "./LocaleSwitcher";
import { NavLinks } from "./NavLinks";

export function Header() {
  return (
    <header className="glass sticky top-0 z-50 border-x-0 border-t-0">
      <div className="mx-auto flex h-18 max-w-6xl items-center justify-between gap-4 px-4 sm:px-6">
        <BrandMark />
        <NavLinks className="hidden md:block" />
        <LocaleSwitcher />
      </div>
      {/* Mobile: navegação em uma segunda linha com rolagem horizontal */}
      <NavLinks className="overflow-x-auto border-t border-line px-2 py-1 md:hidden" />
    </header>
  );
}
