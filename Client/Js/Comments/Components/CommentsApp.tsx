import React, { useCallback, useEffect, useState } from "react";
import { IAppModule } from "../Models/IAppModule";
import { IComment } from "../Models/IComment";
import { ICommentsBootstrap } from "../Models/ICommentsBootstrap";
import CommentCompose from "./CommentCompose";
import CommentList from "./CommentList";

export interface CommentsAppProps {
  module: IAppModule;
  bootstrap: ICommentsBootstrap;
}

function updateInTree(
  comments: IComment[],
  commentId: number,
  updater: (c: IComment) => IComment | null
): IComment[] {
  const result: IComment[] = [];
  for (const c of comments) {
    if (c.commentId === commentId) {
      const next = updater(c);
      if (next) result.push({ ...next, replies: next.replies || [] });
      continue;
    }
    result.push({
      ...c,
      replies: updateInTree(c.replies || [], commentId, updater),
    });
  }
  return result;
}

function insertReply(
  comments: IComment[],
  parentId: number,
  reply: IComment
): IComment[] {
  return comments.map((c) => {
    if (c.commentId === parentId) {
      return { ...c, replies: [...(c.replies || []), reply] };
    }
    return {
      ...c,
      replies: insertReply(c.replies || [], parentId, reply),
    };
  });
}

export default function CommentsApp(props: CommentsAppProps) {
  const { module, bootstrap } = props;
  const [comments, setComments] = useState<IComment[]>(
    bootstrap.initialComments || []
  );
  const [total, setTotal] = useState(bootstrap.nrComments || 0);
  const [loading, setLoading] = useState(!bootstrap.initialComments);
  const [submitting, setSubmitting] = useState(false);
  const [notice, setNotice] = useState("");
  const [error, setError] = useState("");

  const canAdd =
    bootstrap.canAddComment && bootstrap.allowComments !== false;
  const loggedIn = bootstrap.loggedIn;

  const reload = useCallback(() => {
    setLoading(true);
    module.service.listComments(
      bootstrap.blogId,
      bootstrap.postId,
      (data) => {
        setComments(data.comments || []);
        setTotal(data.total || 0);
        setLoading(false);
      },
      (err) => {
        setError(err || "Failed to load comments");
        setLoading(false);
      }
    );
  }, [module.service, bootstrap.blogId, bootstrap.postId]);

  useEffect(() => {
    if (!bootstrap.initialComments) {
      reload();
    }
  }, [bootstrap.initialComments, reload]);

  const onUpdate = (comment: IComment) => {
    setComments((prev) =>
      updateInTree(prev, comment.commentId, () => comment)
    );
  };

  const onDelete = (commentId: number) => {
    setComments((prev) => updateInTree(prev, commentId, () => null));
    setTotal((t) => Math.max(0, t - 1));
  };

  const onReplyAdded = (parentId: number, comment: IComment) => {
    setComments((prev) => insertReply(prev, parentId, comment));
    setTotal((t) => t + 1);
  };

  const onRootSubmit = (payload: {
    comment: string;
    author: string;
    email: string;
    website: string;
  }) => {
    setSubmitting(true);
    setNotice("");
    setError("");
    module.service.addComment(
      {
        blogId: bootstrap.blogId,
        postId: bootstrap.postId,
        parentId: -1,
        comment: payload.comment,
        author: payload.author,
        email: payload.email,
        website: payload.website,
      },
      (res) => {
        setSubmitting(false);
        if (res.result === "successnotapproved") {
          setNotice(bootstrap.labels.notApproved);
          return;
        }
        if (res.result === "success" && res.comment) {
          setComments((prev) => [...prev, res.comment!]);
          setTotal((t) => t + 1);
        }
      },
      (err) => {
        setSubmitting(false);
        setError(err || "Failed to add comment");
      }
    );
  };

  const labels = bootstrap.labels;

  return (
    <section className="blog-comments">
      <header className="blog-comments__header">
        <h2 className="blog-comments__count">
          {labels.total}: {total} {labels.comments}
        </h2>
      </header>

      {loading && <p className="blog-comments__loading">…</p>}
      {!loading && (
        <CommentList
          blogId={bootstrap.blogId}
          postId={bootstrap.postId}
          comments={comments}
          labels={labels}
          locale={module.locale}
          loggedIn={loggedIn}
          canAddComment={canAdd}
          canApproveComment={bootstrap.canApproveComment}
          service={module.service}
          onUpdate={onUpdate}
          onDelete={onDelete}
          onReplyAdded={onReplyAdded}
        />
      )}

      {notice && <p className="blog-comments__notice">{notice}</p>}
      {error && <p className="blog-comments__error">{error}</p>}

      {canAdd && (
        <CommentCompose
          labels={labels}
          loggedIn={loggedIn}
          submitting={submitting}
          onSubmit={onRootSubmit}
        />
      )}
    </section>
  );
}
