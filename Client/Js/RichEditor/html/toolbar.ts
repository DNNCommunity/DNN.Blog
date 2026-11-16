import { EditorState, type Transaction } from "prosemirror-state";
import { EditorView } from "prosemirror-view";
import { toggleMark, setBlockType } from "prosemirror-commands";
import { wrapInList, liftListItem, sinkListItem } from "prosemirror-schema-list";
import {
  addColumnAfter,
  addRowAfter,
  deleteTable,
} from "prosemirror-tables";
import type { MarkType } from "prosemirror-model";
import { blogSchema, ALLOWED_COLORS, ALLOWED_SIZES } from "./schema";
import { docToHtml } from "./serialize";
import type { EngineHost, UploadResult } from "../core/types";
import { isImageFile, normalizeUploadResult } from "../core/upload";
import { icons } from "./icons";

export type ToolbarHandle = HTMLElement & { _refresh?: () => void };

function markActive(state: EditorState, type: MarkType): boolean {
  const { from, $from, to, empty } = state.selection;
  if (empty) return !!type.isInSet(state.storedMarks || $from.marks());
  return state.doc.rangeHasMark(from, to, type);
}

function setMarkAttr(type: MarkType, attrs: Record<string, unknown>) {
  return (state: EditorState, dispatch?: (tr: Transaction) => void) => {
    const { empty, $cursor, ranges } = state.selection as any;
    if (empty && !$cursor) return false;
    if (dispatch) {
      if ($cursor) {
        dispatch(state.tr.addStoredMark(type.create(attrs)));
      } else {
        let tr = state.tr;
        for (const range of ranges) {
          tr = tr.addMark(range.$from.pos, range.$to.pos, type.create(attrs));
        }
        dispatch(tr.scrollIntoView());
      }
    }
    return true;
  };
}

function clearMark(type: MarkType) {
  return (state: EditorState, dispatch?: (tr: Transaction) => void) => {
    const { empty, $cursor, ranges } = state.selection as any;
    if (empty && !$cursor) return false;
    if (dispatch) {
      if ($cursor) {
        dispatch(state.tr.removeStoredMark(type));
      } else {
        let tr = state.tr;
        for (const range of ranges) {
          tr = tr.removeMark(range.$from.pos, range.$to.pos, type);
        }
        dispatch(tr.scrollIntoView());
      }
    }
    return true;
  };
}

function insertUploaded(
  view: EditorView,
  host: EngineHost,
  file: File,
  result: UploadResult
): void {
  const normalized = normalizeUploadResult(result);
  if (isImageFile(file)) {
    const node = blogSchema.nodes.image.create({
      src: normalized.url,
      alt: normalized.fileName || file.name,
    });
    view.dispatch(view.state.tr.replaceSelectionWith(node).scrollIntoView());
  } else {
    const mark = blogSchema.marks.link.create({ href: normalized.url });
    const text = blogSchema.text(normalized.fileName || file.name, [mark]);
    view.dispatch(view.state.tr.replaceSelectionWith(text, false).scrollIntoView());
  }
  view.focus();
  host.onChange(docToHtml(view.state.doc));
}

