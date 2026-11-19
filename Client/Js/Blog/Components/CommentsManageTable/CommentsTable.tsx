import * as React from "react";
import {
  IManageComment,
  ManageCommentSortColumn,
  ManageCommentSortDir,
  ManageCommentStatusFilter,
} from "../../Models/IManageComment";
import { IManageBlogOption } from "../../Models/IManagePost";
import CommentClamp from "./CommentClamp";

export interface CommentsTableProps {
  items: IManageComment[];
  blogs: IManageBlogOption[];
  blogId: number;
  statusFilter: ManageCommentStatusFilter;
  sortColumn: ManageCommentSortColumn;
  sortDir: ManageCommentSortDir;
  busy: boolean;
  loading: boolean;
  selectedIds: Set<number>;
  skeletonRows?: number;
  searchInput: string;
  authorInput: string;
  authorPlaceholder: string;
  searchPlaceholder: string;
  allBlogsLabel: string;
  allStatusesLabel: string;
  showMoreLabel: string;
  showLessLabel: string;
  emptyLabel: string;
  approvedLabel: string;
  pendingLabel: string;
  deletedLabel: string;
  flagLabel: string;
  editLabel: string;
  deleteLabel: string;
  bulkDeleteLabel: string;
  onSearchChange: (value: string) => void;
  onAuthorChange: (value: string) => void;
  onBlogChange: (value: string) => void;
  onStatusChange: (value: ManageCommentStatusFilter) => void;
  onBulkDelete: () => void;
  onSort: (column: ManageCommentSortColumn) => void;
  onToggleSelect: (commentId: number) => void;
  onToggleSelectAll: () => void;
  onDelete: (comment: IManageComment) => void;
  onFlag: (comment: IManageComment) => void;
  onEdit: (comment: IManageComment) => void;
}

function formatDate(value: string): string {
  if (!value) return "";
  const d = new Date(value);
  if (isNaN(d.getTime())) return value;
  return d.toLocaleString();
}

function SortHeader(props: {
  label: string;
  column: ManageCommentSortColumn;
  sortColumn: ManageCommentSortColumn;
  sortDir: ManageCommentSortDir;
  onSort: (column: ManageCommentSortColumn) => void;
}): React.ReactElement {
  const active = props.sortColumn === props.column;
  const ind = active ? (props.sortDir === "ASC" ? "▲" : "▼") : "↕";
  return (
    <th>
      <button
        type="button"
        className="blog-manage__th-sort"
        onClick={() => props.onSort(props.column)}
      >
        {props.label}
        <span
          className={
            "blog-manage__sort-ind" +
            (active ? " blog-manage__sort-ind--active" : "")
          }
        >
          {ind}
        </span>
      </button>
    </th>
  );
}

function statusLabel(
  comment: IManageComment,
  labels: { approved: string; pending: string; deleted: string }
): string {
  if (comment.Deleted) return labels.deleted;
  if (comment.Approved) return labels.approved;
  return labels.pending;
}

function SkeletonBar(props: {
  width?: "check" | "short" | "medium" | "long";
}): React.ReactElement {
  const widthClass =
    props.width === "check"
      ? " blog-manage__skeleton-bar--check"
      : props.width === "short"
        ? " blog-manage__skeleton-bar--short"
        : props.width === "medium"
          ? " blog-manage__skeleton-bar--medium"
          : props.width === "long"
            ? " blog-manage__skeleton-bar--long"
            : "";
  return <span className={"blog-manage__skeleton-bar" + widthClass} />;
}

function SkeletonRows(props: { count: number }): React.ReactElement {
  return (
    <>
      {Array.from({ length: props.count }, (_, index) => (
        <tr
          key={`skeleton-${index}`}
          className="blog-manage__skeleton-row"
          aria-hidden="true"
        >
          <td className="blog-manage__check-cell">
            <SkeletonBar width="check" />
          </td>
          <td>
            <SkeletonBar width="long" />
            <SkeletonBar width="medium" />
          </td>
          <td>
            <SkeletonBar width="medium" />
          </td>
          <td>
            <SkeletonBar width="medium" />
          </td>
          <td>
            <SkeletonBar width="short" />
          </td>
          <td>
            <SkeletonBar width="short" />
          </td>
          <td className="blog-manage__actions-cell">
            <SkeletonBar width="short" />
          </td>
        </tr>
      ))}
    </>
  );
}

