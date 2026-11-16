import { getPendingId } from "./PendingImages";
import type { Descendant, TElement, TText } from "platejs";
import { KEYS } from "platejs";

function escapeHtml(text: string): string {
  return text
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function escapeAttr(text: string): string {
  return escapeHtml(text);
}

function styleFromEntries(entries: Array<[string, string | number | undefined | null]>): string {
  return entries
    .filter(([, v]) => v != null && v !== "")
    .map(([k, v]) => `${k}:${v}`)
    .join(";");
}

function openTag(
  tag: string,
  attrs: Record<string, string | number | undefined | null> = {}
): string {
  const parts = Object.entries(attrs)
    .filter(([, v]) => v != null && v !== "")
    .map(([k, v]) => ` ${k}="${escapeAttr(String(v))}"`);
  return `<${tag}${parts.join("")}>`;
}

function serializeText(node: TText): string {
  let text = escapeHtml(node.text || "");
  if (!text && !node.text) return "";

  const style = styleFromEntries([
    ["color", (node as any).color],
    ["background-color", (node as any).backgroundColor],
    ["font-size", (node as any).fontSize],
  ]);

  if (style) text = `<span style="${style}">${text}</span>`;
  if ((node as any).bold) text = `<strong>${text}</strong>`;
  if ((node as any).italic) text = `<em>${text}</em>`;
  if ((node as any).underline) text = `<u>${text}</u>`;
  if ((node as any).strikethrough) text = `<s>${text}</s>`;
  if ((node as any).code) text = `<code>${text}</code>`;
  if ((node as any).highlight) text = `<mark>${text}</mark>`;
  if ((node as any).kbd) text = `<kbd>${text}</kbd>`;
  if ((node as any).subscript) text = `<sub>${text}</sub>`;
  if ((node as any).superscript) text = `<sup>${text}</sup>`;
  return text;
}

function serializeChildren(children: Descendant[]): string {
  return (children || []).map(serializeNode).join("");
}

function blockStyle(el: TElement): string {
  return styleFromEntries([
    ["text-align", (el as any).align],
    ["line-height", (el as any).lineHeight],
    ["margin-left", (el as any).indent ? `${Number((el as any).indent) * 24}px` : undefined],
    ["background-color", (el as any).backgroundColor],
  ]);
}

function wrapBlock(tag: string, el: TElement, inner: string, extra: Record<string, string> = {}): string {
  const style = blockStyle(el);
  return `${openTag(tag, { ...extra, style: style || undefined })}${inner}</${tag}>`;
}

function isListItem(el: TElement): boolean {
  return !!(el as any).listStyleType;
}

function listTagFor(listStyleType: string): "ul" | "ol" {
  if (
    listStyleType === "disc" ||
    listStyleType === "circle" ||
    listStyleType === "square" ||
    listStyleType === "todo"
  ) {
    return "ul";
  }
  return "ol";
}

/**
 * Group consecutive Plate indent-list paragraphs into ul/ol markup.
 */
function serializeBlocks(nodes: Descendant[]): string {
  let html = "";
  let i = 0;

  while (i < nodes.length) {
    const node = nodes[i];
    if ((node as TText).text !== undefined) {
      html += serializeText(node as TText);
      i += 1;
      continue;
    }

    const el = node as TElement;
    if (!isListItem(el)) {
      html += serializeElement(el);
      i += 1;
      continue;
    }

    const listStyleType = String((el as any).listStyleType);
    const tag = listTagFor(listStyleType);
    const items: string[] = [];

    while (i < nodes.length) {
      const cur = nodes[i] as TElement;
      if ((cur as any).text !== undefined || !isListItem(cur)) break;
      const curType = String((cur as any).listStyleType);
      if (listTagFor(curType) !== tag) break;

      const indent = Number((cur as any).indent) || 1;
      const inner = serializeChildren(cur.children || []);
      const liStyle = styleFromEntries([
        ["list-style-type", curType === "todo" ? "none" : curType],
        ["margin-left", indent > 1 ? `${(indent - 1) * 24}px` : undefined],
        ["text-align", (cur as any).align],
      ]);

      if (curType === "todo") {
        const checked = !!(cur as any).checked;
        items.push(
          `${openTag("li", { style: liStyle || undefined, "data-plate-todo": "true" })}` +
            `<input type="checkbox" disabled${checked ? " checked" : ""} /> ${inner}</li>`
        );
      } else {
        items.push(`${openTag("li", { style: liStyle || undefined })}${inner}</li>`);
      }
      i += 1;
    }

    html += `<${tag}>${items.join("")}</${tag}>`;
  }

  return html;
}

function serializeElement(el: TElement): string {
  const type = el.type;
  const children = el.children || [];

  switch (type) {
    case KEYS.h1:
      return wrapBlock("h1", el, serializeChildren(children));
    case KEYS.h2:
      return wrapBlock("h2", el, serializeChildren(children));
    case KEYS.h3:
      return wrapBlock("h3", el, serializeChildren(children));
    case KEYS.h4:
      return wrapBlock("h4", el, serializeChildren(children));
    case KEYS.h5:
      return wrapBlock("h5", el, serializeChildren(children));
    case KEYS.h6:
      return wrapBlock("h6", el, serializeChildren(children));
    case KEYS.blockquote:
      return wrapBlock("blockquote", el, serializeChildren(children));
    case KEYS.hr:
      return "<hr />";
    case KEYS.a: {
      const url = String((el as any).url || "");
      return `${openTag("a", { href: url })}${serializeChildren(children)}</a>`;
    }
    case KEYS.img: {
      const url = String((el as any).url || "");
      const width = (el as any).width;
      const style = styleFromEntries([
        ["width", width ? (typeof width === "number" ? `${width}px` : width) : undefined],
        ["text-align", (el as any).align],
      ]);
      const pendingId = url.startsWith("blob:") ? getPendingId(url) : undefined;
      return openTag("img", {
        src: url,
        "data-blog-pending": pendingId,
        alt: (el as any).alt || "",
        style: style || undefined,
      }).replace(/>$/, " />");
    }
    case KEYS.video: {
      const url = String((el as any).url || "");
      return `${openTag("video", {
        src: url,
        controls: "controls",
        style: blockStyle(el) || undefined,
        "data-plate-type": "video",
      })}</video>`;
    }
    case KEYS.audio: {
      const url = String((el as any).url || "");
      return `${openTag("audio", {
        src: url,
        controls: "controls",
        "data-plate-type": "audio",
      })}</audio>`;
    }
    case KEYS.file: {
      const url = String((el as any).url || "");
      const name = String((el as any).name || url.split("/").pop() || "file");
      return `${openTag("a", {
        href: url,
        "data-plate-type": "file",
        download: name,
      })}${escapeHtml(name)}</a>`;
    }
    case KEYS.mediaEmbed: {
      const url = String((el as any).url || "");
      return `${openTag("a", {
        href: url,
        "data-plate-type": "media_embed",
      })}${escapeHtml(url)}</a>`;
    }
    case KEYS.codeBlock: {
      const lang = (el as any).lang || (el as any).language || "";
      const lines = children
        .map((line) => {
          if ((line as TText).text !== undefined) return serializeText(line as TText);
          return serializeChildren((line as TElement).children || []);
        })
        .join("\n");
      return `${openTag("pre", { "data-language": lang || undefined })}<code>${lines}</code></pre>`;
    }
    case KEYS.table:
      return `${openTag("table", {
        style: "border-collapse:collapse;width:100%",
        "data-plate-type": "table",
      })}<tbody>${serializeChildren(children)}</tbody></table>`;
    case KEYS.tr:
      return `<tr>${serializeChildren(children)}</tr>`;
    case KEYS.th:
    case KEYS.td: {
      const tag = type === KEYS.th ? "th" : "td";
      const style = styleFromEntries([
        ["background-color", (el as any).background],
        ["border", (el as any).borders ? undefined : "1px solid #ddd"],
        ["padding", "6px"],
      ]);
      return `${openTag(tag, {
        colspan: (el as any).colSpan > 1 ? (el as any).colSpan : undefined,
        rowspan: (el as any).rowSpan > 1 ? (el as any).rowSpan : undefined,
        style: style || undefined,
      })}${serializeChildren(children)}</${tag}>`;
    }
    case KEYS.toggle: {
      const summary =
        serializeChildren(children.slice(0, 1)) || "Toggle";
      const body = serializeBlocks(children.slice(1));
      return `<details data-plate-type="toggle"><summary>${summary}</summary>${body}</details>`;
    }
    case KEYS.callout: {
      const icon = (el as any).icon || "💡";
      const style = styleFromEntries([
        ["background-color", (el as any).backgroundColor || "#f4f4f5"],
        ["padding", "12px"],
        ["border-radius", "6px"],
      ]);
      return `${openTag("aside", {
        "data-plate-type": "callout",
        "data-plate-icon": icon,
        style,
      })}<span data-plate-callout-icon="${escapeAttr(icon)}">${escapeHtml(icon)}</span><div>${serializeChildren(children)}</div></aside>`;
    }
    case KEYS.columnGroup:
      return `${openTag("div", {
        "data-plate-type": "column_group",
        style: "display:flex;gap:16px",
      })}${serializeChildren(children)}</div>`;
    case KEYS.column: {
      const width = (el as any).width;
      return `${openTag("div", {
        "data-plate-type": "column",
        style: styleFromEntries([
          ["flex", width ? `0 0 ${width}` : "1"],
        ]),
      })}${serializeBlocks(children)}</div>`;
    }
    case KEYS.equation: {
      const tex = String((el as any).texExpression || "");
      return `${openTag("div", {
        "data-plate-type": "equation",
        "data-slate-tex-expression": tex,
      })}${escapeHtml(tex)}</div>`;
    }
    case KEYS.inlineEquation: {
      const tex = String((el as any).texExpression || "");
      return `${openTag("span", {
        "data-plate-type": "inline_equation",
        "data-slate-tex-expression": tex,
      })}${escapeHtml(tex)}</span>`;
    }
    case KEYS.date: {
      const date = String((el as any).date || "");
      return `${openTag("time", {
        datetime: date,
        "data-plate-type": "date",
      })}${escapeHtml(date)}</time>`;
    }
    case KEYS.toc:
      return `${openTag("nav", { "data-plate-type": "toc" })}Table of contents</nav>`;
    case "footnoteReference":
    case KEYS.footnoteReference as string: {
      const id = String((el as any).id || (el as any).footnoteId || "");
      return `${openTag("sup", {
        "data-plate-type": "footnoteReference",
        "data-footnote-id": id,
      })}[${escapeHtml(id)}]</sup>`;
    }
    case "footnoteDefinition":
    case KEYS.footnoteDefinition as string: {
      const id = String((el as any).id || "");
      return `${openTag("aside", {
        "data-plate-type": "footnoteDefinition",
        "data-footnote-id": id,
        id: id ? `fn-${id}` : undefined,
      })}${serializeChildren(children)}</aside>`;
    }
    case KEYS.p:
    default:
      return wrapBlock("p", el, serializeChildren(children));
  }
}

function serializeNode(node: Descendant): string {
  if ((node as TText).text !== undefined) {
    return serializeText(node as TText);
  }
  return serializeElement(node as TElement);
}

export function serializeNodesToHtml(nodes: Descendant[]): string {
  if (!nodes || nodes.length === 0) return "";
  return serializeBlocks(nodes);
}
