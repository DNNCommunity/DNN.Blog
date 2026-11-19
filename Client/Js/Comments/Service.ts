import {
  IActionResult,
  ICommentAddResponse,
  ICommentListResponse,
} from "./Models/IComment";

interface DnnPage {
  dnn?: { getVar?: (key: string, fallback?: string) => unknown };
  dnn_vars?: Record<string, unknown>;
}

function readDnnVar(key: string, fallback: string): string {
  const page = window as Window & DnnPage;
  if (page.dnn && typeof page.dnn.getVar === "function") {
    const value = page.dnn.getVar(key, fallback);
    if (value != null && value !== "") return String(value);
  }
  const vars = page.dnn_vars;
  if (vars && vars[key] != null && vars[key] !== "") return String(vars[key]);
  return fallback;
}

function serviceRoot(moduleName: string): string {
  let root = readDnnVar("sf_siteRoot", "/");
  if (!root.endsWith("/")) root += "/";
  return root + "API/" + moduleName + "/";
}

function antiForgeryToken(): string {
  const fromVar = readDnnVar("__RequestVerificationToken", "");
  if (fromVar) return fromVar;
  const input = document.querySelector<HTMLInputElement>(
    'input[name="__RequestVerificationToken"]'
  );
  return input?.value ?? "";
}

function pick<T>(obj: any, camel: string, pascal: string, fallback: T): T {
  if (obj == null) return fallback;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel] as T;
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal] as T;
  return fallback;
}

function normalizeComment(raw: any): import("./Models/IComment").IComment {
  const repliesRaw = pick<any[]>(raw, "replies", "Replies", []) || [];
  return {
    commentId: pick(raw, "commentId", "CommentId", 0),
    parentId: pick(raw, "parentId", "ParentId", -1),
    displayName: pick(raw, "displayName", "DisplayName", ""),
    authorUserId: pick(raw, "authorUserId", "AuthorUserId", -1),
    profilePicUrl: pick(raw, "profilePicUrl", "ProfilePicUrl", ""),
    profileUrl: pick(raw, "profileUrl", "ProfileUrl", ""),
    commentHtml: pick(raw, "commentHtml", "CommentHtml", ""),
    createdOnDateUtc: pick(raw, "createdOnDateUtc", "CreatedOnDateUtc", ""),
    approved: !!pick(raw, "approved", "Approved", false),
    deleted: !!pick(raw, "deleted", "Deleted", false),
    isEdited: !!pick(raw, "isEdited", "IsEdited", false),
    canEdit: !!pick(raw, "canEdit", "CanEdit", false),
    likes: pick(raw, "likes", "Likes", 0),
    dislikes: pick(raw, "dislikes", "Dislikes", 0),
    reports: pick(raw, "reports", "Reports", 0),
    liked: !!pick(raw, "liked", "Liked", false),
    disliked: !!pick(raw, "disliked", "Disliked", false),
    reported: !!pick(raw, "reported", "Reported", false),
    replies: repliesRaw.map(normalizeComment),
  };
}

export default class DataService {
  private moduleId: number = -1;
  public baseServicepath: string = "";
  public tabId: string = "";

  constructor(mid: number) {
    this.moduleId = mid;
    this.baseServicepath = serviceRoot("DotNetNuke/Blog");
    this.tabId = readDnnVar("sf_tabId", "-1");
  }

  private ajaxCall(
    type: string,
    controller: string,
    action: string,
    data: any,
    success: (data: any) => void,
    fail?: (error: string) => void
  ): void {
    let url = this.baseServicepath + controller + "/" + action;
    const headers: Record<string, string> = {
      ModuleId: String(this.moduleId),
      TabId: this.tabId,
      Accept: "application/json",
    };
    const token = antiForgeryToken();
    if (token) headers.RequestVerificationToken = token;

    let body: BodyInit | undefined;
    if (type === "GET") {
      if (data && typeof data === "object") {
        const params = new URLSearchParams();
        Object.keys(data).forEach((key) => {
          const value = data[key];
          if (value != null) params.append(key, String(value));
        });
        const qs = params.toString();
        if (qs) url += (url.indexOf("?") >= 0 ? "&" : "?") + qs;
      }
    } else {
      // Help BlogAuthorize resolve BlogId before body binding.
      if (data && data.blogId != null) {
        url += (url.indexOf("?") >= 0 ? "&" : "?") + "blogId=" + encodeURIComponent(String(data.blogId));
      }
      headers["Content-Type"] = "application/json; charset=utf-8";
      body = JSON.stringify(data ?? {});
    }

    fetch(url, { method: type, headers, body, credentials: "same-origin" })
      .then(async (response) => {
        const text = await response.text();
        if (!response.ok) {
          fail?.(text || response.statusText || "Request failed");
          return;
        }
        if (!text) {
          success(undefined);
          return;
        }
        try {
          success(JSON.parse(text));
        } catch {
          fail?.(text || "Request failed");
        }
      })
      .catch((error: unknown) => {
        fail?.(error instanceof Error ? error.message : "Request failed");
      });
  }

  public listComments(
    blogId: number,
    postId: number,
    success: (data: ICommentListResponse) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      "Comments",
      "List",
      { blogId, postId },
      (raw) => {
        const commentsRaw = pick<any[]>(raw, "comments", "Comments", []) || [];
        success({
          result: pick(raw, "result", "Result", ""),
          total: pick(raw, "total", "Total", 0),
          comments: commentsRaw.map(normalizeComment),
        });
      },
      fail
    );
  }

  public addComment(
    payload: {
      blogId: number;
      postId: number;
      parentId: number;
      comment: string;
      author?: string;
      website?: string;
      email?: string;
    },
    success: (data: ICommentAddResponse) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "Comments",
      "Add",
      payload,
      (raw) => {
        const commentRaw = pick<any>(raw, "comment", "Comment", null);
        success({
          result: pick(raw, "result", "Result", ""),
          comment: commentRaw ? normalizeComment(commentRaw) : undefined,
        });
      },
      fail
    );
  }

  public karmaComment(
    blogId: number,
    commentId: number,
    karma: number,
    success: (data: IActionResult) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "Comments",
      "Karma",
      { blogId, commentId, karma },
      (raw) => {
        const commentRaw = pick<any>(raw, "comment", "Comment", null);
        success({
          result: pick(raw, "result", "Result", ""),
          comment: commentRaw ? normalizeComment(commentRaw) : undefined,
        });
      },
      fail
    );
  }

  public approveComment(
    blogId: number,
    commentId: number,
    success: (data: IActionResult) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "Comments",
      "Approve",
      { blogId, commentId },
      (raw) => success({ result: pick(raw, "result", "Result", "") }),
      fail
    );
  }

  public deleteComment(
    blogId: number,
    commentId: number,
    success: (data: IActionResult) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "Comments",
      "Delete",
      { blogId, commentId },
      (raw) => {
        const commentRaw = pick<any>(raw, "comment", "Comment", null);
        success({
          result: pick(raw, "result", "Result", ""),
          comment: commentRaw ? normalizeComment(commentRaw) : undefined,
        });
      },
      fail
    );
  }

  public updateComment(
    blogId: number,
    commentId: number,
    comment: string,
    success: (data: IActionResult) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "Comments",
      "Update",
      { blogId, commentId, comment },
      (raw) => {
        const commentRaw = pick<any>(raw, "comment", "Comment", null);
        success({
          result: pick(raw, "result", "Result", ""),
          comment: commentRaw ? normalizeComment(commentRaw) : undefined,
        });
      },
      fail
    );
  }
}
