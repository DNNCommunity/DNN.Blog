import React from "react";
import { IComment } from "../Models/IComment";
import { ICommentsLabels } from "../Models/ICommentsBootstrap";
import DataService from "../Service";
import CommentItem from "./CommentItem";

export interface CommentListProps {
  blogId: number;
  postId: number;
  comments: IComment[];
  labels: ICommentsLabels;
  locale: string;
  loggedIn: boolean;
  canAddComment: boolean;
  canApproveComment: boolean;
  service: DataService;
  onUpdate: (comment: IComment) => void;
  onDelete: (commentId: number) => void;
  onReplyAdded: (parentId: number, comment: IComment) => void;
}

export default function CommentList(props: CommentListProps) {
  const { comments, labels } = props;

  if (!comments.length) {
    return <p className="blog-comments__empty">{labels.empty}</p>;
  }

  return (
    <div className="blog-comments__list">
      {comments.map((c) => (
        <CommentItem key={c.commentId} {...props} comment={c} />
      ))}
    </div>
  );
}
