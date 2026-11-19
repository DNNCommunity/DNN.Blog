import * as React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import ConfirmDialog from "../../Common/ConfirmDialog";
import { IAppModule } from "../Models/IAppModule";
import { IManageBlogOption } from "../Models/IManagePost";
import {
  IManageComment,
  IManageCommentList,
  ManageCommentSortColumn,
  ManageCommentSortDir,
  ManageCommentStatusFilter,
} from "../Models/IManageComment";
import PostsPager from "./PostsManageTable/PostsPager";
import CommentsTable from "./CommentsManageTable/CommentsTable";

export interface CommentsManageTableProps {
  module: IAppModule;
  blogs: IManageBlogOption[];
  deleteConfirm?: string;
  bulkDeleteConfirm?: string;
  deleteLabel?: string;
  flagLabel?: string;
  flagConfirm?: string;
  bulkDeleteLabel?: string;
  cancelLabel?: string;
  searchPlaceholder?: string;
  authorPlaceholder?: string;
  allBlogsLabel?: string;
  allStatusesLabel?: string;
  showMoreLabel?: string;
  showLessLabel?: string;
  emptyLabel?: string;
  approvedLabel?: string;
  pendingLabel?: string;
  deletedLabel?: string;
}

const PAGE_SIZE = 10;

