import React, { useState } from "react";
import { IComment } from "../Models/IComment";
import { ICommentsLabels } from "../Models/ICommentsBootstrap";
import DataService from "../Service";

export interface CommentKarmaProps {
  blogId: number;
  comment: IComment;
  labels: ICommentsLabels;
  service: DataService;
  canKarma: boolean;
  onChange: (comment: IComment) => void;
}

type KarmaKind = "like" | "dislike" | "report";

const KARMA_VALUE: Record<KarmaKind, number> = {
  like: 0,
  dislike: 1,
  report: 2,
};

export default function CommentKarma(props: CommentKarmaProps) {
  const { blogId, comment, labels, service, canKarma, onChange } = props;
  const [busy, setBusy] = useState(false);

  const toggle = (kind: KarmaKind) => {
    if (!canKarma || busy) return;
    const alreadyActive =
      (kind === "like" && comment.liked) ||
      (kind === "dislike" && comment.disliked) ||
      (kind === "report" && comment.reported);
    if (kind === "report" && !alreadyActive && labels.reportConfirm) {
      if (!window.confirm(labels.reportConfirm)) return;
    }

    setBusy(true);
    service.karmaComment(
      blogId,
      comment.commentId,
      KARMA_VALUE[kind],
      (res) => {
        setBusy(false);
        if (res.result !== "added" && res.result !== "removed") return;
        if (res.comment) {
          onChange({
            ...comment,
            ...res.comment,
            replies: comment.replies,
          });
          return;
        }
        // Optimistic fallback if API omits refreshed comment
        const next = { ...comment };
        if (kind === "like") {
          if (alreadyActive) {
            next.liked = false;
            next.likes = Math.max(0, next.likes - 1);
          } else {
            if (next.disliked) {
              next.disliked = false;
              next.dislikes = Math.max(0, next.dislikes - 1);
            }
            next.liked = true;
            next.likes += 1;
          }
        } else if (kind === "dislike") {
          if (alreadyActive) {
            next.disliked = false;
            next.dislikes = Math.max(0, next.dislikes - 1);
          } else {
            if (next.liked) {
              next.liked = false;
              next.likes = Math.max(0, next.likes - 1);
            }
            next.disliked = true;
            next.dislikes += 1;
          }
        } else {
          if (alreadyActive) {
            next.reported = false;
            next.reports = Math.max(0, next.reports - 1);
          } else {
            next.reported = true;
            next.reports += 1;
          }
        }
        onChange(next);
      },
      () => setBusy(false)
    );
  };

  const karmaClass =
    "blog-comment__karma" +
    (comment.liked ? " blog-comment__karma--liked" : "") +
    (comment.disliked ? " blog-comment__karma--disliked" : "") +
    (comment.reported ? " blog-comment__karma--reported" : "");

  const renderBtn = (
    kind: KarmaKind,
    active: boolean,
    count: number,
    title: string,
    iconOff: string,
    iconOn: string
  ) => {
    const className =
      `blog-comment__karma-btn blog-comment__karma-btn--${kind}` +
      (active ? " blog-comment__karma-btn--active" : "");

    if (!canKarma) {
      return (
        <span className={className} title={title}>
          <i className={active ? iconOn : iconOff} aria-hidden="true" />
          <span className="blog-comment__karma-count">{count}</span>
        </span>
      );
    }

    return (
      <button
        type="button"
        className={className}
        title={title}
        aria-label={title}
        aria-pressed={active}
        disabled={busy}
        onClick={() => toggle(kind)}
      >
        <i className={active ? iconOn : iconOff} aria-hidden="true" />
        <span className="blog-comment__karma-count">{count}</span>
      </button>
    );
  };

  return (
    <span className={karmaClass}>
      {renderBtn(
        "like",
        comment.liked,
        comment.likes,
        labels.likeTitle,
        "fa fa-thumbs-o-up",
        "fa fa-thumbs-up"
      )}
      {renderBtn(
        "dislike",
        comment.disliked,
        comment.dislikes,
        labels.dislikeTitle,
        "fa fa-thumbs-o-down",
        "fa fa-thumbs-down"
      )}
      {renderBtn(
        "report",
        comment.reported,
        comment.reports,
        labels.reportTitle,
        "fa fa-warning",
        "fa fa-warning"
      )}
    </span>
  );
}
