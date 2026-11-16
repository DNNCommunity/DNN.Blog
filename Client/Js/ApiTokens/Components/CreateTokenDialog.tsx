import * as React from "react";

export interface CreateTokenDialogProps {
  open: boolean;
  title: string;
  nameLabel: string;
  createLabel: string;
  cancelLabel: string;
  busy?: boolean;
  onCreate: (name: string) => void;
  onCancel: () => void;
}

export default function CreateTokenDialog(
  props: CreateTokenDialogProps
): React.ReactElement | null {
  const {
    open,
    title,
    nameLabel,
    createLabel,
    cancelLabel,
    busy,
    onCreate,
    onCancel,
  } = props;
  const [name, setName] = React.useState("");
  const titleId = React.useRef(
    `blog-create-token-title-${Math.random().toString(36).slice(2, 9)}`
  ).current;
  const inputRef = React.useRef<HTMLInputElement>(null);

  React.useEffect(() => {
    if (!open) {
      setName("");
      return;
    }
    inputRef.current?.focus();
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

  const trimmed = name.trim();
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
          <label className="blog-admin__label" htmlFor={titleId + "-input"}>
            {nameLabel}
          </label>
          <input
            ref={inputRef}
            id={titleId + "-input"}
            type="text"
            className="blog-admin__input"
            value={name}
            disabled={busy}
            maxLength={100}
            onChange={(e) => setName(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter" && canSubmit) {
                e.preventDefault();
                onCreate(trimmed);
              }
            }}
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
            onClick={() => onCreate(trimmed)}
          >
            {createLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