export function createToolbar(
  root: ShadowRoot | HTMLElement,
  getView: () => EditorView | null,
  host: EngineHost
): ToolbarHandle {
  const bar = document.createElement("div") as ToolbarHandle;
  bar.className = "bre__toolbar";
  bar.setAttribute("role", "toolbar");

  const openMenus = new Set<HTMLElement>();

  const closeAllMenus = (except?: HTMLElement) => {
    openMenus.forEach((wrap) => {
      if (wrap === except) return;
      wrap.classList.remove("is-open");
      const btn = wrap.querySelector(".bre__btn") as HTMLButtonElement | null;
      btn?.setAttribute("aria-expanded", "false");
    });
  };

  const onDocPointer = (e: Event) => {
    const t = e.target as Node | null;
    for (const wrap of Array.from(openMenus)) {
      if (!wrap.contains(t)) {
        wrap.classList.remove("is-open");
        wrap.querySelector(".bre__btn")?.setAttribute("aria-expanded", "false");
        openMenus.delete(wrap);
      }
    }
  };
  const onEsc = (e: KeyboardEvent) => {
    if (e.key === "Escape") closeAllMenus();
  };
  root.addEventListener("mousedown", onDocPointer);
  root.addEventListener("keydown", onEsc as EventListener);

  const fileInput = document.createElement("input");
  fileInput.type = "file";
  fileInput.className = "bre__hidden-input";
  fileInput.accept =
    "image/jpeg,image/png,image/gif,.jpg,.jpeg,.png,.gif,.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.zip,.csv";
  fileInput.title =
    "Upload an image or file (images appear inline; other files become links)";
  root.appendChild(fileInput);

  const addGroup = () => {
    const g = document.createElement("div");
    g.className = "bre__group";
    bar.appendChild(g);
    return g;
  };

  const iconBtn = (
    group: HTMLElement,
    iconHtml: string,
    title: string,
    onClick: () => void,
    isActive?: () => boolean
  ) => {
    const b = document.createElement("button");
    b.type = "button";
    b.className = "bre__btn bre__btn--icon";
    b.title = title;
    b.setAttribute("aria-label", title);
    b.innerHTML = iconHtml;
    b.addEventListener("mousedown", (e) => {
      e.preventDefault();
      onClick();
      refresh();
    });
    (b as any)._isActive = isActive;
    group.appendChild(b);
    return b;
  };

  const select = (
    group: HTMLElement,
    options: { value: string; label: string }[],
    title: string,
    onChange: (value: string) => void
  ) => {
    const s = document.createElement("select");
    s.className = "bre__select";
    s.title = title;
    for (const o of options) {
      const opt = document.createElement("option");
      opt.value = o.value;
      opt.textContent = o.label;
      s.appendChild(opt);
    }
    s.addEventListener("change", () => {
      onChange(s.value);
      refresh();
    });
    group.appendChild(s);
    return s;
  };

  const menu = (
    group: HTMLElement,
    iconHtml: string,
    title: string,
    items: { label: string; action: () => void }[]
  ) => {
    const wrap = document.createElement("div");
    wrap.className = "bre__menu";
    const b = document.createElement("button");
    b.type = "button";
    b.className = "bre__btn bre__btn--icon";
    b.title = title;
    b.setAttribute("aria-label", title);
    b.setAttribute("aria-haspopup", "true");
    b.setAttribute("aria-expanded", "false");
    b.innerHTML = iconHtml + `<span class="bre__chevron">${icons.chevron}</span>`;
    const panel = document.createElement("div");
    panel.className = "bre__menu-panel";
    panel.setAttribute("role", "menu");
    for (const item of items) {
      const mi = document.createElement("button");
      mi.type = "button";
      mi.className = "bre__menu-item";
      mi.setAttribute("role", "menuitem");
      mi.textContent = item.label;
      mi.addEventListener("mousedown", (e) => {
        e.preventDefault();
        item.action();
        wrap.classList.remove("is-open");
        b.setAttribute("aria-expanded", "false");
        openMenus.delete(wrap);
        refresh();
      });
      panel.appendChild(mi);
    }
    b.addEventListener("mousedown", (e) => {
      e.preventDefault();
      const open = !wrap.classList.contains("is-open");
      closeAllMenus(wrap);
      wrap.classList.toggle("is-open", open);
      b.setAttribute("aria-expanded", open ? "true" : "false");
      if (open) openMenus.add(wrap);
      else openMenus.delete(wrap);
    });
    wrap.appendChild(b);
    wrap.appendChild(panel);
    group.appendChild(wrap);
    return wrap;
  };

  const run = (
    cmd: (
      state: EditorState,
      dispatch?: (tr: Transaction) => void,
      view?: EditorView
    ) => boolean
  ) => {
    const view = getView();
    if (!view || !view.editable) return;
    cmd(view.state, view.dispatch.bind(view), view);
    view.focus();
  };

  const insertTable = () => {
    const view = getView();
    if (!view) return;
    const { table, table_row, table_cell, table_header } = blogSchema.nodes;
    const cell = () => table_cell.createAndFill()!;
    const header = () => table_header.createAndFill()!;
    const row = (...cells: ReturnType<typeof cell>[]) =>
      table_row.create(null, cells);
    const node = table.create(null, [
      row(header(), header(), header()),
      row(cell(), cell(), cell()),
      row(cell(), cell(), cell()),
    ]);
    view.dispatch(view.state.tr.replaceSelectionWith(node).scrollIntoView());
    view.focus();
  };

  // 1. Style select
  const g1 = addGroup();
  const blockSelect = select(
    g1,
    [
      { value: "p", label: "Paragraph" },
      { value: "h1", label: "Heading 1" },
      { value: "h2", label: "Heading 2" },
      { value: "h3", label: "Heading 3" },
      { value: "h4", label: "Heading 4" },
      { value: "h5", label: "Heading 5" },
      { value: "h6", label: "Heading 6" },
      { value: "pre", label: "Pre" },
    ],
    "Block type",
    (value) => {
      if (value === "p") run(setBlockType(blogSchema.nodes.paragraph));
      else if (value === "pre") run(setBlockType(blogSchema.nodes.pre));
      else {
        const level = parseInt(value.slice(1), 10);
        run(setBlockType(blogSchema.nodes.heading, { level }));
      }
    }
  );

  // 2. Marks
  const g2 = addGroup();
  iconBtn(
    g2,
    icons.bold,
    "Bold",
    () => run(toggleMark(blogSchema.marks.strong)),
    () => {
      const v = getView();
      return !!v && markActive(v.state, blogSchema.marks.strong);
    }
  );
  iconBtn(
    g2,
    icons.italic,
    "Italic",
    () => run(toggleMark(blogSchema.marks.em)),
    () => {
      const v = getView();
      return !!v && markActive(v.state, blogSchema.marks.em);
    }
  );
  iconBtn(
    g2,
    icons.strikethrough,
    "Strikethrough",
    () => run(toggleMark(blogSchema.marks.strikethrough)),
    () => {
      const v = getView();
      return !!v && markActive(v.state, blogSchema.marks.strikethrough);
    }
  );
  iconBtn(
    g2,
    icons.underline,
    "Underline",
    () => run(toggleMark(blogSchema.marks.underline)),
    () => {
      const v = getView();
      return !!v && markActive(v.state, blogSchema.marks.underline);
    }
  );
  iconBtn(
    g2,
    icons.subscript,
    "Subscript",
    () => run(toggleMark(blogSchema.marks.subscript)),
    () => {
      const v = getView();
      return !!v && markActive(v.state, blogSchema.marks.subscript);
    }
  );
  iconBtn(
    g2,
    icons.superscript,
    "Superscript",
    () => run(toggleMark(blogSchema.marks.superscript)),
    () => {
      const v = getView();
      return !!v && markActive(v.state, blogSchema.marks.superscript);
    }
  );

  // 3. Lists dropdown
  const g3 = addGroup();
  menu(g3, icons.list, "Lists", [
    {
      label: "Bullet list",
      action: () => run(wrapInList(blogSchema.nodes.bullet_list)),
    },
    {
      label: "Numbered list",
      action: () => run(wrapInList(blogSchema.nodes.ordered_list)),
    },
    {
      label: "Outdent",
      action: () => run(liftListItem(blogSchema.nodes.list_item)),
    },
    {
      label: "Indent",
      action: () => run(sinkListItem(blogSchema.nodes.list_item)),
    },
  ]);

  // 4. Table dropdown
  const g4 = addGroup();
  menu(g4, icons.table, "Table", [
    { label: "Insert table", action: insertTable },
    { label: "Add column", action: () => run(addColumnAfter) },
    { label: "Add row", action: () => run(addRowAfter) },
    { label: "Delete table", action: () => run(deleteTable) },
  ]);

  // 5. Color swatch picker + size
  const g5 = addGroup();
  const colorWrap = document.createElement("div");
  colorWrap.className = "bre__menu bre__color-picker";
  const colorBtn = document.createElement("button");
  colorBtn.type = "button";
  colorBtn.className = "bre__btn bre__btn--icon";
  colorBtn.title = "Text color";
  colorBtn.setAttribute("aria-label", "Text color");
  colorBtn.setAttribute("aria-haspopup", "true");
  colorBtn.setAttribute("aria-expanded", "false");
  colorBtn.innerHTML = icons.color;
  const colorPanel = document.createElement("div");
  colorPanel.className = "bre__menu-panel bre__swatch-panel";
  colorPanel.setAttribute("role", "menu");

  const clearBtn = document.createElement("button");
  clearBtn.type = "button";
  clearBtn.className = "bre__menu-item";
  clearBtn.textContent = "Default";
  clearBtn.addEventListener("mousedown", (e) => {
    e.preventDefault();
    run(clearMark(blogSchema.marks.textColor));
    colorWrap.classList.remove("is-open");
    colorBtn.setAttribute("aria-expanded", "false");
    openMenus.delete(colorWrap);
    refresh();
  });
  colorPanel.appendChild(clearBtn);

  for (const c of ALLOWED_COLORS) {
    const sw = document.createElement("button");
    sw.type = "button";
    sw.className = "bre__swatch";
    sw.title = c;
    sw.setAttribute("aria-label", c);
    sw.style.background = c;
    if (c.toLowerCase() === "#ffffff") sw.classList.add("bre__swatch--light");
    sw.addEventListener("mousedown", (e) => {
      e.preventDefault();
      run(setMarkAttr(blogSchema.marks.textColor, { color: c }));
      colorWrap.classList.remove("is-open");
      colorBtn.setAttribute("aria-expanded", "false");
      openMenus.delete(colorWrap);
      refresh();
    });
    colorPanel.appendChild(sw);
  }

  colorBtn.addEventListener("mousedown", (e) => {
    e.preventDefault();
    const open = !colorWrap.classList.contains("is-open");
    closeAllMenus(colorWrap);
    colorWrap.classList.toggle("is-open", open);
    colorBtn.setAttribute("aria-expanded", open ? "true" : "false");
    if (open) openMenus.add(colorWrap);
    else openMenus.delete(colorWrap);
  });
  colorWrap.appendChild(colorBtn);
  colorWrap.appendChild(colorPanel);
  g5.appendChild(colorWrap);

  select(
    g5,
    [{ value: "", label: "Size" }, ...ALLOWED_SIZES.map((s) => ({ value: s, label: s }))],
    "Font size",
    (value) => {
      if (!value) return;
      run(setMarkAttr(blogSchema.marks.fontSize, { size: value }));
    }
  );

  // 6. Link
  const g6 = addGroup();
  iconBtn(g6, icons.link, "Insert link", () => {
    const view = getView();
    if (!view) return;
    const href = window.prompt("Link URL", "https://");
    if (!href) return;
    const { empty } = view.state.selection;
    if (empty) {
      const text = window.prompt("Link text", href) || href;
      const mark = blogSchema.marks.link.create({ href });
      const node = blogSchema.text(text, [mark]);
      view.dispatch(
        view.state.tr.replaceSelectionWith(node, false).scrollIntoView()
      );
    } else {
      run(toggleMark(blogSchema.marks.link, { href }));
    }
  });

  // 7. Single upload
  iconBtn(
    g6,
    icons.upload,
    "Upload an image or file (images appear inline; other files become links)",
    () => fileInput.click()
  );

  fileInput.addEventListener("change", async () => {
    const file = fileInput.files?.[0];
    fileInput.value = "";
    if (!file) return;
    const view = getView();
    if (!view) return;
    try {
      const result = await host.upload(file);
      insertUploaded(view, host, file, result);
    } catch (err: any) {
      host.onError(err?.message || "Upload failed");
    }
  });

  const refresh = () => {
    bar.querySelectorAll(".bre__btn").forEach((el) => {
      const b = el as HTMLButtonElement & { _isActive?: () => boolean };
      if (b._isActive) b.classList.toggle("is-active", !!b._isActive());
    });
    const view = getView();
    if (!view) return;
    const $from = view.state.selection.$from;
    const node = $from.parent;
    if (node.type === blogSchema.nodes.heading) {
      blockSelect.value = "h" + node.attrs.level;
    } else if (node.type === blogSchema.nodes.pre) {
      blockSelect.value = "pre";
    } else {
      blockSelect.value = "p";
    }
  };

  bar._refresh = refresh;
  return bar;
}

export { insertUploaded };
