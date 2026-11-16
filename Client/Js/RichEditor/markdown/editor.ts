import { EditorView as CmView, keymap, dropCursor, drawSelection } from "@codemirror/view";
import { EditorState as CmState, Prec } from "@codemirror/state";
import { defaultKeymap, history, historyKeymap, indentWithTab } from "@codemirror/commands";
import { markdown, markdownLanguage } from "@codemirror/lang-markdown";
import { LanguageDescription } from "@codemirror/language";
import MarkdownIt from "markdown-it";
import DOMPurify from "dompurify";
import type { EditorEngine, EngineHost } from "../core/types";
import { isImageFile } from "../core/upload";

/** Nested fence highlighting for common languages (avoids full language-data bundle). */
const fenceLanguages = [
  LanguageDescription.of({
    name: "JavaScript",
    alias: ["js", "javascript", "jsx"],
    load: () => import("@codemirror/lang-javascript").then((m) => m.javascript()),
  }),
  LanguageDescription.of({
    name: "TypeScript",
    alias: ["ts", "typescript", "tsx"],
    load: () =>
      import("@codemirror/lang-javascript").then((m) =>
        m.javascript({ typescript: true })
      ),
  }),
  LanguageDescription.of({
    name: "HTML",
    alias: ["html", "htm"],
    load: () => import("@codemirror/lang-html").then((m) => m.html()),
  }),
  LanguageDescription.of({
    name: "CSS",
    alias: ["css"],
    load: () => import("@codemirror/lang-css").then((m) => m.css()),
  }),
  LanguageDescription.of({
    name: "JSON",
    alias: ["json"],
    load: () => import("@codemirror/lang-json").then((m) => m.json()),
  }),
  LanguageDescription.of({
    name: "Python",
    alias: ["py", "python"],
    load: () => import("@codemirror/lang-python").then((m) => m.python()),
  }),
  LanguageDescription.of({
    name: "SQL",
    alias: ["sql"],
    load: () => import("@codemirror/lang-sql").then((m) => m.sql()),
  }),
  LanguageDescription.of({
    name: "XML",
    alias: ["xml"],
    load: () => import("@codemirror/lang-xml").then((m) => m.xml()),
  }),
  LanguageDescription.of({
    name: "Markdown",
    alias: ["md", "markdown"],
    support: markdown({ base: markdownLanguage }),
  }),
];

const md = new MarkdownIt({ html: false, linkify: true, breaks: true });

function renderPreview(source: string): string {
  const html = md.render(source || "");
  return DOMPurify.sanitize(html, {
    ALLOWED_TAGS: [
      "p", "br", "h1", "h2", "h3", "h4", "h5", "h6",
      "strong", "em", "s", "a", "ul", "ol", "li",
      "table", "thead", "tbody", "tr", "th", "td",
      "img", "pre", "code", "blockquote", "hr",
    ],
    ALLOWED_ATTR: ["href", "title", "src", "alt", "colspan", "rowspan"],
  }) as string;
}

function wrapSelection(view: CmView, before: string, after: string = before): void {
  const { from, to } = view.state.selection.main;
  const text = view.state.sliceDoc(from, to);
  view.dispatch({
    changes: { from, to, insert: before + text + after },
    selection: {
      anchor: from + before.length,
      head: from + before.length + text.length,
    },
  });
  view.focus();
}

async function uploadAndInsert(host: EngineHost, view: CmView, file: File, pos?: number): Promise<void> {
  try {
    const result = await host.upload(file);
    const name = result.fileName || file.name;
    const snippet = isImageFile(file)
      ? `![${name}](${result.url})`
      : `[${name}](${result.url})`;
    const insertAt = pos ?? view.state.selection.main.head;
    view.dispatch({
      changes: { from: insertAt, insert: snippet },
      selection: { anchor: insertAt + snippet.length },
    });
    host.onChange(view.state.doc.toString());
  } catch (err: any) {
    host.onError(err?.message || "Upload failed");
  }
}

