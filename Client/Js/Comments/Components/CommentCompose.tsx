import React, { useState } from "react";
import { ICommentsLabels } from "../Models/ICommentsBootstrap";

export interface CommentComposeProps {
  labels: ICommentsLabels;
  loggedIn: boolean;
  submitting?: boolean;
  submitLabel?: string;
  initialComment?: string;
  hideAuthorFields?: boolean;
  onSubmit: (payload: {
    comment: string;
    author: string;
    email: string;
    website: string;
  }) => void;
  onCancel?: () => void;
}

export default function CommentCompose(props: CommentComposeProps) {
  const {
    labels,
    loggedIn,
    submitting,
    submitLabel,
    initialComment = "",
    hideAuthorFields = false,
    onSubmit,
    onCancel,
  } = props;
  const [comment, setComment] = useState(initialComment);
  const [author, setAuthor] = useState("");
  const [email, setEmail] = useState("");
  const [website, setWebsite] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = comment.trim();
    if (!trimmed) {
      setError(labels.required);
      return;
    }
    if (!loggedIn && !hideAuthorFields && !author.trim()) {
      setError(labels.required);
      return;
    }
    setError("");
    onSubmit({
      comment: trimmed,
      author: author.trim(),
      email: email.trim(),
      website: website.trim(),
    });
    if (!initialComment) {
      setComment("");
    }
  };

  return (
    <form className="blog-comments__compose" onSubmit={handleSubmit}>
      {!loggedIn && !hideAuthorFields && (
        <div className="blog-comments__compose-fields">
          <label className="blog-comments__field">
            <span className="blog-comments__field-label">{labels.name}</span>
            <input
              type="text"
              className="blog-comments__input"
              value={author}
              onChange={(e) => setAuthor(e.target.value)}
              disabled={submitting}
            />
          </label>
          <label className="blog-comments__field">
            <span className="blog-comments__field-label">{labels.email}</span>
            <input
              type="email"
              className="blog-comments__input"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={submitting}
            />
          </label>
          <label className="blog-comments__field">
            <span className="blog-comments__field-label">{labels.website}</span>
            <input
              type="url"
              className="blog-comments__input"
              value={website}
              onChange={(e) => setWebsite(e.target.value)}
              disabled={submitting}
            />
          </label>
        </div>
      )}
      <label className="blog-comments__field">
        <span className="blog-comments__field-label">{labels.comment}</span>
        <textarea
          className="blog-comments__textarea"
          rows={4}
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          disabled={submitting}
        />
      </label>
      {error && <p className="blog-comments__error">{error}</p>}
      <div className="blog-comments__compose-actions">
        <button type="submit" className="blog-comments__submit" disabled={submitting}>
          {submitLabel || labels.submit}
        </button>
        {onCancel && (
          <button
            type="button"
            className="blog-comments__cancel"
            onClick={onCancel}
            disabled={submitting}
          >
            {labels.cancel}
          </button>
        )}
      </div>
    </form>
  );
}
