import * as React from "react";
import { IPostCategoryNode } from "../Models/IPostEditBootstrap";
import "./CategoryPicker.scss";

export interface CategoryPickerProps {
  nodes: IPostCategoryNode[];
  selectedIds: Set<number>;
  onChange: (selectedIds: Set<number>) => void;
}

function collectExpanded(nodes: IPostCategoryNode[], into: Set<number>): void {
  for (const n of nodes) {
    if (n.Children && n.Children.length > 0) {
      into.add(n.TermId);
      collectExpanded(n.Children, into);
    }
  }
}

function CategoryNode(props: {
  node: IPostCategoryNode;
  selectedIds: Set<number>;
  expandedIds: Set<number>;
  onToggleExpand: (termId: number) => void;
  onToggleSelect: (termId: number, checked: boolean) => void;
}): React.ReactElement {
  const { node, selectedIds, expandedIds, onToggleExpand, onToggleSelect } =
    props;
  const hasChildren = node.Children && node.Children.length > 0;
  const expanded = expandedIds.has(node.TermId);
  const checked = selectedIds.has(node.TermId);

  return (
    <li className="blog-cat-picker__item">
      <div className="blog-cat-picker__row">
        {hasChildren ? (
          <button
            type="button"
            className="blog-cat-picker__twist"
            aria-expanded={expanded}
            aria-label="Toggle"
            onClick={() => onToggleExpand(node.TermId)}
          />
        ) : (
          <span className="blog-cat-picker__twist-spacer" />
        )}
        <label className="blog-cat-picker__label">
          <input
            type="checkbox"
            checked={checked}
            onChange={(e) => onToggleSelect(node.TermId, e.target.checked)}
          />
          <span className="blog-cat-picker__name">{node.Name}</span>
        </label>
      </div>
      {hasChildren && expanded && (
        <ul className="blog-cat-picker__list">
          {node.Children.map((child) => (
            <CategoryNode
              key={child.TermId}
              node={child}
              selectedIds={selectedIds}
              expandedIds={expandedIds}
              onToggleExpand={onToggleExpand}
              onToggleSelect={onToggleSelect}
            />
          ))}
        </ul>
      )}
    </li>
  );
}

export default function CategoryPicker(
  props: CategoryPickerProps
): React.ReactElement {
  const { nodes, selectedIds, onChange } = props;
  const [expandedIds, setExpandedIds] = React.useState<Set<number>>(() => {
    const s = new Set<number>();
    collectExpanded(nodes, s);
    return s;
  });

  const onToggleExpand = (termId: number) => {
    setExpandedIds((prev) => {
      const next = new Set(prev);
      if (next.has(termId)) next.delete(termId);
      else next.add(termId);
      return next;
    });
  };

  const onToggleSelect = (termId: number, checked: boolean) => {
    const next = new Set(selectedIds);
    if (checked) next.add(termId);
    else next.delete(termId);
    onChange(next);
  };

  return (
    <div className="blog-cat-picker">
      {selectedIds.size > 0
        ? Array.from(selectedIds).map((id) => (
            <input
              key={id}
              type="hidden"
              name="SelectedCategoryIds"
              value={id}
            />
          ))
        : null}
      {nodes.length === 0 ? (
        <p className="blog-cat-picker__empty">No categories</p>
      ) : (
        <ul className="blog-cat-picker__list blog-cat-picker__list--root">
          {nodes.map((node) => (
            <CategoryNode
              key={node.TermId}
              node={node}
              selectedIds={selectedIds}
              expandedIds={expandedIds}
              onToggleExpand={onToggleExpand}
              onToggleSelect={onToggleSelect}
            />
          ))}
        </ul>
      )}
    </div>
  );
}