export function createMarkdownEngine(host: EngineHost): EditorEngine {
  let preview = false;
  let cm: CmView | null = null;

  const shell = document.createElement("div");
  shell.className = "bre__chrome";

  const toolbar = document.createElement("div");
  toolbar.className = "bre__toolbar";

  const addBtn = (label: string, title: string, fn: () => void) => {
    const b = document.createElement("button");
    b.type = "button";
    b.className = "bre__btn";
    b.textContent = label;
    b.title = title;
    b.addEventListener("mousedown", (e) => {
      e.preventDefault();
      fn();
    });
    toolbar.appendChild(b);
    return b;
  };

  const group = document.createElement("div");
  group.className = "bre__group";
  toolbar.appendChild(group);

  const mk = (label: string, title: string, fn: () => void) => {
    const b = document.createElement("button");
    b.type = "button";
    b.className = "bre__btn";
    b.textContent = label;
    b.title = title;
    b.addEventListener("mousedown", (e) => {
      e.preventDefault();
      fn();
    });
    group.appendChild(b);
  };

  mk("B", "Bold", () => cm && wrapSelection(cm, "**"));
  mk("I", "Italic", () => cm && wrapSelection(cm, "_"));
  mk("`code`", "Inline code", () => cm && wrapSelection(cm, "`"));
  mk("```", "Code fence", () => cm && wrapSelection(cm, "```\n", "\n```"));
  mk("Link", "Link", () => {
    if (!cm) return;
    const url = window.prompt("URL", "https://");
    if (!url) return;
    wrapSelection(cm, "[", `](${url})`);
  });

  const fileInput = document.createElement("input");
  fileInput.type = "file";
  fileInput.className = "bre__hidden-input";
  fileInput.accept = "image/*,.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.zip,.csv";
  mk("Upload", "Upload image or file", () => fileInput.click());
  fileInput.addEventListener("change", () => {
    const file = fileInput.files?.[0];
    fileInput.value = "";
    if (file && cm) void uploadAndInsert(host, cm, file);
  });

  const spacer = document.createElement("div");
  spacer.className = "bre__spacer";
  toolbar.appendChild(spacer);

  const previewBtn = addBtn("Preview", "Toggle preview", () => {
    preview = !preview;
    previewBtn.classList.toggle("is-active", preview);
    syncPanels();
  });

  const body = document.createElement("div");
  body.className = "bre__body";
  const editorMount = document.createElement("div");
  editorMount.className = "bre__cm";
  const previewEl = document.createElement("div");
  previewEl.className = "bre__preview";
  previewEl.hidden = true;
  body.appendChild(editorMount);
  body.appendChild(previewEl);

  shell.appendChild(toolbar);
  shell.appendChild(fileInput);
  shell.appendChild(body);
  host.mount.appendChild(shell);

  const syncPanels = () => {
    editorMount.hidden = preview;
    previewEl.hidden = !preview;
    if (preview && cm) previewEl.innerHTML = renderPreview(cm.state.doc.toString());
  };

  const dropHandler = Prec.high(
    CmView.domEventHandlers({
      drop(event, view) {
        const files = event.dataTransfer?.files;
        if (!files?.length) return false;
        event.preventDefault();
        const pos = view.posAtCoords({ x: event.clientX, y: event.clientY });
        void uploadAndInsert(host, view, files[0], pos ?? undefined);
        return true;
      },
      paste(event, view) {
        const files = event.clipboardData?.files;
        if (!files?.length) return false;
        event.preventDefault();
        void uploadAndInsert(host, view, files[0]);
        return true;
      },
    })
  );

  cm = new CmView({
    parent: editorMount,
    state: CmState.create({
      doc: host.initialValue || "",
      extensions: [
        history(),
        drawSelection(),
        dropCursor(),
        keymap.of([...defaultKeymap, ...historyKeymap, indentWithTab]),
        markdown({ codeLanguages: fenceLanguages }),
        CmView.updateListener.of((update) => {
          if (update.docChanged) host.onChange(update.state.doc.toString());
        }),
        CmView.editable.of(!host.disabled),
        dropHandler,
        CmView.theme({
          "&": { height: "100%" },
          ".cm-content": { caretColor: "#0f6e56" },
        }),
      ],
    }),
  });

  return {
    getValue: () => cm?.state.doc.toString() ?? "",
    setValue: (value: string) => {
      if (!cm) return;
      const next = value || "";
      if (cm.state.doc.toString() === next) return;
      const sel = cm.state.selection.main;
      const head = Math.min(sel.head, next.length);
      const anchor = Math.min(sel.anchor, next.length);
      cm.dispatch({
        changes: { from: 0, to: cm.state.doc.length, insert: next },
        selection: { anchor, head },
      });
      if (preview) previewEl.innerHTML = renderPreview(next);
    },
    focus: () => cm?.focus(),
    destroy: () => {
      cm?.destroy();
      cm = null;
      shell.remove();
    },
  };
}
