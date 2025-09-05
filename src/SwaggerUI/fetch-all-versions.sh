#!/usr/bin/env bash
set -Eeuo pipefail

# Base URL and output directory (override OUT_DIR via env or first arg)
BASE_URL="https://ri-se.github.io/Eltariff-API/releases"
OUT_DIR="${1:-${OUT_DIR:-./openapi-cache}}"

# Ensure we're inside a git repo
if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "Error: run this inside a Git repository (where your tags live)." >&2
  exit 1
fi

# Find tags like v1.2.3 (sorted ascending semver)
mapfile -t TAGS < <(git tag --list 'v[0-9]*.[0-9]*.[0-9]*' --sort=v:refname)

if [[ ${#TAGS[@]} -eq 0 ]]; then
  echo "No version tags matching v*.*.* found." >&2
  exit 0
fi

echo "Output directory: $OUT_DIR"
mkdir -p "$OUT_DIR"

# Fetch each openapi.json
for tag in "${TAGS[@]}"; do
  url="$BASE_URL/$tag/openapi.json"
  dest="$OUT_DIR/openapi-$tag.json"

  # Skip if already present (remove this block if you always want to re-download)
  if [[ -s "$dest" ]]; then
    echo "✓ $tag already downloaded → $dest"
    continue
  fi

  echo "→ $tag : $url"
  if curl -fLsS "$url" -o "$dest"; then
    echo "  Saved: $dest"
  else
    echo "  WARN: could not fetch $url (leaving no file)" >&2
    rm -f "$dest"
  fi
done
