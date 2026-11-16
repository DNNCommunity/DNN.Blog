import * as React from "react";
import { IAppModule } from "../Models/IAppModule";
import "./TagsAutocomplete.scss";

export interface TagsAutocompleteProps {
  module: IAppModule;
  tags: string[];
  onChange: (tags: string[]) => void;
  placeholder: string;
}

function parseInitial(tags: string[]): string[] {
  return tags
    .map((t) => t.trim())
    .filter((t, i, arr) => t.length > 0 && arr.indexOf(t) === i);
}

/** Normalize user/API input: trim, drop a leading #, strip commas. */
function normalizeTagName(raw: string): string {
  let name = raw.trim().replace(/,/g, "");
  if (name.startsWith("#")) name = name.slice(1).trim();
  return name;
}

export default function TagsAutocomplete(
  props: TagsAutocompleteProps
): React.ReactElement {
  const { module, tags, onChange, placeholder } = props;
  const [input, setInput] = React.useState("");
  const [suggestions, setSuggestions] = React.useState<string[]>([]);
  const [open, setOpen] = React.useState(false);
  const [highlight, setHighlight] = React.useState(0);
  const debounceRef = React.useRef<number | null>(null);
  const wrapRef = React.useRef<HTMLDivElement>(null);

  const commit = React.useCallback(
    (raw: string) => {
      const name = normalizeTagName(raw);
      if (!name) return;
      const lower = name.toLowerCase();
      if (tags.some((t) => t.toLowerCase() === lower)) {
        setInput("");
        setSuggestions([]);
        setOpen(false);
        return;
      }
      onChange([...tags, name]);
      setInput("");
      setSuggestions([]);
      setOpen(false);
    },
    [tags, onChange]
  );

  const removeAt = (index: number) => {
    onChange(tags.filter((_, i) => i !== index));
  };

  React.useEffect(() => {
    if (debounceRef.current != null) window.clearTimeout(debounceRef.current);
    const q = normalizeTagName(input);
    if (q.length < 2) {
      setSuggestions([]);
      setOpen(false);
      return;
    }
    debounceRef.current = window.setTimeout(() => {
      module.service.searchTags(
        q,
        (names) => {
          const existing = new Set(tags.map((t) => t.toLowerCase()));
          const filtered = names.filter(
            (n) => n && !existing.has(n.toLowerCase())
          );
          setSuggestions(filtered);
          setOpen(filtered.length > 0);
          setHighlight(0);
        },
        () => {
          setSuggestions([]);
          setOpen(false);
        }
      );
    }, 150);
    return () => {
      if (debounceRef.current != null) window.clearTimeout(debounceRef.current);
    };
  }, [input, module.service, tags]);

  React.useEffect(() => {
    const onDoc = (e: MouseEvent) => {
      if (!wrapRef.current?.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener("mousedown", onDoc);
    return () => document.removeEventListener("mousedown", onDoc);
  }, []);

  const onKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" || e.key === ",") {
      e.preventDefault();
      if (open && suggestions[highlight]) {
        commit(suggestions[highlight]);
      } else {
        commit(input);
      }
      return;
    }
    if (e.key === "Backspace" && !input && tags.length > 0) {
      removeAt(tags.length - 1);
      return;
    }
    if (e.key === "ArrowDown" && open) {
      e.preventDefault();
      setHighlight((h) => Math.min(h + 1, suggestions.length - 1));
      return;
    }
    if (e.key === "ArrowUp" && open) {
      e.preventDefault();
      setHighlight((h) => Math.max(h - 1, 0));
      return;
    }
    if (e.key === "Escape") {
      setOpen(false);
    }
  };

  const onBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    // Suggestion buttons use mousedown+preventDefault so blur does not fire.
    // Skip when focus moves to a chip remove control inside the widget.
    const related = e.relatedTarget as Node | null;
    if (related && wrapRef.current?.contains(related)) return;
    commit(e.currentTarget.value);
  };

  return (
    <div className="blog-tags-ac" ref={wrapRef}>
      <input type="hidden" name="Tags" value={tags.join(", ")} />
      <div className="blog-tags-ac__box">
        {parseInitial(tags).map((tag, i) => (
          <span className="blog-tags-ac__chip" key={`${tag}-${i}`}>
            <span className="blog-tags-ac__chip-text">#{tag}</span>
            <button
              type="button"
              className="blog-tags-ac__chip-remove"
              aria-label={`Remove ${tag}`}
              onClick={() => removeAt(i)}
            >
              ×
            </button>
          </span>
        ))}
        <input
          type="text"
          className="blog-tags-ac__input"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={onKeyDown}
          onBlur={onBlur}
          onFocus={() => {
            if (suggestions.length > 0) setOpen(true);
          }}
          placeholder={tags.length === 0 ? placeholder : ""}
          autoComplete="off"
        />
      </div>
      {open && suggestions.length > 0 && (
        <ul className="blog-tags-ac__suggestions" role="listbox">
          {suggestions.map((s, i) => (
            <li key={s}>
              <button
                type="button"
                className={`blog-tags-ac__suggestion${
                  i === highlight ? " is-active" : ""
                }`}
                role="option"
                aria-selected={i === highlight}
                onMouseDown={(e) => {
                  e.preventDefault();
                  commit(s);
                }}
              >
                {s}
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
