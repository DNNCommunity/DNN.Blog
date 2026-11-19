import React, { useState } from "react";
import ConfirmDialog from "../../Common/ConfirmDialog";
import { IComment } from "../Models/IComment";
import { ICommentsLabels } from "../Models/ICommentsBootstrap";
import DataService from "../Service";

export interface CommentModerationProps {
  blogId: number;
  comment: IComment;
  labels: ICommentsLabels;
  service: DataService;
  onApproved: (comment: IComment) => void;
  onDeleted: (comment: IComment) => void;
}

export default function CommentModeration(props: CommentModerationProps) {
  const { blogId, comment, labels, service, onApproved, onDeleted } = props;
  const [busy, setBusy] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);

  if (comment.deleted) {
    return null;
  }

  const approve = () => {
    if (busy) return;
    setBusy(true);
    service.approveComment(
      blogId,
      comment.commentId,
      (res) => {
        setBusy(false);
        if (res.result === "success") {
          onApproved({ ...comment, approved: true });
        }
      },
      () => setBusy(false)
    );
  };

  const requestDelete = () => {
    if (busy) return;
    setConfirmOpen(true);
  };

  const confirmDelete = () => {
    if (busy) return;
    setBusy(true);
    service.deleteComment(
      blogId,
      comment.commentId,
      (res) => {
        setBusy(false);
        setConfirmOpen(false);
        if (res.result === "success") {
          onDeleted(
            res.comment || {
              ...comment,
              deleted: true,
              commentHtml: "*** content deleted ***",
            }
          );
        }
      },
      () => {
        setBusy(false);
        setConfirmOpen(false);
      }
    );
  };

  return (
    <span className="blog-comment__moderation">
      {!comment.approved && (
        <button
          type="button"
          className="blog-comment__action"
          disabled={busy}
          onClick={approve}
        >
          {labels.approve}
        </button>
      )}
      <button
        type="button"
        className="blog-comment__action"
        title={labels.deleteComment}
        disabled={busy}
        onClick={requestDelete}
      >
        {labels.deleteComment}
      </button>
      <ConfirmDialog
        open={confirmOpen}
        title={labels.deleteComment}
        message={labels.deleteConfirm}
        confirmLabel={labels.deleteComment}
        cancelLabel={labels.cancel}
        busy={busy}
        onCancel={() => {
          if (!busy) setConfirmOpen(false);
        }}
        onConfirm={confirmDelete}
      />
    </span>
  );
}
