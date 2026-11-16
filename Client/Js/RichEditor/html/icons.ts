/** Inline SVG toolbar icons (24x24 viewBox, currentColor + accents). */

const svg = (body: string, title?: string) =>
  `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"${title ? ` title="${title}"` : ""}>${body}</svg>`;

export const icons = {
  bold: svg(`<path d="M6 4h8a4 4 0 0 1 0 8H6z"/><path d="M6 12h9a4 4 0 0 1 0 8H6z"/>`),
  italic: svg(`<line x1="19" y1="4" x2="10" y2="4"/><line x1="14" y1="20" x2="5" y2="20"/><line x1="15" y1="4" x2="9" y2="20"/>`),
  strikethrough: svg(
    `<path d="M16 4H9a3 3 0 0 0 0 6h6a3 3 0 0 1 0 6H8"/><line x1="4" y1="12" x2="20" y2="12"/>`
  ),
  underline: svg(`<path d="M6 4v6a6 6 0 0 0 12 0V4"/><line x1="4" y1="20" x2="20" y2="20"/>`),
  subscript: svg(
    `<path d="m4 5 8 6-8 6"/><path d="m12 19 4-2 4 2"/><path d="M16 17v4"/>`
  ),
  superscript: svg(
    `<path d="m4 19 8-6-8-6"/><path d="m12 5 4-2 4 2"/><path d="M16 3v4"/>`
  ),
  list: svg(
    `<line x1="8" y1="6" x2="21" y2="6"/><line x1="8" y1="12" x2="21" y2="12"/><line x1="8" y1="18" x2="21" y2="18"/><line x1="3" y1="6" x2="3.01" y2="6"/><line x1="3" y1="12" x2="3.01" y2="12"/><line x1="3" y1="18" x2="3.01" y2="18"/>`
  ),
  table: svg(
    `<rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M3 15h18M9 3v18M15 3v18"/>`
  ),
  link: svg(
    `<path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"/><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"/>`
  ),
  upload: svg(
    `<path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="17 8 12 3 7 8"/><line x1="12" y1="3" x2="12" y2="15"/>`,
  ),
  color: svg(
    `<circle cx="12" cy="12" r="9" fill="#0f6e56" stroke="#0f6e56"/><path d="M8 14s1.5 2 4 2 4-2 4-2" stroke="#fff"/><line x1="9" y1="9" x2="9.01" y2="9" stroke="#fff"/><line x1="15" y1="9" x2="15.01" y2="9" stroke="#fff"/>`
  ),
  chevron: svg(`<polyline points="6 9 12 15 18 9"/>`),
};
