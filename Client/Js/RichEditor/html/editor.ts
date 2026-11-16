import { EditorState, Plugin, TextSelection } from "prosemirror-state";
import { EditorView } from "prosemirror-view";
import { history, undo, redo } from "prosemirror-history";
import { keymap } from "prosemirror-keymap";
import { baseKeymap, toggleMark } from "prosemirror-commands";
import { splitListItem, liftListItem, sinkListItem } from "prosemirror-schema-list";
import {
  columnResizing,
  tableEditing,
  goToNextCell,
} from "prosemirror-tables";
import { inputRules, wrappingInputRule, textblockTypeInputRule } from "prosemirror-inputrules";
import { blogSchema } from "./schema";
import { docToHtml, htmlToDoc, sanitizePastedHtml } from "./serialize";
import type { EditorEngine, EngineHost } from "../core/types";
import { createToolbar, insertUploaded } from "./toolbar";

function buildInputRules() {
  const s = blogSchema;
  return inputRules({
    rules: [
      wrappingInputRule(/^\s*>\s$/, s.nodes.blockquote),
      wrappingInputRule(
        /^(\d+)\.\s$/,
        s.nodes.ordered_list,
        (match) => ({ order: +match[1] }),
        (match, node) => node.childCount + node.attrs.order === +match[1]
      ),
      wrappingInputRule(/^\s*([-+*])\s$/, s.nodes.bullet_list),
      textblockTypeInputRule(/^```$/, s.nodes.pre),
      textblockTypeInputRule(/^(#{1,6})\s$/, s.nodes.heading, (m) => ({
        level: m[1].length,
      })),
    ],
  });
}

export function createHtmlEngine(host: EngineHost): EditorEngine {
  let view: EditorView | null = null;

  const shell = document.createElement("div");
  shell.className = "bre__chrome";

  const toolbar = createToolbar(
    host.mount.getRootNode() as ShadowRoot,
    () => view,
    host
  );
  const body = document.createElement("div");
  body.className = "bre__body";
  const surface = document.createElement("div");
  surface.className = "bre__surface bre__prose";
  body.appendChild(surface);
  shell.appendChild(toolbar);
  shell.appendChild(body);
  host.mount.appendChild(shell);

  const pastePlugin = new Plugin({
    props: {
      handlePaste(view, event) {
        const html = event.clipboardData?.getData("text/html");
        if (!html) return false;
        const clean = sanitizePastedHtml(html);
        const doc = htmlToDoc(clean);
        const slice = doc.slice(0);
        const tr = view.state.tr.replaceSelection(slice);
        view.dispatch(tr.scrollIntoView());
        return true;
      },
      handleDrop(view, event) {
        const files = event.dataTransfer?.files;
        if (!files?.length) return false;
        event.preventDefault();
        const file = files[0];
        void (async () => {
          try {
            const result = await host.upload(file);
            const coords = view.posAtCoords({
              left: event.clientX,
              top: event.clientY,
            });
            if (coords) {
              view.dispatch(
                view.state.tr.setSelection(
                  TextSelection.create(view.state.doc, coords.pos)
                )
              );
            }
            insertUploaded(view, host, file, result);
          } catch (err: any) {
            host.onError(err?.message || "Upload failed");
          }
        })();
        return true;
      },
    },
  });

  const state = EditorState.create({
    doc: htmlToDoc(host.initialValue),
    plugins: [
      buildInputRules(),
      history(),
      keymap({
        "Mod-z": undo,
        "Mod-y": redo,
        "Mod-Shift-z": redo,
        "Mod-b": toggleMark(blogSchema.marks.strong),
        "Mod-i": toggleMark(blogSchema.marks.em),
        "Shift-Enter": (state, dispatch) => {
          if (dispatch)
            dispatch(
              state.tr
                .replaceSelectionWith(blogSchema.nodes.hard_break.create())
                .scrollIntoView()
            );
          return true;
        },
        Enter: splitListItem(blogSchema.nodes.list_item),
        "Mod-[": liftListItem(blogSchema.nodes.list_item),
        "Mod-]": sinkListItem(blogSchema.nodes.list_item),
        Tab: goToNextCell(1),
        "Shift-Tab": goToNextCell(-1),
      }),
      keymap(baseKeymap),
      columnResizing(),
      tableEditing(),
      pastePlugin,
    ],
  });

  view = new EditorView(surface, {
    state,
    editable: () => !host.disabled,
    attributes: { class: "bre__prose" },
    dispatchTransaction(tr) {
      if (!view) return;
      const next = view.state.apply(tr);
      view.updateState(next);
      toolbar._refresh?.();
      if (tr.docChanged) host.onChange(docToHtml(next.doc));
    },
  });
  toolbar._refresh?.();

  return {
    getValue: () => (view ? docToHtml(view.state.doc) : ""),
    setValue: (value: string) => {
      if (!view) return;
      const next = value || "";
      if (docToHtml(view.state.doc) === next) return;
      const doc = htmlToDoc(next);
      const tr = view.state.tr.replaceWith(0, view.state.doc.content.size, doc.content);
      view.dispatch(tr);
    },
    focus: () => view?.focus(),
    destroy: () => {
      view?.destroy();
      view = null;
      shell.remove();
    },
  };
}
