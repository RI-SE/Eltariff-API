# ElTariffKatalog — presentation

A short slide deck about the **catalogue service** (ElTariffKatalog) in the
Eltariff-API project. The deck is authored as Markdown and rendered with
[Marp](https://marp.app/); it is styled to match the RISE brand.

> This folder is self-contained. Building the deck does **not** touch the
> repository's `package.json`, `dotnet` build, git hooks or any other tooling.

## Files

| Path | What it is |
|------|------------|
| `eltariff-katalog.md` | The deck source — edit this. |
| `assets/rise.css` | Custom Marp theme. RISE colours + the Code Pro / Lato / Roboto Mono fonts and the RISE logo, all embedded as base64 so the deck renders identically offline and in PDF/PPTX. |
| `assets/lookup_flow.mmd` | Mermaid source for the slide-5 sequence diagram. |
| `assets/lookup_flow.svg` | Rendered diagram embedded by the deck (committed, so a normal build needs no Chromium). |
| `assets/rise_logo_*.svg`, `assets/fonts/*.woff2` | Original RISE brand assets (the theme embeds these; kept here for reference). |
| `build.sh` | Renders HTML + PDF + PPTX. |
| `eltariff-katalog.{html,pdf,pptx}` | Build outputs. |

## Prerequisites

- **Node.js** (provides `npx`). Marp is fetched on demand via `npx`, so there's
  nothing to install globally.
- Only for `./build.sh --diagram`: mermaid-cli downloads a headless Chromium the
  first time. Not needed for a normal build.

## Build

```bash
./build.sh            # HTML + PDF + PPTX from the committed sources
./build.sh --diagram  # also regenerate assets/lookup_flow.svg from Mermaid first
```

Outputs are written next to the source in this folder.

## Editing

- **Slides** are separated by `---` in `eltariff-katalog.md`.
- **Speaker notes** live in `<!-- ... -->` HTML comments; Marp exports them as
  presenter notes (and they're ignored on the slide itself).
- **Cover / closing** slides use `<!-- _class: lead -->` (the black-on-yellow
  RISE look defined in `assets/rise.css`).
- **The sequence diagram**: edit `assets/lookup_flow.mmd`, then rebuild with
  `./build.sh --diagram`.
- Live preview while editing: the **Marp for VS Code** extension (point it at
  `assets/rise.css` as the theme).

## PPTX note

Marp's PPTX export renders each slide as a full-bleed **image** — pixel-identical
to the PDF, great for presenting, but the text is not editable in PowerPoint. To
change content, edit the Markdown and rebuild.

## Brand assets

The Code Pro / Lato fonts and the RISE logo are RISE brand assets, included here
for an internal talk about the project. Keep that in mind before redistributing
the deck externally.
