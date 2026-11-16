export interface MediaUploadResult {
  url: string;
  fileName: string;
}

export interface ItemDto {
  key: string;
  value: string;
  hasChildren: boolean;
  selectable: boolean;
}

export interface ItemTreeNode {
  data: ItemDto;
  children?: ItemTreeNode[];
}

export interface ItemListResponse {
  Success: boolean;
  IgnoreRoot?: boolean;
  Tree: ItemTreeNode;
}

export type FileBrowserMode = "image" | "file";

const IMAGE_FILTER = "jpg,jpeg,png,gif,bmp,svg";

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
  public internalServicesPath: string = "";
  public tabId: string = "";

  constructor(mid: number) {
    this.moduleId = mid;
    this.baseServicepath = serviceRoot("DotNetNuke/Blog");
    this.internalServicesPath = serviceRoot("InternalServices");
    this.tabId = readDnnVar("sf_tabId", "-1");
  }

  private ajaxCall(
    type: string,
    servicePath: string,
    controller: string,
    action: string,
    id: any,
    data: any,
    success: (data: any) => void,
    fail?: (error: string) => void,
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
          if (fail) fail(text || response.statusText || "Request failed");
          return;
        }
        if (!text) {
          success(undefined);
          return;
        }
        try {
          success(JSON.parse(text));
        } catch {
          if (fail) fail(text || "Request failed");
        }
      })
      .catch((error: unknown) => {
        if (fail) {
          fail(error instanceof Error ? error.message : "Request failed");
        }
      });
  }

  public searchTags(
    term: string,
    success: (names: string[]) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "Tags",
      "Search",
      undefined,
      { term },
      (data: any) => {
        if (Array.isArray(data)) {
          success(data.map((x) => String(x)));
        } else {
          success([]);
        }
      },
      fail
    );
  }

  public uploadImage(
    blogId: number,
    postId: number,
    file: File,
    success: (data: MediaUploadResult) => void,
    fail?: (error: string) => void
  ): void {
    const form = new FormData();
    form.append("blogId", String(blogId));
    form.append("postId", String(postId));
    form.append("file", file, file.name);
    this.ajaxCall(
      "POSTFORM",
      this.baseServicepath,
      "Media",
      "Upload",
      undefined,
      form,
      success,
      fail,
      true
    );
  }

  public getFileUrl(
    fileId: number,
    success: (data: { url: string; fileName: string }) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      this.baseServicepath,
      "Media",
      "FileUrl",
      undefined,
      { fileId },
      success,
      fail
    );
  }

  public getFolders(
    parentFolderId: number,
    success: (data: ItemListResponse) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      this.internalServicesPath,
      "ItemListService",
      "GetFolders",
      undefined,
      { parentFolderId, sortOrder: 1, permission: "BROWSE" },
      success,
      fail
    );
  }

  public getFolderDescendants(
    parentId: string,
    success: (items: ItemDto[]) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      this.internalServicesPath,
      "ItemListService",
      "GetFolderDescendants",
      undefined,
      { parentId, sortOrder: 1, searchText: "", permission: "BROWSE" },
      (data: { Success: boolean; Items: ItemDto[] }) => {
        success(data.Items || []);
      },
      fail
    );
  }

  public getFiles(
    parentId: number,
    mode: FileBrowserMode,
    success: (data: ItemListResponse) => void,
    fail?: (error: string) => void
  ): void {
    const filter = mode === "image" ? IMAGE_FILTER : "";
    this.ajaxCall(
      "GET",
      this.internalServicesPath,
      "ItemListService",
      "GetFiles",
      undefined,
      { parentId, filter, sortOrder: 1, permission: "READ" },
      success,
      fail
    );
  }
}
