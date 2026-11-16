import * as React from "react";
import { useCallback, useEffect, useMemo, useState } from "react";
import { IAppModule } from "../../Models/IAppModule";
import {
  ICategoryLocale,
  ICategoryNode,
  ICategoryTree,
} from "../../Models/ICategory";
import CategoryTree from "./CategoryTree";

export interface CategoriesEditorProps {
  module: IAppModule;
  vocabularyId: number;
  defaultLocale: string;
  locales: ICategoryLocale[];
  multilingual: boolean;
}

function findNode(
  nodes: ICategoryNode[],
  termId: number
): ICategoryNode | null {
  for (const n of nodes) {
    if (n.TermId === termId) return n;
    const child = findNode(n.Children || [], termId);
    if (child) return child;
  }
  return null;
}

function collectExpandableIds(nodes: ICategoryNode[]): number[] {
  const ids: number[] = [];
  const walk = (list: ICategoryNode[]) => {
    for (const n of list) {
      if (n.Children && n.Children.length) {
        ids.push(n.TermId);
        walk(n.Children);
      }
    }
  };
  walk(nodes);
  return ids;
}

function findParentAndIndex(
  nodes: ICategoryNode[],
  termId: number,
  parentId: number = -1
): { parentId: number; index: number; siblings: ICategoryNode[] } | null {
  for (let i = 0; i < nodes.length; i++) {
    if (nodes[i].TermId === termId) {
      return { parentId, index: i, siblings: nodes };
    }
    const found = findParentAndIndex(
      nodes[i].Children || [],
      termId,
      nodes[i].TermId
    );
    if (found) return found;
  }
  return null;
}

