#!/usr/bin/env bash
#
# Build the "ElTariffKatalog" presentation into HTML, PDF and PPTX.
#
# Self-contained on purpose: this script only reads/writes files under
# doc/presentation/ and invokes Marp via `npx`. It does NOT use or modify the
# repository's package.json, dotnet build, git hooks, or any other build
# tooling, so it cannot interfere with the rest of the project.
#
# Usage:
#   ./build.sh              Render HTML + PDF + PPTX from the committed sources
#   ./build.sh --diagram    Also regenerate the Mermaid sequence diagram first
#                           (needs a headless Chromium; off by default)
#
set -euo pipefail

# Always operate from this script's own directory, regardless of where it's run.
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$HERE"

DECK="eltariff-katalog.md"
THEME="assets/rise.css"
MARP="npx --yes @marp-team/marp-cli@latest"
MMDC="npx --yes -p @mermaid-js/mermaid-cli mmdc"

# --- optional: regenerate the sequence diagram from its Mermaid source --------
# Off by default because mermaid-cli downloads a headless Chromium. The rendered
# assets/lookup_flow.svg is committed, so a normal build doesn't need this.
if [[ "${1:-}" == "--diagram" ]]; then
  echo ">> Rendering sequence diagram from Mermaid…"
  printf '{"args":["--no-sandbox"]}' > .puppeteer.json
  trap 'rm -f "$HERE/.puppeteer.json"' EXIT
  $MMDC -i assets/lookup_flow.mmd -o assets/lookup_flow.svg -b transparent -p .puppeteer.json
fi

# --- render the deck ----------------------------------------------------------
# --allow-local-files lets Marp embed the local logo/diagram into PDF and PPTX.
echo ">> Rendering HTML…"
$MARP "$DECK" --theme "$THEME" --allow-local-files -o eltariff-katalog.html

echo ">> Rendering PDF…"
$MARP "$DECK" --theme "$THEME" --allow-local-files --pdf

echo ">> Rendering PPTX…"
$MARP "$DECK" --theme "$THEME" --allow-local-files --pptx

echo ">> Done. Outputs in $HERE:"
ls -1 eltariff-katalog.html eltariff-katalog.pdf eltariff-katalog.pptx
