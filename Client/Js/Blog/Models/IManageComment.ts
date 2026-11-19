export interface IManageComment {
  CommentId: number;
  BlogId: number;
  BlogTitle: string;
  PostId: number;
  PostTitle: string;
  AuthorDisplayName: string;
  AuthorUserId: number;
  CommentHtml: string;
  CreatedOnDateUtc: string;
  Approved: boolean;
  Deleted: boolean;
  Reports: number;
  CanDelete: boolean;
  CanFlag: boolean;
}

export interface IManageCommentList {
  Items: IManageComment[];
  Page: number;
  PageSize: number;
  Total: number;
}

export interface IManageCommentQuery {
  page: number;
  pageSize: number;
  orderBy: string;
  search: string;
  blogId?: number;
  authorSearch?: string;
  status?: ManageCommentStatusFilter;
}

export type ManageCommentStatusFilter = "" | "approved" | "pending" | "deleted";
export type ManageCommentSortColumn = "CREATEDONDATE" | "AUTHOR" | "APPROVED";
export type ManageCommentSortDir = "ASC" | "DESC";

function pick<T>(obj: any, camel: string, pascal: string, fallback: T): T {
  if (obj == null) return fallback;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel] as T;
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal] as T;
  return fallback;
}

export function normalizeManageComment(raw: any): IManageComment {
  return {
    CommentId: pick(raw, "commentId", "CommentId", 0),
    BlogId: pick(raw, "blogId", "BlogId", 0),
    BlogTitle: pick(raw, "blogTitle", "BlogTitle", ""),
    PostId: pick(raw, "postId", "PostId", 0),
    PostTitle: pick(raw, "postTitle", "PostTitle", ""),
    AuthorDisplayName: pick(raw, "authorDisplayName", "AuthorDisplayName", ""),
    AuthorUserId: pick(raw, "authorUserId", "AuthorUserId", -1),
    CommentHtml: pick(raw, "commentHtml", "CommentHtml", ""),
    CreatedOnDateUtc: pick(raw, "createdOnDateUtc", "CreatedOnDateUtc", ""),
    Approved: !!pick(raw, "approved", "Approved", false),
    Deleted: !!pick(raw, "deleted", "Deleted", false),
    Reports: pick(raw, "reports", "Reports", 0),
    CanDelete: !!pick(raw, "canDelete", "CanDelete", false),
    CanFlag: !!pick(raw, "canFlag", "CanFlag", false),
  };
}
