import * as React from "react";

export interface PostsPagerProps {
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (page: number) => void;
  disabled?: boolean;
}

export default function PostsPager(props: PostsPagerProps): React.ReactElement | null {
  const { page, pageSize, total, onPageChange, disabled } = props;
  const totalPages = Math.max(1, Math.ceil(total / pageSize) || 1);
  const current = Math.min(page + 1, totalPages);
  const canPrev = page > 0 && !disabled;
  const canNext = page + 1 < totalPages && !disabled;

  if (total === 0) {
    return null;
  }

  return (
    <div className="blog-manage__pager">
      <span className="blog-manage__pager-info">
        Page {current} of {totalPages} ({total} post{total === 1 ? "" : "s"})
      </span>
      <div className="blog-manage__pager-btns">
        <button
          type="button"
          className="blog-manage__btn blog-manage__btn--secondary"
          disabled={!canPrev}
          onClick={() => onPageChange(page - 1)}
        >
          Previous
        </button>
        <button
          type="button"
          className="blog-manage__btn blog-manage__btn--secondary"
          disabled={!canNext}
          onClick={() => onPageChange(page + 1)}
        >
          Next
        </button>
      </div>
    </div>
  );
}
