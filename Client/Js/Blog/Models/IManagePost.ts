export interface IManagePost {
  BlogId: number;
  PostId: number;
  Title: string;
  PublishedOnDate: string;
  Published: boolean;
  BlogTitle: string;
  CanEdit: boolean;
  CanApprove: boolean;
}

export interface IManagePostList {
  Items: IManagePost[];
  Page: number;
  PageSize: number;
  Total: number;
}

export interface IManagePostQuery {
  page: number;
  pageSize: number;
  orderBy: string;
  search: string;
  blogId?: number;
}

export interface IManageBlogOption {
  BlogId: number;
  Title: string;
}

export type ManagePostSortColumn = "PUBLISHEDONDATE" | "TITLE" | "PUBLISHED";
export type ManagePostSortDir = "ASC" | "DESC";
