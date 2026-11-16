import * as React from "react";

export interface TokenSecretDialogProps {
  open: boolean;
  title: string;
  warning: string;
  token: string;
  copyLabel: string;
  closeLabel: string;
  onClose: () => void;
}

export default function TokenSecretDialog(
  props: TokenSecretDialogProps
): React.ReactElement | null {
  const { open, title, warning, token, copyLabel, closeLabel, onClose } = props;
  const [copied, setCopied] = React.useState(false);
  const titleId = React.useRef(
    `blog-token-secret-title-${Math.random().toString(36).slice(2, 9)}`
  ).current;
  const closeRef = React.useRef<HTMLButtonElement>(null);

  React.useEffect(() => {
    if (!open) {
      setCopied(false);
      return;
    }
    closeRef.current?.focus();
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        e.preventDefault();
        onClose();
      }
    };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, [open, onClose]);

  if (!open) return null;

  const copy = async () => {
    try {
      if (navigator.clipboard?.writeText) {
        await navigator.clipboard.writeText(token);
      } else {
        const ta = document.createElement("textarea");
        ta.value = token;
        ta.setAttribute("readonly", "");
        ta.style.position = "absolute";
        ta.style.left = "-9999px";
        document.body.appendChild(ta);
        ta.select();
        document.execCommand("copy");
        document.body.removeChild(ta);
      }
      setCopied(true);
    } catch {
      setCopied(false);
    }
  };

  return (
    <div className="blog-dialog is-open" role="presentation">
      <div className="blog-dialog__backdrop" onClick={onClose} />
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
            aria-label={closeLabel}
            onClick={onClose}
          >
            &times;
          </button>
        </header>
        <div className="blog-dialog__body">
          <p className="blog-dialog__message">{warning}</p>
          <textarea
            className="blog-admin__input"
            readOnly
            rows={3}
            value={token}
            onFocus={(e) => e.currentTarget.select()}
          />
        </div>
        <div className="blog-dialog__actions">
          <button
            type="button"
            className="blog-admin__btn blog-admin__btn--secondary"
            onClick={copy}
          >
            {copied ? "✓ " + copyLabel : copyLabel}
          </button>
          <button
            ref={closeRef}
            type="button"
            className="blog-admin__btn blog-admin__btn--primary"
            onClick={onClose}
          >
            {closeLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
