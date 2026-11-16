import * as React from "react";
import {
  IManagePost,
  ManagePostSortColumn,
  ManagePostSortDir,
} from "../../Models/IManagePost";

export interface PostsTableProps {
  items: IManagePost[];
  sortColumn: ManagePostSortColumn;
  sortDir: ManagePostSortDir;
  editUrl: string;
  busy: boolean;
  onSort: (column: ManagePostSortColumn) => void;
  onDelete: (post: IManagePost) => void;
  onApprove: (post: IManagePost) => void;
}

function formatDate(value: string): string {
  if (!value) return "";
  const d = new Date(value);
  if (isNaN(d.getTime())) return value;
  return d.toLocaleString();
}

function SortHeader(props: {
  label: string;
  column: ManagePostSortColumn;
  sortColumn: ManagePostSortColumn;
  sortDir: ManagePostSortDir;
  onSort: (column: ManagePostSortColumn) => void;
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

function buildEditHref(editUrl: string, blogId: number, postId: number): string {
  try {
    const url = new URL(editUrl, window.location.origin);
    url.searchParams.set("Blog", String(blogId));
    url.searchParams.set("Post", String(postId));
    return `${url.pathname}${url.search}${url.hash}`;
  } catch {
    const sep = editUrl.indexOf("?") >= 0 ? "&" : "?";
    return `${editUrl}${sep}Blog=${blogId}&Post=${postId}`;
  }
}

export default function PostsTable(props: PostsTableProps): React.ReactElement {
  const { items, sortColumn, sortDir, editUrl, busy, onSort, onDelete, onApprove } =
    props;

  if (!busy && items.length === 0) {
    return <p className="blog-manage__empty">No posts found.</p>;
  }

  return (
    <div className="blog-manage__table-wrap">
      <table className="blog-manage__table">
        <thead>
          <tr>
            <th>Actions</th>
            <SortHeader
              label="Date"
              column="PUBLISHEDONDATE"
              sortColumn={sortColumn}
              sortDir={sortDir}
              onSort={onSort}
            />
            <SortHeader
              label="Title"
              column="TITLE"
              sortColumn={sortColumn}
              sortDir={sortDir}
              onSort={onSort}
            />
            <SortHeader
              label="Published"
              column="PUBLISHED"
              sortColumn={sortColumn}
              sortDir={sortDir}
              onSort={onSort}
            />
            <th>Blog</th>
          </tr>
        </thead>
        <tbody>
          {items.map((post) => (
            <tr key={post.PostId}>
              <td className="blog-manage__actions-cell">
                {post.CanEdit && (
                  <>
                    <a
                      className="blog-manage__icon-btn"
                      href={buildEditHref(editUrl, post.BlogId, post.PostId)}
                      title="Edit"
                    >
                      <i className="fa fa-pencil-square-o" aria-hidden="true" />
                    </a>
                    <button
                      type="button"
                      className="blog-manage__icon-btn blog-manage__icon-btn--danger"
                      title="Delete"
                      disabled={busy}
                      onClick={() => onDelete(post)}
                    >
                      <i className="fa fa-times" aria-hidden="true" />
                    </button>
                  </>
                )}
                {post.CanApprove && (
                  <button
                    type="button"
                    className="blog-manage__icon-btn blog-manage__icon-btn--success"
                    title="Approve"
                    disabled={busy}
                    onClick={() => onApprove(post)}
                  >
                    <i className="fa fa-check" aria-hidden="true" />
                  </button>
                )}
              </td>
              <td>{formatDate(post.PublishedOnDate)}</td>
              <td>{post.Title}</td>
              <td>
                <span
                  className={
                    "blog-manage__published blog-manage__published--" +
                    (post.Published ? "yes" : "no")
                  }
                  title={post.Published ? "Published" : "Unpublished"}
                >
                  <i
                    className={
                      "fa fa-" + (post.Published ? "check" : "times")
                    }
                    aria-hidden="true"
                  />
                </span>
              </td>
              <td>{post.BlogTitle}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
