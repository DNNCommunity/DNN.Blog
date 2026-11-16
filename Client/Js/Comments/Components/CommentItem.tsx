import React, { useState } from "react";
import { IComment } from "../Models/IComment";
import { ICommentsLabels } from "../Models/ICommentsBootstrap";
import DataService from "../Service";
import { formatRelativeTime } from "../utils/relativeTime";
import CommentCompose from "./CommentCompose";
import CommentKarma from "./CommentKarma";
import CommentModeration from "./CommentModeration";

export interface CommentItemProps {
  blogId: number;
  postId: number;
  comment: IComment;
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

export default function CommentItem(props: CommentItemProps) {
  const {
    blogId,
    postId,
    comment,
    labels,
    locale,
    loggedIn,
    canAddComment,
    canApproveComment,
    service,
    onUpdate,
    onDelete,
    onReplyAdded,
  } = props;
  const [replying, setReplying] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [notice, setNotice] = useState("");

  const submitReply = (payload: {
    comment: string;
    author: string;
    email: string;
    website: string;
  }) => {
    setSubmitting(true);
    setNotice("");
    service.addComment(
      {
        blogId,
        postId,
        parentId: comment.commentId,
        comment: payload.comment,
        author: payload.author,
        email: payload.email,
        website: payload.website,
      },
      (res) => {
        setSubmitting(false);
        if (res.result === "successnotapproved") {
          setNotice(labels.notApproved);
          setReplying(false);
          return;
        }
        if (res.result === "success" && res.comment) {
          onReplyAdded(comment.commentId, res.comment);
          setReplying(false);
        }
      },
      () => setSubmitting(false)
    );
  };

  const className =
    "blog-comment" + (comment.approved ? "" : " blog-comment--pending");

  return (
    <article className={className} id={"comment-" + comment.commentId}>
      <header className="blog-comment__author">
        {comment.profilePicUrl ? (
          <img
            className="blog-comment__avatar"
            src={comment.profilePicUrl}
            alt={comment.displayName}
            width={32}
            height={32}
          />
        ) : (
          <span className="blog-comment__avatar blog-comment__avatar--empty" aria-hidden="true" />
        )}
        <span className="blog-comment__displayname">
          {comment.profileUrl ? (
            <a href={comment.profileUrl}>{comment.displayName}</a>
          ) : (
            comment.displayName
          )}
        </span>
      </header>
      <div
        className="blog-comment__body"
        dangerouslySetInnerHTML={{ __html: comment.commentHtml }}
      />
      <div className="blog-comment__meta">
        <time
          className="blog-comment__time"
          dateTime={comment.createdOnDateUtc}
          title={comment.createdOnDateUtc}
        >
          {formatRelativeTime(comment.createdOnDateUtc, locale)}
        </time>
        <div className="blog-comment__actions">
          <CommentKarma
            blogId={blogId}
            comment={comment}
            labels={labels}
            service={service}
            canKarma={loggedIn}
            onChange={onUpdate}
          />
          {canApproveComment && (
            <CommentModeration
              blogId={blogId}
              comment={comment}
              labels={labels}
              service={service}
              onApproved={onUpdate}
              onDeleted={onDelete}
            />
          )}
          {canAddComment && (
            <button
              type="button"
              className="blog-comment__action"
              onClick={() => setReplying((v) => !v)}
            >
              {labels.reply}
            </button>
          )}
        </div>
      </div>
      {notice && <p className="blog-comments__notice">{notice}</p>}
      {replying && (
        <CommentCompose
          labels={labels}
          loggedIn={loggedIn}
          submitting={submitting}
          submitLabel={labels.reply}
          onSubmit={submitReply}
          onCancel={() => setReplying(false)}
        />
      )}
      {comment.replies && comment.replies.length > 0 && (
        <div className="blog-comment__replies">
          {comment.replies.map((child) => (
            <CommentItem
              key={child.commentId}
              blogId={blogId}
              postId={postId}
              comment={child}
              labels={labels}
              locale={locale}
              loggedIn={loggedIn}
              canAddComment={canAddComment}
              canApproveComment={canApproveComment}
              service={service}
              onUpdate={onUpdate}
              onDelete={onDelete}
              onReplyAdded={onReplyAdded}
            />
          ))}
        </div>
      )}
    </article>
  );
}
