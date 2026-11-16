import React, { useEffect, useRef } from "react";
import {
  BlogRichEditorElement,
  defineBlogRichEditor,
} from "../core/BlogRichEditor";
import type { EditorLocaleOption, EditorMode, UploadFn } from "../core/types";

export interface BlogRichEditorProps {
  mode?: EditorMode;
  value?: string;
  onChange?: (value: string) => void;
  blogId: number;
  postId: number;
  moduleId: number;
  uploadUrl?: string;
  uploadFn?: UploadFn;
  textarea?: HTMLTextAreaElement | null;
  disabled?: boolean;
  placeholder?: string;
  className?: string;
  /** Locales for the toolbar-end switcher (default first). Empty/omit to hide. */
  locales?: EditorLocaleOption[];
  locale?: string;
  onLocaleChange?: (code: string) => void;
  localeLabel?: string;
}

const HOST_STYLE_ID = "blog-rich-editor-host-style";

function ensureHostStyles(): void {
  if (typeof document === "undefined") return;
  if (document.getElementById(HOST_STYLE_ID)) return;
  const style = document.createElement("style");
  style.id = HOST_STYLE_ID;
  style.textContent = `
    textarea[data-blog-html-editor] {
      position: absolute !important;
      width: 1px !important;
      height: 1px !important;
      padding: 0 !important;
      margin: -1px !important;
      overflow: hidden !important;
      clip: rect(0, 0, 0, 0) !important;
      border: 0 !important;
    }
    blog-rich-editor {
      display: block;
      width: 100%;
    }
  `;
  document.head.appendChild(style);
}

defineBlogRichEditor();

export function BlogRichEditor(props: BlogRichEditorProps): React.ReactElement {
  const ref = useRef<BlogRichEditorElement | null>(null);
  const onChangeRef = useRef(props.onChange);
  onChangeRef.current = props.onChange;
  const onLocaleChangeRef = useRef(props.onLocaleChange);
  onLocaleChangeRef.current = props.onLocaleChange;
  const lastEmittedRef = useRef<string | null>(null);
  const mode: EditorMode = props.mode === "markdown" ? "markdown" : "html";

  useEffect(() => {
    ensureHostStyles();
  }, []);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    el.mode = mode;
    el.blogId = props.blogId;
    el.postId = props.postId;
    el.moduleId = props.moduleId;
    if (props.uploadUrl) el.uploadUrl = props.uploadUrl;
    el.disabled = !!props.disabled;
    if (props.placeholder) el.setAttribute("placeholder", props.placeholder);
    if (props.uploadFn) el.setUploadFn(props.uploadFn);
    el.bindTextarea(props.textarea ?? null);

    const initial =
      props.value ??
      props.textarea?.value ??
      "";
    if (lastEmittedRef.current === null) {
      lastEmittedRef.current = initial;
      if (el.getValue() !== initial) {
        el.setValue(initial);
      }
    }

    const handler = (e: Event) => {
      const detail = (e as CustomEvent).detail as { value: string };
      const next = detail?.value ?? el.getValue();
      lastEmittedRef.current = next;
      onChangeRef.current?.(next);
    };
    el.addEventListener("change", handler);
    return () => el.removeEventListener("change", handler);
  }, [
    mode,
    props.blogId,
    props.postId,
    props.moduleId,
    props.uploadUrl,
    props.uploadFn,
    props.textarea,
    props.disabled,
    props.placeholder,
  ]);

  // Apply external value changes only (not echoes of our own onChange).
  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    const next = props.value ?? "";
    if (next === lastEmittedRef.current) return;
    if (el.getValue() === next) {
      lastEmittedRef.current = next;
      return;
    }
    lastEmittedRef.current = next;
    el.setValue(next);
  }, [props.value, mode]);

  // Locale switcher lives in the editor toolbar end slot.
  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    const locales = props.locales || [];
    el.setLocaleSwitcher(
      locales,
      props.locale || locales[0]?.code || "",
      (code) => onLocaleChangeRef.current?.(code),
      props.localeLabel
    );
  }, [props.locales, props.locale, props.localeLabel, mode]);

  return (
    <blog-rich-editor
      ref={ref as any}
      className={props.className}
      mode={mode}
      blog-id={String(props.blogId)}
      post-id={String(props.postId)}
      module-id={String(props.moduleId)}
    />
  );
}

export default BlogRichEditor;
