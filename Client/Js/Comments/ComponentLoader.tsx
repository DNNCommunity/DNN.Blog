import React from "react";
import { createRoot } from "react-dom/client";
import { AppManager } from "./AppManager";
import CommentsApp from "./Components/CommentsApp";
import { IComment } from "./Models/IComment";
import { ICommentsBootstrap, ICommentsLabels } from "./Models/ICommentsBootstrap";

export class ComponentLoader {
  public static load(): void {
    document
      .querySelectorAll<HTMLElement>(".Blog[data-component='comments']")
      .forEach((el) => this.mount(el));
  }

  private static mount(el: HTMLElement): void {
    const moduleId = el.dataInt("moduleid");
    const key = moduleId.toString();
    if (!AppManager.Modules.ContainsKey(key)) {
      console.warn("Comments: no DotNetNukeBlog context for module", moduleId);
      return;
    }
    const module = AppManager.Modules.Item(key);
    let bootstrap: any = el.dataObject("bootstrap");
    if (!bootstrap) {
      const script = document.getElementById("blog-comments-bootstrap");
      if (script && script.textContent) {
        try {
          bootstrap = JSON.parse(script.textContent);
        } catch (e) {
          console.warn("Comments: failed to parse bootstrap JSON", e);
        }
      }
    }
    if (!bootstrap) {
      console.warn("Comments: missing bootstrap data");
      return;
    }

    const data = normalizeBootstrap(bootstrap);

    const afInput = document.querySelector(
      '#blog-comments-af input[name="__RequestVerificationToken"]'
    ) as HTMLInputElement | null;
    if (afInput) {
      data.antiforgeryToken = afInput.value;
      data.antiforgeryFieldName = afInput.name || "__RequestVerificationToken";
    }

    const root = createRoot(el);
    root.render(<CommentsApp module={module} bootstrap={data} />);
  }
}

function pick<T>(obj: any, camel: string, pascal: string, fallback: T): T {
  if (obj == null) return fallback;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel] as T;
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal] as T;
  return fallback;
}

function normalizeComment(raw: any): IComment {
  const repliesRaw = pick<any[]>(raw, "replies", "Replies", []) || [];
  return {
    commentId: pick(raw, "commentId", "CommentId", 0),
    parentId: pick(raw, "parentId", "ParentId", -1),
    displayName: pick(raw, "displayName", "DisplayName", ""),
    authorUserId: pick(raw, "authorUserId", "AuthorUserId", -1),
    profilePicUrl: pick(raw, "profilePicUrl", "ProfilePicUrl", ""),
    profileUrl: pick(raw, "profileUrl", "ProfileUrl", ""),
    commentHtml: pick(raw, "commentHtml", "CommentHtml", ""),
    createdOnDateUtc: String(
      pick(raw, "createdOnDateUtc", "CreatedOnDateUtc", "")
    ),
    approved: !!pick(raw, "approved", "Approved", false),
    likes: pick(raw, "likes", "Likes", 0),
    dislikes: pick(raw, "dislikes", "Dislikes", 0),
    reports: pick(raw, "reports", "Reports", 0),
    liked: !!pick(raw, "liked", "Liked", false),
    disliked: !!pick(raw, "disliked", "Disliked", false),
    reported: !!pick(raw, "reported", "Reported", false),
    replies: repliesRaw.map(normalizeComment),
  };
}

function normalizeLabels(raw: any): ICommentsLabels {
  return {
    total: pick(raw, "total", "Total", "Total"),
    comments: pick(raw, "comments", "Comments", "Comments"),
    comment: pick(raw, "comment", "Comment", "Comment"),
    reply: pick(raw, "reply", "Reply", "Reply"),
    like: pick(raw, "like", "Like", "Like"),
    dislike: pick(raw, "dislike", "Dislike", "Dislike"),
    report: pick(raw, "report", "Report", "Report"),
    likeTitle: pick(raw, "likeTitle", "LikeTitle", "Like Comment"),
    dislikeTitle: pick(raw, "dislikeTitle", "DislikeTitle", "Dislike Comment"),
    reportTitle: pick(raw, "reportTitle", "ReportTitle", "Report Comment"),
    reportConfirm: pick(
      raw,
      "reportConfirm",
      "ReportConfirm",
      "Report this comment?"
    ),
    approve: pick(raw, "approve", "Approve", "Approve"),
    deleteComment: pick(raw, "deleteComment", "DeleteComment", "Delete"),
    deleteConfirm: pick(
      raw,
      "deleteConfirm",
      "DeleteConfirm",
      "Delete the comment? This cannot be undone."
    ),
    submit: pick(raw, "submit", "Submit", "Submit"),
    cancel: pick(raw, "cancel", "Cancel", "Cancel"),
    name: pick(raw, "name", "Name", "Name"),
    email: pick(raw, "email", "Email", "Email"),
    website: pick(raw, "website", "Website", "Website"),
    notApproved: pick(
      raw,
      "notApproved",
      "NotApproved",
      "Your comment was submitted and is awaiting approval."
    ),
    empty: pick(raw, "empty", "Empty", "No comments yet."),
    required: pick(raw, "required", "Required", "Please enter a comment."),
  };
}

function normalizeBootstrap(raw: any): ICommentsBootstrap {
  const initialRaw = pick<any[]>(raw, "initialComments", "InitialComments", null);
  return {
    blogId: pick(raw, "blogId", "BlogId", 0),
    postId: pick(raw, "postId", "PostId", 0),
    nrComments: pick(raw, "nrComments", "NrComments", 0),
    loggedIn: !!pick(raw, "loggedIn", "LoggedIn", false),
    canAddComment: !!pick(raw, "canAddComment", "CanAddComment", false),
    canApproveComment: !!pick(
      raw,
      "canApproveComment",
      "CanApproveComment",
      false
    ),
    allowComments: pick(raw, "allowComments", "AllowComments", true) !== false,
    labels: normalizeLabels(pick(raw, "labels", "Labels", {})),
    initialComments: initialRaw ? initialRaw.map(normalizeComment) : undefined,
  };
}
