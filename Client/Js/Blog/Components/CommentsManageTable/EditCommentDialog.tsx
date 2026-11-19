import * as React from "react";

export interface EditCommentDialogProps {
  open: boolean;
  title: string;
  commentLabel: string;
  saveLabel: string;
  cancelLabel: string;
  initialHtml: string;
  busy?: boolean;
  onSave: (comment: string) => void;
  onCancel: () => void;
}

function htmlToPlainText(html: string): string {
  if (!html) return "";
  const textarea = document.createElement("textarea");
  textarea.innerHTML = html;
  return textarea.value
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<\/p>/gi, "\n")
    .replace(/<p[^>]*>/gi, "");
}

export default function EditCommentDialog(
  props: EditCommentDialogProps
): React.ReactElement | null {
  const {
    open,
    title,
    commentLabel,
    saveLabel,
    cancelLabel,
    initialHtml,
    busy,
    onSave,
    onCancel,
  } = props;
  const [text, setText] = React.useState("");
  const titleId = React.useRef(
    `blog-edit-comment-title-${Math.random().toString(36).slice(2, 9)}`
  ).current;
  const inputId = titleId + "-input";
  const textareaRef = React.useRef<HTMLTextAreaElement>(null);

  React.useEffect(() => {
    if (!open) {
      setText("");
      return;
    }
    setText(htmlToPlainText(initialHtml));
    window.setTimeout(() => textareaRef.current?.focus(), 0);
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        e.preventDefault();
        if (!busy) onCancel();
      }
    };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, [open, initialHtml, busy, onCancel]);

  if (!open) return null;

  const trimmed = text.trim();
  const canSubmit = trimmed.length > 0 && !busy;

  return (
    <div className="blog-dialog is-open" role="presentation">
      <div
        className="blog-dialog__backdrop"
        onClick={() => {
          if (!busy) onCancel();
        }}
      />
      <div
        className="blog-dialog__panel blog-dialog__panel--wide blog-admin"
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
      >
        <header className="blog-dialog__header">
          <h2 className="blog-dialog__title" id={titleId}>
            {title}
          </h2>
          <button
            type="button"
            className="blog-dialog__close"
            aria-label={cancelLabel}
            disabled={busy}
            onClick={onCancel}
          >
            &times;
          </button>
        </header>
        <div className="blog-dialog__body">
          <label className="blog-dialog__label" htmlFor={inputId}>
            {commentLabel}
          </label>
          <textarea
            ref={textareaRef}
            id={inputId}
            className="blog-admin__input blog-admin__textarea"
            rows={8}
            value={text}
            disabled={busy}
            onChange={(e) => setText(e.target.value)}
          />
        </div>
        <div className="blog-dialog__actions">
          <button
            type="button"
            className="blog-admin__btn blog-admin__btn--secondary"
            disabled={busy}
            onClick={onCancel}
          >
            {cancelLabel}
          </button>
          <button
            type="button"
            className="blog-admin__btn blog-admin__btn--primary"
            disabled={!canSubmit}
            onClick={() => onSave(trimmed)}
          >
            {saveLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
