import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Plate, usePlateEditor } from "platejs/react";
import { EditorKit } from "@/components/editor/editor-kit";
import { Editor, EditorContainer } from "@/components/ui/editor";
import { htmlToPlateValue } from "./deserialize";
import { serializeNodesToHtml } from "./serialize";
import { MediaProvider } from "./MediaContext";
import { EditorErrorBoundary } from "./EditorErrorBoundary";
import { TooltipProvider } from "@/components/ui/tooltip";
import "../styles/plate.css";
import "katex/dist/katex.min.css";

export interface HtmlEditorMediaService {
  uploadImage: (
    blogId: number,
    postId: number,
    file: File,
    success: (data: { url: string; fileName: string }) => void,
    fail?: (error: string) => void
  ) => void;
  getFileUrl: (
    fileId: number,
    success: (data: { url: string; fileName: string }) => void,
    fail?: (error: string) => void
  ) => void;
  getFolders: (
    parentFolderId: number,
    success: (data: any) => void,
    fail?: (error: string) => void
  ) => void;
  getFolderDescendants: (
    parentId: string,
    success: (items: any[]) => void,
    fail?: (error: string) => void
  ) => void;
  getFiles: (
    parentId: number,
    mode: "image" | "file",
    success: (data: any) => void,
    fail?: (error: string) => void
  ) => void;
}

export interface HtmlEditorProps {
  module: { service: HtmlEditorMediaService };
  blogId: number;
  postId: number;
  value?: string;
  onChange?: (html: string) => void;
  textarea?: HTMLTextAreaElement | null;
}

function HtmlEditorInner({
  module,
  blogId,
  postId,
  value,
  onChange,
  textarea,
}: HtmlEditorProps) {
  const [error, setError] = useState<string | null>(null);
  const syncingRef = useRef(false);
  const onChangeRef = useRef(onChange);
  onChangeRef.current = onChange;

  const initialHtml = useMemo(() => {
    if (value != null) return value;
    if (textarea) return textarea.value || "";
    return "";
  }, []);

  const initialValue = useMemo(() => {
    try {
      return htmlToPlateValue(initialHtml);
    } catch (e) {
      console.error("htmlToPlateValue failed", e);
      return [{ type: "p", children: [{ text: "" }] }];
    }
  }, [initialHtml]);

  const editor = usePlateEditor({
    plugins: EditorKit,
    value: initialValue,
  });

  const emitHtml = useCallback(() => {
    if (syncingRef.current) return;
    syncingRef.current = true;
    try {
      const html = serializeNodesToHtml(editor.children);
      if (textarea) {
        textarea.value = html;
        textarea.dispatchEvent(new Event("change", { bubbles: true }));
        textarea.dispatchEvent(new Event("input", { bubbles: true }));
      }
      onChangeRef.current?.(html);
    } finally {
      syncingRef.current = false;
    }
  }, [editor, textarea]);

  useEffect(() => {
    if (!textarea) return;
    const form = textarea.closest("form");
    if (!form) return;
    const onSubmit = () => emitHtml();
    form.addEventListener("submit", onSubmit);
    return () => form.removeEventListener("submit", onSubmit);
  }, [textarea, emitHtml]);

  return (
    <div className="blog-he">
      <MediaProvider
        service={module.service}
        blogId={blogId}
        postId={postId}
        onError={setError}
      >
        <Plate
          editor={editor}
          onChange={() => {
            emitHtml();
          }}
        >
          <div className="blog-he__shell">
            {error && (
              <div
                className="flex items-center justify-between gap-2 border-b border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive"
                role="alert"
              >
                <span>{error}</span>
                <button
                  type="button"
                  className="text-lg leading-none"
                  onClick={() => setError(null)}
                >
                  ×
                </button>
              </div>
            )}

            <EditorContainer variant="demo" className="h-[580px]">
              <Editor
                variant="fullWidth"
                placeholder="Type your content here…"
              />
            </EditorContainer>
          </div>
        </Plate>
      </MediaProvider>
    </div>
  );
}

export function HtmlEditor(props: HtmlEditorProps) {
  return (
    <EditorErrorBoundary>
      <TooltipProvider delayDuration={300}>
        <HtmlEditorInner {...props} />
      </TooltipProvider>
    </EditorErrorBoundary>
  );
}
