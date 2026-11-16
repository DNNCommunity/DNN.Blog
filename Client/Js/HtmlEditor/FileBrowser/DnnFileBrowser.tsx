import React, { useCallback, useEffect, useState } from "react";
import { Folder, File as FileIcon, X } from "lucide-react";
import DataService, {
  FileBrowserMode,
  ItemDto,
  ItemTreeNode,
} from "../Service";
import "./FileBrowser.scss";

export interface DnnFileBrowserProps {
  service: DataService;
  mode: FileBrowserMode;
  open: boolean;
  onClose: () => void;
  onSelect: (url: string, fileName: string) => void;
}

function flattenRootChildren(tree: ItemTreeNode | undefined): ItemDto[] {
  if (!tree) return [];
  return (tree.children || []).map((c) => c.data);
}

export function DnnFileBrowser({
  service,
  mode,
  open,
  onClose,
  onSelect,
}: DnnFileBrowserProps) {
  const [folders, setFolders] = useState<ItemDto[]>([]);
  const [expanded, setExpanded] = useState<Record<string, ItemDto[]>>({});
  const [currentFolderId, setCurrentFolderId] = useState<number>(-1);
  const [files, setFiles] = useState<ItemDto[]>([]);
  const [selectedFileId, setSelectedFileId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadRoot = useCallback(() => {
    setLoading(true);
    setError(null);
    service.getFolders(
      -1,
      (data) => {
        const roots = flattenRootChildren(data.Tree);
        setFolders(roots);
        if (roots.length > 0) {
          const id = parseInt(roots[0].key, 10);
          setCurrentFolderId(id);
        }
        setLoading(false);
      },
      (err) => {
        setError(err);
        setLoading(false);
      }
    );
  }, [service]);

  const loadFiles = useCallback(
    (folderId: number) => {
      if (folderId < 1) {
        setFiles([]);
        return;
      }
      setLoading(true);
      setSelectedFileId(null);
      service.getFiles(
        folderId,
        mode,
        (data) => {
          setFiles(flattenRootChildren(data.Tree));
          setLoading(false);
        },
        (err) => {
          setError(err);
          setLoading(false);
        }
      );
    },
    [service, mode]
  );

  useEffect(() => {
    if (open) {
      loadRoot();
    } else {
      setFolders([]);
      setFiles([]);
      setExpanded({});
      setSelectedFileId(null);
      setCurrentFolderId(-1);
      setError(null);
    }
  }, [open, loadRoot]);

  useEffect(() => {
    if (open && currentFolderId > 0) {
      loadFiles(currentFolderId);
    }
  }, [open, currentFolderId, loadFiles]);

  const toggleExpand = (folder: ItemDto) => {
    if (expanded[folder.key]) {
      const next = { ...expanded };
      delete next[folder.key];
      setExpanded(next);
      return;
    }
    service.getFolderDescendants(
      folder.key,
      (items) => {
        setExpanded((prev) => ({ ...prev, [folder.key]: items }));
      },
      (err) => setError(err)
    );
  };

  const selectFolder = (folder: ItemDto) => {
    const id = parseInt(folder.key, 10);
    setCurrentFolderId(id);
  };

  const confirmSelect = () => {
    if (!selectedFileId) return;
    const fileId = parseInt(selectedFileId, 10);
    setLoading(true);
    service.getFileUrl(
      fileId,
      (data) => {
        setLoading(false);
        onSelect(data.url, data.fileName);
        onClose();
      },
      (err) => {
        setError(err);
        setLoading(false);
      }
    );
  };

  if (!open) return null;

  const renderFolder = (folder: ItemDto, depth: number) => {
    const kids = expanded[folder.key];
    const isActive = String(currentFolderId) === folder.key;
    return (
      <div key={folder.key} className="blog-fb__folder-node">
        <button
          type="button"
          className={
            "blog-fb__folder-row" + (isActive ? " is-active" : "")
          }
          style={{ paddingLeft: 8 + depth * 14 }}
          onClick={() => selectFolder(folder)}
        >
          {folder.hasChildren ? (
            <span
              className="blog-fb__twist"
              onClick={(e) => {
                e.stopPropagation();
                toggleExpand(folder);
              }}
            >
              {kids ? "▾" : "▸"}
            </span>
          ) : (
            <span className="blog-fb__twist-spacer" />
          )}
          <Folder size={14} />
          <span className="blog-fb__folder-name">{folder.value}</span>
        </button>
        {kids &&
          kids.map((child) => renderFolder(child, depth + 1))}
      </div>
    );
  };

  return (
    <div className="blog-fb__overlay" role="dialog" aria-modal="true">
      <div className="blog-fb__dialog">
        <header className="blog-fb__header">
          <h2 className="blog-fb__title">
            {mode === "image" ? "Select image" : "Select file"}
          </h2>
          <button
            type="button"
            className="blog-fb__icon-btn"
            onClick={onClose}
            aria-label="Close"
          >
            <X size={18} />
          </button>
        </header>
        <div className="blog-fb__body">
          <aside className="blog-fb__tree">
            {folders.map((f) => renderFolder(f, 0))}
          </aside>
          <section className="blog-fb__files">
            {error && <div className="blog-fb__error">{error}</div>}
            {loading && <div className="blog-fb__status">Loading…</div>}
            {!loading && files.length === 0 && (
              <div className="blog-fb__status">No files in this folder</div>
            )}
            <ul className="blog-fb__file-list">
              {files.map((file) => (
                <li key={file.key}>
                  <button
                    type="button"
                    className={
                      "blog-fb__file-row" +
                      (selectedFileId === file.key ? " is-selected" : "")
                    }
                    onClick={() => setSelectedFileId(file.key)}
                    onDoubleClick={() => {
                      setSelectedFileId(file.key);
                      setTimeout(confirmSelect, 0);
                    }}
                  >
                    <FileIcon size={14} />
                    <span>{file.value}</span>
                  </button>
                </li>
              ))}
            </ul>
          </section>
        </div>
        <footer className="blog-fb__footer">
          <button type="button" className="blog-fb__btn" onClick={onClose}>
            Cancel
          </button>
          <button
            type="button"
            className="blog-fb__btn blog-fb__btn--primary"
            disabled={!selectedFileId || loading}
            onClick={confirmSelect}
          >
            Select
          </button>
        </footer>
      </div>
    </div>
  );
}