export default function CommentsTable(
  props: CommentsTableProps
): React.ReactElement {
  const {
    items,
    blogs,
    blogId,
    statusFilter,
    sortColumn,
    sortDir,
    busy,
    loading,
    selectedIds,
    skeletonRows = 5,
    searchInput,
    authorInput,
    authorPlaceholder,
    searchPlaceholder,
    allBlogsLabel,
    allStatusesLabel,
    showMoreLabel,
    showLessLabel,
    emptyLabel,
    approvedLabel,
    pendingLabel,
    deletedLabel,
    flagLabel,
    editLabel,
    deleteLabel,
    bulkDeleteLabel,
    onSearchChange,
    onAuthorChange,
    onBlogChange,
    onStatusChange,
    onBulkDelete,
    onSort,
    onToggleSelect,
    onToggleSelectAll,
    onDelete,
    onFlag,
    onEdit,
  } = props;

  const selectable = items.filter((c) => c.CanDelete && !c.Deleted);
  const allSelected =
    selectable.length > 0 && selectable.every((c) => selectedIds.has(c.CommentId));
  const showSkeleton = loading;
  const showEmpty = !loading && items.length === 0;
  const skeletonCount = items.length > 0 ? items.length : skeletonRows;

  return (
    <div className="blog-manage__table-wrap">
      <table className="blog-manage__table blog-manage__table--comments">
        <thead>
          <tr className="blog-manage__filter-row">
            <th className="blog-manage__check-cell" aria-hidden="true" />
            <th>
              <input
                type="search"
                className="blog-manage__col-search"
                placeholder={searchPlaceholder}
                value={searchInput}
                onChange={(e) => onSearchChange(e.target.value)}
                aria-label={searchPlaceholder}
              />
            </th>
            <th>
              <input
                type="search"
                className="blog-manage__author-input"
                placeholder={authorPlaceholder}
                value={authorInput}
                onChange={(e) => onAuthorChange(e.target.value)}
                aria-label={authorPlaceholder}
              />
            </th>
            <th>
              <select
                className="blog-manage__blog-filter"
                value={blogId}
                onChange={(e) => onBlogChange(e.target.value)}
                aria-label="Filter by blog"
              >
                <option value={-1}>{allBlogsLabel}</option>
                {(blogs || []).map((blog) => (
                  <option key={blog.BlogId} value={blog.BlogId}>
                    {blog.Title}
                  </option>
                ))}
              </select>
            </th>
            <th aria-hidden="true" />
            <th>
              <select
                className="blog-manage__status-filter"
                value={statusFilter}
                onChange={(e) =>
                  onStatusChange(e.target.value as ManageCommentStatusFilter)
                }
                aria-label="Filter by status"
              >
                <option value="">{allStatusesLabel}</option>
                <option value="approved">{approvedLabel}</option>
                <option value="pending">{pendingLabel}</option>
                <option value="deleted">{deletedLabel}</option>
              </select>
            </th>
            <th className="blog-manage__actions-cell">
              <button
                type="button"
                className="blog-manage__btn blog-manage__btn--secondary blog-manage__bulk-delete"
                disabled={busy || selectedIds.size === 0}
                onClick={onBulkDelete}
              >
                {bulkDeleteLabel}
                {selectedIds.size > 0 ? ` (${selectedIds.size})` : ""}
              </button>
            </th>
          </tr>
          <tr>
            <th className="blog-manage__check-cell">
              <input
                type="checkbox"
                checked={allSelected}
                disabled={busy || selectable.length === 0}
                onChange={onToggleSelectAll}
                aria-label="Select all comments on this page"
              />
            </th>
            <th>Comment</th>
            <th>Author</th>
            <th>Blog</th>
            <SortHeader
              label="Date"
              column="CREATEDONDATE"
              sortColumn={sortColumn}
              sortDir={sortDir}
              onSort={onSort}
            />
            <SortHeader
              label="Status"
              column="APPROVED"
              sortColumn={sortColumn}
              sortDir={sortDir}
              onSort={onSort}
            />
            <th className="blog-manage__actions-cell">Actions</th>
          </tr>
        </thead>
        <tbody aria-busy={showSkeleton || undefined}>
          {showSkeleton ? (
            <SkeletonRows count={skeletonCount} />
          ) : showEmpty ? (
            <tr className="blog-manage__empty-row">
              <td colSpan={7}>
                <p className="blog-manage__empty">{emptyLabel}</p>
              </td>
            </tr>
          ) : (
            items.map((comment) => (
              <tr
                key={comment.CommentId}
                className={
                  comment.Deleted ? "blog-manage__row--deleted" : undefined
                }
              >
                <td className="blog-manage__check-cell">
                  <input
                    type="checkbox"
                    checked={selectedIds.has(comment.CommentId)}
                    disabled={busy || !comment.CanDelete || comment.Deleted}
                    onChange={() => onToggleSelect(comment.CommentId)}
                    aria-label={`Select comment ${comment.CommentId}`}
                  />
                </td>
                <td>
                  <CommentClamp
                    html={comment.CommentHtml}
                    showMoreLabel={showMoreLabel}
                    showLessLabel={showLessLabel}
                  />
                  {comment.PostTitle ? (
                    <div className="blog-manage__comment-post">
                      {comment.PostTitle}
                    </div>
                  ) : null}
                </td>
                <td>{comment.AuthorDisplayName}</td>
                <td>{comment.BlogTitle}</td>
                <td>{formatDate(comment.CreatedOnDateUtc)}</td>
                <td>
                  {statusLabel(comment, {
                    approved: approvedLabel,
                    pending: pendingLabel,
                    deleted: deletedLabel,
                  })}
                </td>
                <td className="blog-manage__actions-cell">
                  {comment.CanEdit && (
                    <button
                      type="button"
                      className="blog-manage__icon-btn"
                      title={editLabel}
                      disabled={busy}
                      onClick={() => onEdit(comment)}
                    >
                      <i className="fa fa-pencil" aria-hidden="true" />
                    </button>
                  )}
                  {comment.CanFlag && (
                    <button
                      type="button"
                      className="blog-manage__icon-btn"
                      title={flagLabel}
                      disabled={busy}
                      onClick={() => onFlag(comment)}
                    >
                      <i className="fa fa-flag" aria-hidden="true" />
                    </button>
                  )}
                  {comment.CanDelete && (
                    <button
                      type="button"
                      className="blog-manage__icon-btn blog-manage__icon-btn--danger"
                      title={deleteLabel}
                      disabled={busy}
                      onClick={() => onDelete(comment)}
                    >
                      <i className="fa fa-times" aria-hidden="true" />
                    </button>
                  )}
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
