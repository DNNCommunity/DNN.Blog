import { ICategoryTree } from "./Models/ICategory";
import {
  IManageCommentList,
  IManageCommentQuery,
  normalizeManageComment,
} from "./Models/IManageComment";
import { IManagePostList, IManagePostQuery } from "./Models/IManagePost";
import { ITagItem, ITagList, ITagListQuery } from "./Models/ITag";

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
    servicePath: string,
    controller: string,
    action: string,
    id: any,
    data: any,
    success: Function,
    fail?: Function,
    isUploadForm?: boolean
  ): void {
    const method = type === "POSTFORM" ? "POST" : type;
    let url =
      servicePath +
      controller +
      "/" +
      action +
      (id != undefined ? "/" + id : "");

    const headers: Record<string, string> = {
      ModuleId: String(this.moduleId),
      TabId: this.tabId,
      Accept: "application/json",
    };
    const token = antiForgeryToken();
    if (token) headers.RequestVerificationToken = token;

    let body: BodyInit | undefined;
    if (method === "GET" || method === "DELETE") {
      if (data && typeof data === "object") {
        const params = new URLSearchParams();
        Object.keys(data).forEach((key) => {
          const value = data[key];
          if (value != null) params.append(key, String(value));
        });
        const qs = params.toString();
        if (qs) url += (url.indexOf("?") >= 0 ? "&" : "?") + qs;
      }
    } else if (isUploadForm) {
      body = data as FormData;
    } else {
      headers["Content-Type"] = "application/json; charset=utf-8";
      body = JSON.stringify(data ?? {});
    }

    fetch(url, { method, headers, body, credentials: "same-origin" })
      .then(async (response) => {
        const text = await response.text();
        if (!response.ok) {
          if (fail != undefined) {
            fail(text || response.statusText || "Request failed");
          }
          return;
        }
        if (!text) {
          if (success != undefined) success(undefined);
          return;
        }
        try {
          if (success != undefined) success(JSON.parse(text));
        } catch {
          if (fail != undefined) fail(text || "Request failed");
        }
      })
      .catch((error: unknown) => {
        if (fail != undefined) {
          fail(error instanceof Error ? error.message : "Request failed");
        }
      });
  }

  public fooBar(success: Function): void {
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "ProductTemplates",
      "AddItem",
      undefined,
      null,
      success
    );
  }

  public getManagePosts(
    query: IManagePostQuery,
    success: (data: IManagePostList) => void,
    fail?: (error: string) => void
  ): void {
    const params: any = {
      page: query.page,
      pageSize: query.pageSize,
      orderBy: query.orderBy,
    };
    if (query.search) {
      params.search = query.search;
    }
    if (query.blogId != null && query.blogId > 0) {
      params.blogId = query.blogId;
    }
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "Posts",
      "List",
      undefined,
      params,
      (data: any) => {
        success({
          Items: (data && (data.Items || data.items)) || [],
          Page: (data && (data.Page != null ? data.Page : data.page)) || 0,
          PageSize:
            (data && (data.PageSize || data.pageSize)) || query.pageSize,
          Total: (data && (data.Total != null ? data.Total : data.total)) || 0,
        });
      },
      fail
    );
  }

  public approvePost(
    blogId: number,
    postId: number,
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Posts",
      "Approve",
      undefined,
      { BlogId: blogId, PostId: postId },
      success,
      fail
    );
  }

  public deletePost(
    blogId: number,
    postId: number,
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Posts",
      "Delete",
      undefined,
      { BlogId: blogId, PostId: postId },
      success,
      fail
    );
  }

  public getManageComments(
    query: IManageCommentQuery,
    success: (data: IManageCommentList) => void,
    fail?: (error: string) => void
  ): void {
    const params: any = {
      page: query.page,
      pageSize: query.pageSize,
      orderBy: query.orderBy,
    };
    if (query.search) params.search = query.search;
    if (query.blogId != null && query.blogId > 0) params.blogId = query.blogId;
    if (query.authorSearch) params.authorSearch = query.authorSearch;
    if (query.status) params.status = query.status;
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "Comments",
      "ManageList",
      undefined,
      params,
      (data: any) => {
        const rawItems = (data && (data.Items || data.items)) || [];
        success({
          Items: rawItems.map(normalizeManageComment),
          Page: (data && (data.Page != null ? data.Page : data.page)) || 0,
          PageSize:
            (data && (data.PageSize || data.pageSize)) || query.pageSize,
          Total: (data && (data.Total != null ? data.Total : data.total)) || 0,
        });
      },
      fail
    );
  }

  public deleteManageComment(
    blogId: number,
    commentId: number,
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Comments",
      "Delete",
      undefined,
      { BlogId: blogId, CommentId: commentId },
      success,
      fail
    );
  }

  public updateManageComment(
    blogId: number,
    commentId: number,
    comment: string,
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Comments",
      "Update",
      undefined,
      { BlogId: blogId, CommentId: commentId, Comment: comment },
      success,
      fail
    );
  }

  public unapproveComment(
    blogId: number,
    commentId: number,
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Comments",
      "Unapprove",
      undefined,
      { BlogId: blogId, CommentId: commentId },
      success,
      fail
    );
  }

  public bulkDeleteComments(
    commentIds: number[],
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Comments",
      "BulkDelete",
      undefined,
      { CommentIds: commentIds },
      success,
      fail
    );
  }

  private normalizeCategoryTree(data: any): ICategoryTree {
    const locales = ((data && (data.Locales || data.locales)) || []).map(
      (l: any) => ({
        Code: l.Code || l.code || "",
        Name: l.Name || l.name || "",
      })
    );
    const mapNode = (n: any): any => ({
      TermId: n.TermId != null ? n.TermId : n.termId,
      Name: n.Name || n.name || "",
      ParentTermId:
        n.ParentTermId != null
          ? n.ParentTermId
          : n.parentTermId != null
            ? n.parentTermId
            : -1,
      ViewOrder:
        n.ViewOrder != null
          ? n.ViewOrder
          : n.viewOrder != null
            ? n.viewOrder
            : 0,
      Localizations: n.Localizations || n.localizations || {},
      Children: ((n.Children || n.children) || []).map(mapNode),
    });
    return {
      Items: ((data && (data.Items || data.items)) || []).map(mapNode),
      DefaultLocale: (data && (data.DefaultLocale || data.defaultLocale)) || "",
      Locales: locales,
      IsMultilingual: !!(
        data &&
        (data.IsMultilingual != null
          ? data.IsMultilingual
          : data.isMultilingual)
      ),
    };
  }

  public getCategoryTree(
    success: (data: ICategoryTree) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "Categories",
      "Tree",
      undefined,
      null,
      (data: any) => success(this.normalizeCategoryTree(data)),
      fail
    );
  }

  public addCategories(
    lines: string,
    parentTermId: number,
    success: (data: ICategoryTree) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Categories",
      "Add",
      undefined,
      { Lines: lines, ParentTermId: parentTermId },
      (data: any) => success(this.normalizeCategoryTree(data)),
      fail
    );
  }

  public updateCategory(
    payload: {
      TermId: number;
      Name?: string;
      ParentTermId?: number;
      ViewOrder?: number;
    },
    success: (data: ICategoryTree) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Categories",
      "Update",
      undefined,
      payload,
      (data: any) => success(this.normalizeCategoryTree(data)),
      fail
    );
  }

  public deleteCategory(
    termId: number,
    success: (data: ICategoryTree) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Categories",
      "Delete",
      undefined,
      { TermId: termId },
      (data: any) => success(this.normalizeCategoryTree(data)),
      fail
    );
  }

  public localizeCategory(
    payload: {
      TermId: number;
      Name: string;
      Localizations: Record<string, string>;
    },
    success: (data: ICategoryTree) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Categories",
      "Localize",
      undefined,
      payload,
      (data: any) => success(this.normalizeCategoryTree(data)),
      fail
    );
  }

  private parseApiError(responseText: string, fallback: string): string {
    if (!responseText) return fallback;
    try {
      const data = JSON.parse(responseText);
      return (data && (data.Message || data.message)) || fallback;
    } catch {
      return fallback;
    }
  }

  private normalizeTagList(data: any, pageSizeFallback: number): ITagList {
    const locales = ((data && (data.Locales || data.locales)) || []).map(
      (l: any) => ({
        Code: l.Code || l.code || "",
        Name: l.Name || l.name || "",
      })
    );
    const mapItem = (t: any): ITagItem => ({
      TermId: t.TermId != null ? t.TermId : t.termId,
      Name: t.Name || t.name || "",
      Localizations: t.Localizations || t.localizations || {},
    });
    return {
      Items: ((data && (data.Items || data.items)) || []).map(mapItem),
      Page: (data && (data.Page != null ? data.Page : data.page)) || 0,
      PageSize: (data && (data.PageSize || data.pageSize)) || pageSizeFallback,
      Total: (data && (data.Total != null ? data.Total : data.total)) || 0,
      DefaultLocale: (data && (data.DefaultLocale || data.defaultLocale)) || "",
      Locales: locales,
    };
  }

  public getTags(
    query: ITagListQuery,
    success: (data: ITagList) => void,
    fail?: (error: string) => void
  ): void {
    const params: any = {
      page: query.page,
      pageSize: query.pageSize,
    };
    if (query.search) {
      params.search = query.search;
    }
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "Tags",
      "List",
      undefined,
      params,
      (data: any) => success(this.normalizeTagList(data, query.pageSize)),
      (text: string) => fail && fail(this.parseApiError(text, "Failed to load tags."))
    );
  }

  public addTag(
    name: string,
    success: (item: ITagItem) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Tags",
      "Add",
      undefined,
      { Name: name },
      (data: any) => {
        const item = (data && (data.Item || data.item)) || data;
        success({
          TermId: item.TermId != null ? item.TermId : item.termId,
          Name: item.Name || item.name || name,
          Localizations: item.Localizations || item.localizations || {},
        });
      },
      (text: string) => fail && fail(this.parseApiError(text, "Failed to add tag."))
    );
  }

  public updateTagCell(
    payload: { TermId: number; Locale: string; Value: string },
    success: (value: string) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Tags",
      "UpdateCell",
      undefined,
      payload,
      (data: any) => {
        const value = data && (data.Value != null ? data.Value : data.value);
        success(value != null ? value : payload.Value);
      },
      (text: string) => fail && fail(this.parseApiError(text, "Failed to save."))
    );
  }

  public deleteTag(
    termId: number,
    success: Function,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      this.baseServicepath,
      "Tags",
      "Delete",
      undefined,
      { TermId: termId },
      success,
      (text: string) =>
        fail && fail(this.parseApiError(text, "Failed to delete tag."))
    );
  }
}
