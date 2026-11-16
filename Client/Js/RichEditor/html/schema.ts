import { Schema, type NodeSpec, type MarkSpec } from "prosemirror-model";
import { tableNodes } from "prosemirror-tables";

const ALLOWED_COLORS = [
  "#1c2430",
  "#5c6675",
  "#a33b2b",
  "#0f6e56",
  "#1a4f8c",
  "#8a5a00",
  "#ffffff",
];

const ALLOWED_SIZES = ["0.85em", "1em", "1.15em", "1.35em", "1.6em"];

const nodes: Record<string, NodeSpec> = {
  doc: { content: "block+" },
  paragraph: {
    content: "inline*",
    group: "block",
    parseDOM: [{ tag: "p" }],
    toDOM() {
      return ["p", 0];
    },
  },
  heading: {
    attrs: { level: { default: 1 } },
    content: "inline*",
    group: "block",
    defining: true,
    parseDOM: [
      { tag: "h1", attrs: { level: 1 } },
      { tag: "h2", attrs: { level: 2 } },
      { tag: "h3", attrs: { level: 3 } },
      { tag: "h4", attrs: { level: 4 } },
      { tag: "h5", attrs: { level: 5 } },
      { tag: "h6", attrs: { level: 6 } },
    ],
    toDOM(node) {
      return ["h" + node.attrs.level, 0];
    },
  },
  blockquote: {
    content: "block+",
    group: "block",
    defining: true,
    parseDOM: [{ tag: "blockquote" }],
    toDOM() {
      return ["blockquote", 0];
    },
  },
  horizontal_rule: {
    group: "block",
    parseDOM: [{ tag: "hr" }],
    toDOM() {
      return ["hr"];
    },
  },
  pre: {
    content: "text*",
    marks: "",
    group: "block",
    code: true,
    defining: true,
    parseDOM: [{ tag: "pre", preserveWhitespace: "full" as const }],
    toDOM() {
      return ["pre", ["code", 0]];
    },
  },
  text: { group: "inline" },
  hard_break: {
    inline: true,
    group: "inline",
    selectable: false,
    parseDOM: [{ tag: "br" }],
    toDOM() {
      return ["br"];
    },
  },
  image: {
    inline: true,
    attrs: {
      src: {},
      alt: { default: null },
      title: { default: null },
    },
    group: "inline",
    draggable: true,
    parseDOM: [
      {
        tag: "img[src]",
        getAttrs(dom: HTMLElement | string) {
          if (typeof dom === "string") return false;
          return {
            src: dom.getAttribute("src"),
            alt: dom.getAttribute("alt"),
            title: dom.getAttribute("title"),
          };
        },
      },
    ],
    toDOM(node) {
      return ["img", node.attrs];
    },
  },
  ...tableNodes({
    tableGroup: "block",
    cellContent: "block+",
    cellAttributes: {
      background: {
        default: null,
        getFromDOM(dom) {
          return (dom as HTMLElement).style.backgroundColor || null;
        },
        setDOMAttr(value, attrs) {
          if (value)
            attrs.style = (attrs.style || "") + `background-color: ${value};`;
        },
      },
    },
  }),
};

