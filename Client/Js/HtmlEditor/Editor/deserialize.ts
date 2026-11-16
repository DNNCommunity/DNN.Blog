import { createSlateEditor, type Descendant, type Value } from "platejs";
import { EditorKit } from "@/components/editor/editor-kit";

const emptyDoc: Value = [{ type: "p", children: [{ text: "" }] }];

/**
 * Convert a stored post HTML fragment into Plate nodes using plugin HTML parsers.
 */
export function htmlToPlateValue(html: string): Value {
  if (!html || !html.trim()) return emptyDoc;

  const editor = createSlateEditor({ plugins: EditorKit });
  const element = new DOMParser().parseFromString(html, "text/html").body;
  const nodes = editor.api.html.deserialize({
    element,
  }) as Descendant[];

  if (!nodes || nodes.length === 0) return emptyDoc;
  return nodes as Value;
}
