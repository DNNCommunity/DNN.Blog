import * as React from "react";
import { useEffect, useRef, useState } from "react";

export interface CommentClampProps {
  html: string;
  showMoreLabel: string;
  showLessLabel: string;
}

function stripHtml(html: string): string {
  const el = document.createElement("div");
  el.innerHTML = html || "";
  return (el.textContent || el.innerText || "").trim();
}

export default function CommentClamp(
  props: CommentClampProps
): React.ReactElement {
  const { html, showMoreLabel, showLessLabel } = props;
  const text = stripHtml(html);
  const [expanded, setExpanded] = useState(false);
  const [needsClamp, setNeedsClamp] = useState(false);
  const measureRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setExpanded(false);
    const el = measureRef.current;
    if (!el) return;
    setNeedsClamp(el.scrollHeight > el.clientHeight + 1);
  }, [text]);

  return (
    <div className="blog-manage__comment-cell">
      <div
        ref={measureRef}
        className={
          "blog-manage__comment-text" +
          (expanded ? " blog-manage__comment-text--expanded" : "")
        }
      >
        {text}
      </div>
      {needsClamp && (
        <button
          type="button"
          className="blog-manage__comment-toggle"
          onClick={() => setExpanded((v) => !v)}
        >
          {expanded ? showLessLabel : showMoreLabel}
        </button>
      )}
    </div>
  );
}
