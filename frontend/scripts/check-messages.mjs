// Verifica se todos os locales têm exatamente as mesmas chaves de tradução que pt-BR (referência).
import { readdirSync, readFileSync } from "node:fs";
import { join } from "node:path";

const messagesDir = join(import.meta.dirname, "..", "src", "messages");
const referenceLocale = "pt-BR";

function flattenKeys(value, prefix = "") {
  if (typeof value !== "object" || value === null) return [prefix];
  return Object.entries(value).flatMap(([key, child]) => flattenKeys(child, prefix ? `${prefix}.${key}` : key));
}

function readKeys(locale, file) {
  const json = JSON.parse(readFileSync(join(messagesDir, locale, file), "utf8"));
  return new Set(flattenKeys(json));
}

const locales = readdirSync(messagesDir);
const namespaces = readdirSync(join(messagesDir, referenceLocale)).filter((f) => f.endsWith(".json"));
const errors = [];

for (const locale of locales.filter((l) => l !== referenceLocale)) {
  for (const file of namespaces) {
    let keys;
    try {
      keys = readKeys(locale, file);
    } catch {
      errors.push(`${locale}/${file}: arquivo ausente ou JSON inválido`);
      continue;
    }

    const reference = readKeys(referenceLocale, file);
    for (const key of reference) if (!keys.has(key)) errors.push(`${locale}/${file}: faltando "${key}"`);
    for (const key of keys) if (!reference.has(key)) errors.push(`${locale}/${file}: chave extra "${key}"`);
  }
}

if (errors.length > 0) {
  console.error(errors.join("\n"));
  process.exit(1);
}

console.log(`Traduções consistentes: ${locales.length} locales × ${namespaces.length} namespaces.`);
