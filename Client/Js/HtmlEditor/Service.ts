export interface DnnServiceFramework extends JQueryStatic {
  dnnSF(moduleId: number): DnnServiceFramework;
  getServiceRoot(path: string): string;
  setModuleHeaders(): void;
  getTabId(): string;
}

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

export default class DataService {
  private moduleId: number = -1;
  private dnn: DnnServiceFramework = $ as DnnServiceFramework;
  public baseServicepath: string = "";
  public internalServicesPath: string = "";
  public tabId: string = "";

  constructor(mid: number) {
    this.moduleId = mid;
    this.baseServicepath = this.dnn
      .dnnSF(this.moduleId)
      .getServiceRoot("DotNetNuke/Blog");
    this.internalServicesPath = this.dnn
      .dnnSF(this.moduleId)
      .getServiceRoot("InternalServices");
    this.tabId = this.dnn.dnnSF(this.moduleId).getTabId();
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
    var opts: JQuery.AjaxSettings = {
      type: type === "POSTFORM" ? "POST" : type,
      url:
        servicePath +
        controller +
        "/" +
        action +
        (id != undefined ? "/" + id : ""),
      beforeSend: this.dnn.dnnSF(this.moduleId).setModuleHeaders,
      contentType:
        type === "POSTFORM" ? undefined : "application/json; charset=utf-8",
      data: type == "POST" ? JSON.stringify(data) : data,
      dataType: "json",
    };
    if (isUploadForm) {
      opts.contentType = false;
      opts.processData = false;
    }
    $.ajax(opts)
      .done(function (retdata: any) {
        if (success != undefined) {
          success(retdata);
        }
      })
      .fail(function (xhr: any) {
        if (fail != undefined) {
          fail(xhr.responseText || xhr.statusText || "Request failed");
        }
      });
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
