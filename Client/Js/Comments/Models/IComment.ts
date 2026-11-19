export interface IComment {
  commentId: number;
  parentId: number;
  displayName: string;
  authorUserId: number;
  profilePicUrl: string;
  profileUrl: string;
  commentHtml: string;
  createdOnDateUtc: string;
  approved: boolean;
  deleted: boolean;
  likes: number;
  dislikes: number;
  reports: number;
  liked: boolean;
  disliked: boolean;
  reported: boolean;
  replies: IComment[];
}

export interface ICommentListResponse {
  result: string;
  total: number;
  comments: IComment[];
}

export interface ICommentAddResponse {
  result: string;
  comment?: IComment;
}

export interface IActionResult {
  result: string;
  comment?: IComment;
}
