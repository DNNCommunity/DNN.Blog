import * as React from "react";

export interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  confirmLabel: string;
  cancelLabel: string;
  busy?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

/**
 * Module-styled destructive confirmation using the existing .blog-dialog chrome.
 */
export default function ConfirmDialog(
  props: ConfirmDialogProps
): React.ReactElement | null {
  const {
    open,
    title,
    message,
    confirmLabel,
    cancelLabel,
    busy,
    onConfirm,
    onCancel,
  } = props;
  const titleId = React.useRef(
    `blog-confirm-title-${Math.random().toString(36).slice(2, 9)}`
  ).current;
  const confirmRef = React.useRef<HTMLButtonElement>(null);

  React.useEffect(() => {
    if (!open) return;
    confirmRef.current?.focus();
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        e.preventDefault();
        if (!busy) onCancel();
      }
    };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, [open, busy, onCancel]);

  if (!open) return null;

  return (
    <div className="blog-dialog is-open" role="presentation">
      <div
        className="blog-dialog__backdrop"
        onClick={() => {
          if (!busy) onCancel();
        }}
      />
      <div
        className="blog-dialog__panel blog-admin"
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
          <p className="blog-dialog__message">{message}</p>
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
            ref={confirmRef}
            type="button"
            className="blog-admin__btn blog-admin__btn--danger"
            disabled={busy}
            onClick={onConfirm}
          >
            {confirmLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
