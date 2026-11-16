import * as React from "react";
import { ICategoryNode } from "../../Models/ICategory";

export interface CategoryTreeProps {
  nodes: ICategoryNode[];
  selectedId: number | null;
  expandedIds: Set<number>;
  busy: boolean;
  onSelect: (node: ICategoryNode) => void;
  onToggle: (termId: number) => void;
  onRename: (termId: number, name: string) => void;
  onDrop: (
    sourceId: number,
    targetId: number,
    position: "before" | "after" | "inside"
  ) => void;
}

function CategoryTreeNode(props: {
  node: ICategoryNode;
  depth: number;
  selectedId: number | null;
  expandedIds: Set<number>;
  busy: boolean;
  onSelect: (node: ICategoryNode) => void;
  onToggle: (termId: number) => void;
  onRename: (termId: number, name: string) => void;
  onDrop: (
    sourceId: number,
    targetId: number,
    position: "before" | "after" | "inside"
  ) => void;
}): React.ReactElement {
  const {
    node,
    depth,
    selectedId,
    expandedIds,
    busy,
    onSelect,
    onToggle,
    onRename,
    onDrop,
  } = props;
  const hasChildren = node.Children && node.Children.length > 0;
  const expanded = expandedIds.has(node.TermId);
  const selected = selectedId === node.TermId;
  const [editing, setEditing] = React.useState(false);
  const [editValue, setEditValue] = React.useState(node.Name);
  const inputRef = React.useRef<HTMLInputElement>(null);
  const [dropPos, setDropPos] = React.useState<
    "before" | "after" | "inside" | null
  >(null);

  React.useEffect(() => {
    setEditValue(node.Name);
  }, [node.Name]);

  React.useEffect(() => {
    if (editing && inputRef.current) {
      inputRef.current.focus();
      inputRef.current.select();
    }
  }, [editing]);

  const commitRename = () => {
    setEditing(false);
    const next = editValue.trim();
    if (next && next !== node.Name) {
      onRename(node.TermId, next);
    } else {
      setEditValue(node.Name);
    }
  };

  const onDragStart = (e: React.DragEvent) => {
    e.dataTransfer.setData("text/plain", String(node.TermId));
    e.dataTransfer.effectAllowed = "move";
  };

  const onDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = "move";
    const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
    const y = e.clientY - rect.top;
    const ratio = y / rect.height;
    if (ratio < 0.25) setDropPos("before");
    else if (ratio > 0.75) setDropPos("after");
    else setDropPos("inside");
  };

  const onDragLeave = () => setDropPos(null);

  const onDropNode = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    const sourceId = parseInt(e.dataTransfer.getData("text/plain"), 10);
    const position = dropPos || "inside";
    setDropPos(null);
    if (!sourceId || sourceId === node.TermId || busy) return;
    onDrop(sourceId, node.TermId, position);
  };

  return (
    <li className="blog-cat-tree__item">
      <div
        className={
          "blog-cat-tree__row" +
          (selected ? " is-selected" : "") +
          (dropPos ? ` is-drop-${dropPos}` : "")
        }
        style={{ paddingLeft: `${0.5 + depth * 1.1}rem` }}
        draggable={!busy && !editing}
        onDragStart={onDragStart}
        onDragOver={onDragOver}
        onDragLeave={onDragLeave}
        onDrop={onDropNode}
        onClick={() => onSelect(node)}
        onDoubleClick={(e) => {
          e.stopPropagation();
          if (!busy) {
            setEditing(true);
          }
        }}
      >
        <button
          type="button"
          className="blog-cat-tree__twist"
          disabled={!hasChildren || busy}
          aria-label={expanded ? "Collapse" : "Expand"}
          onClick={(e) => {
            e.stopPropagation();
            if (hasChildren) onToggle(node.TermId);
          }}
        >
          {hasChildren ? (
            <i
              className={
                "fa " + (expanded ? "fa-caret-down" : "fa-caret-right")
              }
              aria-hidden="true"
            />
          ) : (
            <span className="blog-cat-tree__twist-spacer" />
          )}
        </button>
        <i className="fa fa-folder-o blog-cat-tree__icon" aria-hidden="true" />
        {editing ? (
          <input
            ref={inputRef}
            className="blog-cat-tree__edit"
            value={editValue}
            disabled={busy}
            onClick={(e) => e.stopPropagation()}
            onChange={(e) => setEditValue(e.target.value)}
            onBlur={commitRename}
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                e.preventDefault();
                commitRename();
              } else if (e.key === "Escape") {
                setEditValue(node.Name);
                setEditing(false);
              }
            }}
          />
        ) : (
          <span className="blog-cat-tree__label">{node.Name}</span>
        )}
      </div>
      {hasChildren && expanded && (
        <ul className="blog-cat-tree__list">
          {node.Children.map((child) => (
            <CategoryTreeNode
              key={child.TermId}
              node={child}
              depth={depth + 1}
              selectedId={selectedId}
              expandedIds={expandedIds}
              busy={busy}
              onSelect={onSelect}
              onToggle={onToggle}
              onRename={onRename}
              onDrop={onDrop}
            />
          ))}
        </ul>
      )}
    </li>
  );
}

export default function CategoryTree(
  props: CategoryTreeProps
): React.ReactElement {
  const { nodes, ...rest } = props;
  return (
    <ul className="blog-cat-tree__list blog-cat-tree__list--root">
      {nodes.length === 0 ? (
        <li className="blog-cat-tree__empty">No categories yet.</li>
      ) : (
        nodes.map((node) => (
          <CategoryTreeNode
            key={node.TermId}
            node={node}
            depth={0}
            {...rest}
          />
        ))
      )}
    </ul>
  );
}
