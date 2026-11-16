import { IComment } from "./IComment";

export interface ICommentsLabels {
  total: string;
  comments: string;
  comment: string;
  reply: string;
  like: string;
  dislike: string;
  report: string;
  likeTitle: string;
  dislikeTitle: string;
  reportTitle: string;
  reportConfirm: string;
  approve: string;
  deleteComment: string;
  deleteConfirm: string;
  submit: string;
  cancel: string;
  name: string;
  email: string;
  website: string;
  notApproved: string;
  empty: string;
  required: string;
}

export interface ICommentsBootstrap {
  blogId: number;
  postId: number;
  nrComments: number;
  loggedIn: boolean;
  canAddComment: boolean;
  canApproveComment: boolean;
  allowComments: boolean;
  labels: ICommentsLabels;
  initialComments?: IComment[];
  antiforgeryToken?: string;
  antiforgeryFieldName?: string;
}