export default function CommentsManageTable(
  props: CommentsManageTableProps
): React.ReactElement {
  const { module, blogs } = props;
  const deleteConfirm =
    props.deleteConfirm ||
    "Delete this comment? Its content will be replaced with a placeholder.";
  const bulkDeleteConfirm =
    props.bulkDeleteConfirm ||
    "Delete the selected comments? Their content will be replaced with a placeholder.";
  const deleteLabel = props.deleteLabel || "Delete";
  const flagLabel = props.flagLabel || "Flag";
  const flagConfirm =
    props.flagConfirm ||
    "Flag this comment? It will be hidden from public view.";
  const bulkDeleteLabel = props.bulkDeleteLabel || "Delete selected";
  const cancelLabel = props.cancelLabel || "Cancel";
  const searchPlaceholder = props.searchPlaceholder || "Search comments";
  const authorPlaceholder = props.authorPlaceholder || "Filter by author";
  const allBlogsLabel = props.allBlogsLabel || "All blogs";
  const allStatusesLabel = props.allStatusesLabel || "All statuses";
  const showMoreLabel = props.showMoreLabel || "Show more";
  const showLessLabel = props.showLessLabel || "Show less";
  const emptyLabel = props.emptyLabel || "No comments found.";
  const approvedLabel = props.approvedLabel || "Approved";
  const pendingLabel = props.pendingLabel || "Pending";
  const deletedLabel = props.deletedLabel || "Deleted";

  const [items, setItems] = useState<IManageComment[]>([]);
  const [page, setPage] = useState(0);
  const [total, setTotal] = useState(0);
  const [sortColumn, setSortColumn] =
    useState<ManageCommentSortColumn>("CREATEDONDATE");
  const [sortDir, setSortDir] = useState<ManageCommentSortDir>("DESC");
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [blogId, setBlogId] = useState(-1);
  const [authorInput, setAuthorInput] = useState("");
  const [authorSearch, setAuthorSearch] = useState("");
  const [statusFilter, setStatusFilter] =
    useState<ManageCommentStatusFilter>("");
  const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set());
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pendingDelete, setPendingDelete] = useState<IManageComment | null>(
    null
  );
  const [pendingFlag, setPendingFlag] = useState<IManageComment | null>(null);
  const [pendingBulk, setPendingBulk] = useState(false);
  const searchDebounceRef = useRef<number | null>(null);
  const authorDebounceRef = useRef<number | null>(null);
  const requestId = useRef(0);

  const orderBy = `${sortColumn} ${sortDir}`;

  const load = useCallback(() => {
    const id = ++requestId.current;
    setLoading(true);
    setError(null);
    module.service.getManageComments(
      {
        page,
        pageSize: PAGE_SIZE,
        orderBy,
        search,
        blogId,
        authorSearch,
        status: statusFilter,
      },
      (data: IManageCommentList) => {
        if (id !== requestId.current) return;
        setItems(data.Items || []);
        setTotal(data.Total || 0);
        setSelectedIds(new Set());
        setLoading(false);
      },
      (err: string) => {
        if (id !== requestId.current) return;
        setError(err || "Failed to load comments.");
        setItems([]);
        setTotal(0);
        setLoading(false);
      }
    );
  }, [module.service, page, orderBy, search, blogId, authorSearch, statusFilter]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    if (searchDebounceRef.current != null) {
      window.clearTimeout(searchDebounceRef.current);
    }
    searchDebounceRef.current = window.setTimeout(() => {
      setPage(0);
      setSearch(searchInput.trim());
    }, 300);
    return () => {
      if (searchDebounceRef.current != null) {
        window.clearTimeout(searchDebounceRef.current);
      }
    };
  }, [searchInput]);

  useEffect(() => {
    if (authorDebounceRef.current != null) {
      window.clearTimeout(authorDebounceRef.current);
    }
    authorDebounceRef.current = window.setTimeout(() => {
      setPage(0);
      setAuthorSearch(authorInput.trim());
    }, 300);
    return () => {
      if (authorDebounceRef.current != null) {
        window.clearTimeout(authorDebounceRef.current);
      }
    };
  }, [authorInput]);

  const handleSort = (column: ManageCommentSortColumn) => {
    setPage(0);
    if (column === sortColumn) {
      setSortDir((d) => (d === "ASC" ? "DESC" : "ASC"));
    } else {
      setSortColumn(column);
      setSortDir(column === "AUTHOR" ? "ASC" : "DESC");
    }
  };

  const handleBlogChange = (value: string) => {
    const next = parseInt(value, 10);
    setPage(0);
    setBlogId(isNaN(next) || next < 1 ? -1 : next);
  };

  const handleStatusChange = (value: ManageCommentStatusFilter) => {
    setPage(0);
    setStatusFilter(value);
  };

  const toggleSelect = (commentId: number) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(commentId)) next.delete(commentId);
      else next.add(commentId);
      return next;
    });
  };

  const toggleSelectAll = () => {
    const selectable = items.filter((c) => c.CanDelete && !c.Deleted);
    const allSelected =
      selectable.length > 0 &&
      selectable.every((c) => selectedIds.has(c.CommentId));
    if (allSelected) {
      setSelectedIds(new Set());
      return;
    }
    setSelectedIds(new Set(selectable.map((c) => c.CommentId)));
  };

  const confirmDelete = () => {
    const comment = pendingDelete;
    if (!comment) return;
    setPendingDelete(null);
    setBusy(true);
    module.service.deleteManageComment(
      comment.BlogId,
      comment.CommentId,
      () => {
        setBusy(false);
        load();
      },
      (err: string) => {
        setBusy(false);
        window.alert(err || "Delete failed.");
      }
    );
  };

  const confirmFlag = () => {
    const comment = pendingFlag;
    if (!comment) return;
    setPendingFlag(null);
    setBusy(true);
    module.service.unapproveComment(
      comment.BlogId,
      comment.CommentId,
      () => {
        setBusy(false);
        load();
      },
      (err: string) => {
        setBusy(false);
        window.alert(err || "Flag failed.");
      }
    );
  };

  const confirmBulkDelete = () => {
    const ids = Array.from(selectedIds);
    if (ids.length === 0) {
      setPendingBulk(false);
      return;
    }
    setPendingBulk(false);
    setBusy(true);
    module.service.bulkDeleteComments(
      ids,
      () => {
        setBusy(false);
        load();
      },
      (err: string) => {
        setBusy(false);
        window.alert(err || "Bulk delete failed.");
      }
    );
  };

  return (
    <div className="blog-manage__comments">
      {error && <p className="blog-manage__status">{error}</p>}
      <CommentsTable
        items={items}
        blogs={blogs}
        blogId={blogId}
        statusFilter={statusFilter}
        sortColumn={sortColumn}
        sortDir={sortDir}
        busy={busy || loading}
        loading={loading}
        skeletonRows={PAGE_SIZE}
        selectedIds={selectedIds}
        searchInput={searchInput}
        authorInput={authorInput}
        authorPlaceholder={authorPlaceholder}
        searchPlaceholder={searchPlaceholder}
        allBlogsLabel={allBlogsLabel}
        allStatusesLabel={allStatusesLabel}
        showMoreLabel={showMoreLabel}
        showLessLabel={showLessLabel}
        emptyLabel={emptyLabel}
        approvedLabel={approvedLabel}
        pendingLabel={pendingLabel}
        deletedLabel={deletedLabel}
        flagLabel={flagLabel}
        deleteLabel={deleteLabel}
        bulkDeleteLabel={bulkDeleteLabel}
        onSearchChange={setSearchInput}
        onAuthorChange={setAuthorInput}
        onBlogChange={handleBlogChange}
        onStatusChange={handleStatusChange}
        onBulkDelete={() => setPendingBulk(true)}
        onSort={handleSort}
        onToggleSelect={toggleSelect}
        onToggleSelectAll={toggleSelectAll}
        onDelete={setPendingDelete}
        onFlag={setPendingFlag}
      />
      <ConfirmDialog
        open={!!pendingDelete}
        title={deleteLabel}
        message={deleteConfirm}
        confirmLabel={deleteLabel}
        cancelLabel={cancelLabel}
        busy={busy}
        onCancel={() => setPendingDelete(null)}
        onConfirm={confirmDelete}
      />
      <ConfirmDialog
        open={!!pendingFlag}
        title={flagLabel}
        message={flagConfirm}
        confirmLabel={flagLabel}
        cancelLabel={cancelLabel}
        busy={busy}
        onCancel={() => setPendingFlag(null)}
        onConfirm={confirmFlag}
      />
      <ConfirmDialog
        open={pendingBulk}
        title={bulkDeleteLabel}
        message={bulkDeleteConfirm}
        confirmLabel={deleteLabel}
        cancelLabel={cancelLabel}
        busy={busy}
        onCancel={() => setPendingBulk(false)}
        onConfirm={confirmBulkDelete}
      />
      <PostsPager
        page={page}
        pageSize={PAGE_SIZE}
        total={total}
        disabled={busy || loading}
        onPageChange={setPage}
      />
    </div>
  );
}
