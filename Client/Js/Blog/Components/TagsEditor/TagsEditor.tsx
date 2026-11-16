import * as React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import { IAppModule } from "../../Models/IAppModule";
import { ITagItem, ITagLocale, ITagList } from "../../Models/ITag";

export interface TagsEditorProps {
  module: IAppModule;
  defaultLocale: string;
  locales: ITagLocale[];
}

const PAGE_SIZE = 20;

function TagCell(props: {
  termId: number;
  locale: string;
  value: string;
  busy: boolean;
  onCommit: (
    termId: number,
    locale: string,
    value: string,
    previous: string,
    reset: (v: string) => void
  ) => void;
}): React.ReactElement {
  const { termId, locale, value, busy, onCommit } = props;
  const [draft, setDraft] = useState(value);
  const originalRef = useRef(value);

  useEffect(() => {
    setDraft(value);
    originalRef.current = value;
  }, [value, termId, locale]);

  return (
    <input
      type="text"
      className="blog-tags__input"
      value={draft}
      disabled={busy}
      onChange={(e) => setDraft(e.target.value)}
      onBlur={() => {
        const next = draft;
        const prev = originalRef.current;
        if (next === prev) return;
        onCommit(termId, locale, next, prev, (v) => {
          setDraft(v);
          originalRef.current = v;
        });
      }}
      onKeyDown={(e) => {
        if (e.key === "Enter") {
          (e.target as HTMLInputElement).blur();
        } else if (e.key === "Escape") {
          setDraft(originalRef.current);
          (e.target as HTMLInputElement).blur();
        }
      }}
    />
  );
}

