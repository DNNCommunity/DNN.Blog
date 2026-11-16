import { DOMParser, DOMSerializer, type Node as PMNode } from "prosemirror-model";
import DOMPurify, { type Config } from "dompurify";
import { blogSchema } from "./schema";

const PURIFY_CONFIG: Config = {
  ALLOWED_TAGS: [
    "p",
    "br",
    "h1",
    "h2",
    "h3",
    "h4",
    "h5",
    "h6",
    "strong",
    "b",
    "em",
    "i",
    "u",
    "s",
    "strike",
    "del",
    "sub",
    "sup",
    "a",
    "ul",
    "ol",
    "li",
    "table",
    "thead",
    "tbody",
    "tr",
    "th",
    "td",
    "img",
    "pre",
    "code",
    "blockquote",
    "hr",
    "span",
  ],
  ALLOWED_ATTR: [
    "href",
    "title",
    "target",
    "rel",
    "src",
    "alt",
    "colspan",
    "rowspan",
    "style",
    "data-text-color",
    "data-font-size",
    "start",
  ],
  ALLOW_DATA_ATTR: false,
};

export function htmlToDoc(html: string): PMNode {
  const clean = DOMPurify.sanitize(html || "", PURIFY_CONFIG);
  const wrap = document.createElement("div");
  wrap.innerHTML = clean.trim() ? clean : "<p></p>";
  return DOMParser.fromSchema(blogSchema).parse(wrap);
}

export function docToHtml(doc: PMNode): string {
  const fragment = DOMSerializer.fromSchema(blogSchema).serializeFragment(
    doc.content
  );
  const wrap = document.createElement("div");
  wrap.appendChild(fragment);
  return wrap.innerHTML;
}

export function sanitizePastedHtml(html: string): string {
  return DOMPurify.sanitize(html || "", PURIFY_CONFIG) as string;
}