const marks: Record<string, MarkSpec> = {
  link: {
    attrs: {
      href: {},
      title: { default: null },
      target: { default: null },
    },
    inclusive: false,
    parseDOM: [
      {
        tag: "a[href]",
        getAttrs(dom: HTMLElement | string) {
          if (typeof dom === "string") return false;
          return {
            href: dom.getAttribute("href"),
            title: dom.getAttribute("title"),
            target: dom.getAttribute("target"),
          };
        },
      },
    ],
    toDOM(node) {
      const { href, title, target } = node.attrs;
      return ["a", { href, title, target, rel: "noopener noreferrer" }, 0];
    },
  },
  strong: {
    parseDOM: [
      { tag: "strong" },
      {
        tag: "b",
        getAttrs: (node: HTMLElement | string) =>
          typeof node === "string"
            ? false
            : (node as HTMLElement).style.fontWeight !== "normal" && null,
      },
      {
        style: "font-weight=400",
        clearMark: (m) => m.type.name === "strong",
      },
      {
        style: "font-weight",
        getAttrs: (value: string | HTMLElement) =>
          typeof value === "string" && /^(bold(er)?|[5-9]\d{2,})$/.test(value)
            ? null
            : false,
      },
    ],
    toDOM() {
      return ["strong", 0];
    },
  },
  em: {
    parseDOM: [{ tag: "i" }, { tag: "em" }, { style: "font-style=italic" }],
    toDOM() {
      return ["em", 0];
    },
  },
  strikethrough: {
    parseDOM: [
      { tag: "s" },
      { tag: "strike" },
      { tag: "del" },
      { style: "text-decoration=line-through" },
    ],
    toDOM() {
      return ["s", 0];
    },
  },
  underline: {
    parseDOM: [{ tag: "u" }, { style: "text-decoration=underline" }],
    toDOM() {
      return ["u", 0];
    },
  },
  code: {
    parseDOM: [{ tag: "code" }],
    toDOM() {
      return ["code", 0];
    },
  },
  subscript: {
    excludes: "superscript",
    parseDOM: [{ tag: "sub" }, { style: "vertical-align=sub" }],
    toDOM() {
      return ["sub", 0];
    },
  },
  superscript: {
    excludes: "subscript",
    parseDOM: [{ tag: "sup" }, { style: "vertical-align=super" }],
    toDOM() {
      return ["sup", 0];
    },
  },
  textColor: {
    attrs: { color: {} },
    parseDOM: [
      {
        style: "color",
        getAttrs: (value: string | HTMLElement) => {
          if (typeof value !== "string") return false;
          const c = value.toLowerCase();
          return ALLOWED_COLORS.includes(c) ? { color: c } : false;
        },
      },
      {
        tag: "span[data-text-color]",
        getAttrs: (dom: HTMLElement | string) => {
          if (typeof dom === "string") return false;
          const c = (dom.getAttribute("data-text-color") || "").toLowerCase();
          return ALLOWED_COLORS.includes(c) ? { color: c } : false;
        },
      },
    ],
    toDOM(node) {
      return [
        "span",
        {
          "data-text-color": node.attrs.color,
          style: `color: ${node.attrs.color}`,
        },
        0,
      ];
    },
  },
  fontSize: {
    attrs: { size: {} },
    parseDOM: [
      {
        style: "font-size",
        getAttrs: (value: string | HTMLElement) => {
          if (typeof value !== "string") return false;
          return ALLOWED_SIZES.includes(value) ? { size: value } : false;
        },
      },
      {
        tag: "span[data-font-size]",
        getAttrs: (dom: HTMLElement | string) => {
          if (typeof dom === "string") return false;
          const s = dom.getAttribute("data-font-size") || "";
          return ALLOWED_SIZES.includes(s) ? { size: s } : false;
        },
      },
    ],
    toDOM(node) {
      return [
        "span",
        {
          "data-font-size": node.attrs.size,
          style: `font-size: ${node.attrs.size}`,
        },
        0,
      ];
    },
  },
};

export const blogSchema = new Schema({
  nodes: {
    ...nodes,
    // list nodes from schema-list added via ordered/bullet below
    ordered_list: {
      content: "list_item+",
      group: "block",
      attrs: { order: { default: 1 } },
      parseDOM: [
        {
          tag: "ol",
          getAttrs(dom: HTMLElement | string) {
            if (typeof dom === "string") return false;
            return {
              order: (dom as HTMLOListElement).start || 1,
            };
          },
        },
      ],
      toDOM(node) {
        return node.attrs.order === 1
          ? ["ol", 0]
          : ["ol", { start: node.attrs.order }, 0];
      },
    },
    bullet_list: {
      content: "list_item+",
      group: "block",
      parseDOM: [{ tag: "ul" }],
      toDOM() {
        return ["ul", 0];
      },
    },
    list_item: {
      content: "paragraph block*",
      defining: true,
      parseDOM: [{ tag: "li" }],
      toDOM() {
        return ["li", 0];
      },
    },
  },
  marks,
});

export { ALLOWED_COLORS, ALLOWED_SIZES };
