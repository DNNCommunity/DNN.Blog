import * as React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import { IAppModule } from "../Models/IAppModule";
import {
  IManageBlogOption,
  IManagePost,
  IManagePostList,
  ManagePostSortColumn,
  ManagePostSortDir,
} from "../Models/IManagePost";
import PostsPager from "./PostsManageTable/PostsPager";
import PostsTable from "./PostsManageTable/PostsTable";

export interface PostsManageTableProps {
  module: IAppModule;
  editUrl: string;
  blogs: IManageBlogOption[];
}

const PAGE_SIZE = 10;

export default function PostsManageTable(
  props: PostsManageTableProps
): React.ReactElement {
  const { module, editUrl, blogs } = props;
  const [items, setItems] = useState<IManagePost[]>([]);
  const [page, setPage] = useState(0);
  const [total, setTotal] = useState(0);
  const [sortColumn, setSortColumn] =
    useState<ManagePostSortColumn>("PUBLISHEDONDATE");
  const [sortDir, setSortDir] = useState<ManagePostSortDir>("DESC");
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [blogId, setBlogId] = useState(-1);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const debounceRef = useRef<number | null>(null);
  const requestId = useRef(0);

  const orderBy = `${sortColumn} ${sortDir}`;

  const load = useCallback(() => {
    const id = ++requestId.current;
    setLoading(true);
    setError(null);
    module.service.getManagePosts(
      { page, pageSize: PAGE_SIZE, orderBy, search, blogId },
      (data: IManagePostList) => {
        if (id !== requestId.current) return;
        setItems(data.Items || []);
        setTotal(data.Total || 0);
        setLoading(false);
      },
      (err: string) => {
        if (id !== requestId.current) return;
        setError(err || "Failed to load posts.");
        setItems([]);
        setTotal(0);
        setLoading(false);
      }
    );
  }, [module.service, page, orderBy, search, blogId]);

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

  const handleSort = (column: ManagePostSortColumn) => {
    setPage(0);
    if (column === sortColumn) {
      setSortDir((d) => (d === "ASC" ? "DESC" : "ASC"));
    } else {
      setSortColumn(column);
      setSortDir(column === "TITLE" ? "ASC" : "DESC");
    }
  };

  const handleBlogChange = (value: string) => {
    const next = parseInt(value, 10);
    setPage(0);
    setBlogId(isNaN(next) || next < 1 ? -1 : next);
  };

  const handleDelete = (post: IManagePost) => {
    if (!window.confirm("Delete this post?")) return;
    setBusy(true);
    module.service.deletePost(
      post.BlogId,
      post.PostId,
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

  const handleApprove = (post: IManagePost) => {
    setBusy(true);
    module.service.approvePost(
      post.BlogId,
      post.PostId,
      () => {
        setBusy(false);
        load();
      },
      (err: string) => {
        setBusy(false);
        window.alert(err || "Approve failed.");
      }
    );
  };

  return (
    <div className="blog-manage__posts">
      <div className="blog-manage__toolbar">
        <input
          type="search"
          className="blog-manage__search"
          placeholder="Search posts by title…"
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          aria-label="Search posts"
        />
        <select
          className="blog-manage__blog-filter"
          value={blogId}
          onChange={(e) => handleBlogChange(e.target.value)}
          aria-label="Filter by blog"
        >
          <option value={-1}>All Blogs</option>
          {(blogs || []).map((blog) => (
            <option key={blog.BlogId} value={blog.BlogId}>
              {blog.Title}
            </option>
          ))}
        </select>
      </div>
      {error && <p className="blog-manage__status">{error}</p>}
      {loading && !error && (
        <p className="blog-manage__status">Loading posts…</p>
      )}
      <PostsTable
        items={items}
        sortColumn={sortColumn}
        sortDir={sortDir}
        editUrl={editUrl}
        busy={busy || loading}
        onSort={handleSort}
        onDelete={handleDelete}
        onApprove={handleApprove}
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