export default function CategoriesEditor(
  props: CategoriesEditorProps
): React.ReactElement {
  const { module, multilingual } = props;
  const [items, setItems] = useState<ICategoryNode[]>([]);
  const [locales, setLocales] = useState<ICategoryLocale[]>(props.locales || []);
  const [defaultLocale, setDefaultLocale] = useState(props.defaultLocale);
  const [isMl, setIsMl] = useState(multilingual);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [expandedIds, setExpandedIds] = useState<Set<number>>(new Set());
  const [loading, setLoading] = useState(false);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showBulk, setShowBulk] = useState(false);
  const [bulkText, setBulkText] = useState("");
  const [locName, setLocName] = useState("");
  const [locMap, setLocMap] = useState<Record<string, string>>({});

  const selected = useMemo(
    () => (selectedId != null ? findNode(items, selectedId) : null),
    [items, selectedId]
  );

  const applyTree = useCallback((data: ICategoryTree) => {
    setItems(data.Items || []);
    setLocales(data.Locales || []);
    setDefaultLocale(data.DefaultLocale || "");
    setIsMl(!!data.IsMultilingual);
    setExpandedIds(new Set(collectExpandableIds(data.Items || [])));
  }, []);

  const load = useCallback(() => {
    setLoading(true);
    setError(null);
    module.service.getCategoryTree(
      (data) => {
        applyTree(data);
        setLoading(false);
      },
      (err) => {
        setError(err || "Failed to load categories.");
        setLoading(false);
      }
    );
  }, [module.service, applyTree]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    if (!selected) {
      setLocName("");
      setLocMap({});
      return;
    }
    setLocName(selected.Name);
    const next: Record<string, string> = {};
    for (const l of locales) {
      next[l.Code] = (selected.Localizations && selected.Localizations[l.Code]) || "";
    }
    setLocMap(next);
  }, [selected, locales]);

  const withBusy = (fn: (done: (data: ICategoryTree) => void, fail: (e: string) => void) => void) => {
    setBusy(true);
    setError(null);
    fn(
      (data) => {
        applyTree(data);
        setBusy(false);
      },
      (err) => {
        setError(err || "Request failed.");
        setBusy(false);
      }
    );
  };

  const onSelect = (node: ICategoryNode) => setSelectedId(node.TermId);

  const onToggle = (termId: number) => {
    setExpandedIds((prev) => {
      const next = new Set(prev);
      if (next.has(termId)) next.delete(termId);
      else next.add(termId);
      return next;
    });
  };

  const onRename = (termId: number, name: string) => {
    withBusy((done, fail) =>
      module.service.updateCategory({ TermId: termId, Name: name }, done, fail)
    );
  };

  const onDelete = () => {
    if (selectedId == null || busy) return;
    if (
      !window.confirm(
        "Delete this category and all of its subcategories? This cannot be undone."
      )
    ) {
      return;
    }
    const id = selectedId;
    withBusy((done, fail) =>
      module.service.deleteCategory(
        id,
        (data) => {
          setSelectedId(null);
          done(data);
        },
        fail
      )
    );
  };

  const onAddBulk = () => {
    if (!bulkText.trim() || busy) return;
    const parentId = selectedId != null ? selectedId : -1;
    withBusy((done, fail) =>
      module.service.addCategories(
        bulkText,
        parentId,
        (data) => {
          setBulkText("");
          setShowBulk(false);
          done(data);
        },
        fail
      )
    );
  };

  const onSaveLocalize = () => {
    if (!selected || busy) return;
    withBusy((done, fail) =>
      module.service.localizeCategory(
        {
          TermId: selected.TermId,
          Name: locName.trim() || selected.Name,
          Localizations: locMap,
        },
        done,
        fail
      )
    );
  };

  const onDrop = (
    sourceId: number,
    targetId: number,
    position: "before" | "after" | "inside"
  ) => {
    if (busy || sourceId === targetId) return;
    const sourceInfo = findParentAndIndex(items, sourceId);
    const targetInfo = findParentAndIndex(items, targetId);
    if (!sourceInfo || !targetInfo) return;

    let parentTermId: number;
    let viewOrder: number;

    if (position === "inside") {
      parentTermId = targetId;
      const targetNode = findNode(items, targetId);
      viewOrder = (targetNode?.Children?.length || 0) + 1;
    } else {
      parentTermId = targetInfo.parentId;
      viewOrder =
        position === "before" ? targetInfo.index + 1 : targetInfo.index + 2;
      // When moving within same parent and source is before target, adjust
      if (
        sourceInfo.parentId === parentTermId &&
        sourceInfo.index < targetInfo.index &&
        position === "after"
      ) {
        viewOrder = targetInfo.index + 1;
      }
    }

    withBusy((done, fail) =>
      module.service.updateCategory(
        { TermId: sourceId, ParentTermId: parentTermId, ViewOrder: viewOrder },
        done,
        fail
      )
    );
  };

  return (
    <div className="blog-cat-editor">
      <header className="blog-cat-editor__header">
        <div>
          <h2 className="blog-cat-editor__title">Category editor</h2>
          <p className="blog-cat-editor__subtitle">
            Changes save immediately. Double-click a category to rename; drag to
            reorder.
          </p>
        </div>
      </header>

      <div className="blog-cat-editor__toolbar">
        <button
          type="button"
          className="blog-admin__btn blog-admin__btn--secondary"
          disabled={busy}
          onClick={() => setShowBulk((v) => !v)}
        >
          <i className="fa fa-plus" aria-hidden="true" /> Add
        </button>
        <button
          type="button"
          className="blog-admin__btn blog-admin__btn--secondary"
          disabled={busy || selectedId == null}
          onClick={onDelete}
        >
          <i className="fa fa-trash" aria-hidden="true" /> Delete
        </button>
        <button
          type="button"
          className="blog-admin__btn blog-admin__btn--secondary"
          disabled={busy || loading}
          onClick={load}
        >
          <i className="fa fa-refresh" aria-hidden="true" /> Refresh
        </button>
        {busy && <span className="blog-cat-editor__status">Saving…</span>}
        {loading && !busy && (
          <span className="blog-cat-editor__status">Loading…</span>
        )}
      </div>

      {error && (
        <div className="blog-admin__message blog-admin__message--info blog-cat-editor__error">
          {error}
        </div>
      )}

      {showBulk && (
        <div className="blog-cat-bulk">
          <p className="blog-cat-bulk__help">
            One category per line. Use leading dashes for subcategories (e.g.{" "}
            <code>Animals</code>, <code>- Mammals</code>, <code>-- Dogs</code>).
            {selected
              ? ` New top-level lines are added under “${selected.Name}”.`
              : " New top-level lines are added at the root."}
          </p>
          <textarea
            className="blog-cat-bulk__input"
            rows={6}
            value={bulkText}
            disabled={busy}
            onChange={(e) => setBulkText(e.target.value)}
            placeholder={"Sports\n- Football\n- Basketball\n-- NBA"}
          />
          <div className="blog-cat-bulk__actions">
            <button
              type="button"
              className="blog-admin__btn blog-admin__btn--primary"
              disabled={busy || !bulkText.trim()}
              onClick={onAddBulk}
            >
              Add categories
            </button>
            <button
              type="button"
              className="blog-admin__btn blog-admin__btn--secondary"
              disabled={busy}
              onClick={() => {
                setShowBulk(false);
                setBulkText("");
              }}
            >
              Cancel
            </button>
          </div>
        </div>
      )}

      <div
        className={
          "blog-cat-editor__body" +
          (isMl ? " blog-cat-editor__body--split" : "")
        }
      >
        <div className="blog-cat-editor__tree">
          <CategoryTree
            nodes={items}
            selectedId={selectedId}
            expandedIds={expandedIds}
            busy={busy}
            onSelect={onSelect}
            onToggle={onToggle}
            onRename={onRename}
            onDrop={onDrop}
          />
        </div>

        {isMl && (
          <aside className="blog-cat-editor__localize">
            <h3 className="blog-cat-editor__aside-title">
              <i className="fa fa-language" aria-hidden="true" /> Localization
            </h3>
            {!selected ? (
              <p className="blog-cat-editor__aside-empty">
                Select a category to edit translations.
              </p>
            ) : (
              <div className="blog-cat-localize">
                <label className="blog-cat-localize__field">
                  <span className="blog-cat-localize__label">
                    Default ({defaultLocale || "default"})
                  </span>
                  <input
                    type="text"
                    className="blog-admin__input"
                    value={locName}
                    disabled={busy}
                    onChange={(e) => setLocName(e.target.value)}
                  />
                </label>
                {locales.map((l) => (
                  <label key={l.Code} className="blog-cat-localize__field">
                    <span className="blog-cat-localize__label">
                      {l.Name || l.Code}
                    </span>
                    <input
                      type="text"
                      className="blog-admin__input"
                      value={locMap[l.Code] || ""}
                      disabled={busy}
                      onChange={(e) =>
                        setLocMap((prev) => ({
                          ...prev,
                          [l.Code]: e.target.value,
                        }))
                      }
                    />
                  </label>
                ))}
                <button
                  type="button"
                  className="blog-admin__btn blog-admin__btn--primary"
                  disabled={busy}
                  onClick={onSaveLocalize}
                >
                  Save localization
                </button>
              </div>
            )}
          </aside>
        )}
      </div>
    </div>
  );
}