export default function TagsEditor(
  props: TagsEditorProps
): React.ReactElement {
  const { module } = props;
  const [items, setItems] = useState<ITagItem[]>([]);
  const [locales, setLocales] = useState<ITagLocale[]>(props.locales || []);
  const [defaultLocale, setDefaultLocale] = useState(
    props.defaultLocale || ""
  );
  const [page, setPage] = useState(0);
  const [total, setTotal] = useState(0);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const debounceRef = useRef<number | null>(null);
  const requestId = useRef(0);

  const load = useCallback(() => {
    const id = ++requestId.current;
    setLoading(true);
    setError(null);
    module.service.getTags(
      { page, pageSize: PAGE_SIZE, search },
      (data: ITagList) => {
        if (id !== requestId.current) return;
        setItems(data.Items || []);
        setTotal(data.Total || 0);
        if (data.DefaultLocale) setDefaultLocale(data.DefaultLocale);
        if (data.Locales && data.Locales.length) setLocales(data.Locales);
        setLoading(false);
      },
      (err: string) => {
        if (id !== requestId.current) return;
        setError(err || "Failed to load tags.");
        setItems([]);
        setTotal(0);
        setLoading(false);
      }
    );
  }, [module.service, page, search]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    if (debounceRef.current != null) {
      window.clearTimeout(debounceRef.current);
    }
    debounceRef.current = window.setTimeout(() => {
      setPage(0);
      setSearch(searchInput.trim());
    }, 300);
    return () => {
      if (debounceRef.current != null) {
        window.clearTimeout(debounceRef.current);
      }
    };
  }, [searchInput]);

  const handleCommit = (
    termId: number,
    locale: string,
    value: string,
    previous: string,
    reset: (v: string) => void
  ) => {
    setBusy(true);
    module.service.updateTagCell(
      { TermId: termId, Locale: locale, Value: value },
      (saved) => {
        setBusy(false);
        reset(saved);
        setItems((prev) =>
          prev.map((row) => {
            if (row.TermId !== termId) return row;
            if (!locale) {
              return { ...row, Name: saved };
            }
            return {
              ...row,
              Localizations: { ...row.Localizations, [locale]: saved },
            };
          })
        );
      },
      (err) => {
        setBusy(false);
        window.alert(err || "Could not save this value.");
        reset(previous);
      }
    );
  };

  const handleAdd = () => {
    const name = window.prompt("New tag name (default language):");
    if (name == null) return;
    const trimmed = name.trim();
    if (!trimmed) {
      window.alert("Tag name is required.");
      return;
    }
    setBusy(true);
    module.service.addTag(
      trimmed,
      () => {
        setSearchInput("");
        setSearch("");
        setPage(0);
        const id = ++requestId.current;
        module.service.getTags(
          { page: 0, pageSize: PAGE_SIZE, search: "" },
          (data: ITagList) => {
            if (id !== requestId.current) return;
            setItems(data.Items || []);
            setTotal(data.Total || 0);
            if (data.DefaultLocale) setDefaultLocale(data.DefaultLocale);
            if (data.Locales && data.Locales.length) setLocales(data.Locales);
            setBusy(false);
            setLoading(false);
          },
          (err: string) => {
            if (id !== requestId.current) return;
            setBusy(false);
            window.alert(err || "Tag was added but the list could not refresh.");
            load();
          }
        );
      },
      (err) => {
        setBusy(false);
        window.alert(err || "Could not add tag.");
      }
    );
  };

  const handleDelete = (tag: ITagItem) => {
    if (!window.confirm(`Delete tag “${tag.Name}”?`)) return;
    setBusy(true);
    module.service.deleteTag(
      tag.TermId,
      () => {
        setBusy(false);
        const remaining = total - 1;
        const maxPage = Math.max(0, Math.ceil(remaining / PAGE_SIZE) - 1);
        if (page > maxPage) {
          setPage(maxPage);
        } else {
          load();
        }
      },
      (err) => {
        setBusy(false);
        window.alert(err || "Could not delete tag.");
      }
    );
  };

  const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE) || 1);
  const current = Math.min(page + 1, totalPages);

  return (
    <div className="blog-tags">
      <header className="blog-tags__header">
        <div>
          <h2 className="blog-tags__title">Tags</h2>
          <p className="blog-tags__subtitle">
            Edit default names and translations. Changes save when you leave a
            cell.
          </p>
        </div>
      </header>

      <div className="blog-tags__toolbar">
        <button
          type="button"
          className="blog-admin__btn blog-admin__btn--primary"
          disabled={busy}
          onClick={handleAdd}
        >
          <i className="fa fa-plus" aria-hidden="true" /> Add
        </button>
        <input
          type="search"
          className="blog-tags__search"
          placeholder="Search tags…"
          value={searchInput}
          disabled={busy && loading}
          onChange={(e) => setSearchInput(e.target.value)}
        />
        {(busy || loading) && (
          <span className="blog-tags__status">
            {busy ? "Saving…" : "Loading…"}
          </span>
        )}
      </div>

      {error && (
        <div className="blog-admin__message blog-admin__message--info">
          {error}
        </div>
      )}

      <div className="blog-tags__table-wrap">
        <table className="blog-tags__table">
          <thead>
            <tr>
              <th className="blog-tags__col-actions" aria-label="Actions" />
              <th>
                Default
                {defaultLocale ? ` (${defaultLocale})` : ""}
              </th>
              {locales.map((l) => (
                <th key={l.Code}>{l.Name || l.Code}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {items.length === 0 && !loading ? (
              <tr>
                <td
                  colSpan={2 + locales.length}
                  className="blog-tags__empty"
                >
                  No tags found.
                </td>
              </tr>
            ) : (
              items.map((tag) => (
                <tr key={tag.TermId}>
                  <td className="blog-tags__col-actions">
                    <button
                      type="button"
                      className="blog-tags__icon-btn blog-tags__icon-btn--danger"
                      title="Delete"
                      disabled={busy}
                      onClick={() => handleDelete(tag)}
                    >
                      <i className="fa fa-times" aria-hidden="true" />
                    </button>
                  </td>
                  <td>
                    <TagCell
                      termId={tag.TermId}
                      locale=""
                      value={tag.Name}
                      busy={busy}
                      onCommit={handleCommit}
                    />
                  </td>
                  {locales.map((l) => (
                    <td key={l.Code}>
                      <TagCell
                        termId={tag.TermId}
                        locale={l.Code}
                        value={
                          (tag.Localizations && tag.Localizations[l.Code]) ||
                          ""
                        }
                        busy={busy}
                        onCommit={handleCommit}
                      />
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {total > 0 && (
        <div className="blog-tags__pager">
          <span className="blog-tags__pager-info">
            Page {current} of {totalPages} ({total} tag
            {total === 1 ? "" : "s"})
          </span>
          <div className="blog-tags__pager-btns">
            <button
              type="button"
              className="blog-admin__btn blog-admin__btn--secondary"
              disabled={page <= 0 || busy}
              onClick={() => setPage((p) => Math.max(0, p - 1))}
            >
              Previous
            </button>
            <button
              type="button"
              className="blog-admin__btn blog-admin__btn--secondary"
              disabled={page + 1 >= totalPages || busy}
              onClick={() => setPage((p) => p + 1)}
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
