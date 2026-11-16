import type { UploadFn, UploadResult } from "./types";

export interface DnnUploadOptions {
  moduleId: number;
  blogId: number;
  postId: number;
  uploadUrl?: string;
}

declare const $: any;

/** Accept camelCase or PascalCase payloads from DNN Web API. */
export function normalizeUploadResult(raw: unknown): UploadResult {
  const data = (raw ?? {}) as Record<string, unknown>;
  const url = String(data.url ?? data.Url ?? "").trim();
  const fileName = String(data.fileName ?? data.FileName ?? "").trim();
  if (!url) {
    throw new Error("Upload response missing url");
  }
  return { url, fileName: fileName || "file" };
}

function getServiceRoot(moduleId: number): string {
  if (typeof $ !== "undefined" && $.dnnSF) {
    return $.dnnSF(moduleId).getServiceRoot("DotNetNuke/Blog");
  }
  return "/DesktopModules/DotNetNuke/Blog/API/";
}

function setModuleHeaders(xhr: XMLHttpRequest, moduleId: number): void {
  if (typeof $ !== "undefined" && $.dnnSF) {
    const sf = $.dnnSF(moduleId);
    const tabId = sf.getTabId?.();
    if (tabId) xhr.setRequestHeader("TabId", String(tabId));
    xhr.setRequestHeader("ModuleId", String(moduleId));
    const token =
      (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement)
        ?.value || "";
    if (token) xhr.setRequestHeader("RequestVerificationToken", token);
  }
}

export function createDnnUploadFn(opts: DnnUploadOptions): UploadFn {
  const url =
    opts.uploadUrl ||
    `${getServiceRoot(opts.moduleId)}Media/Upload`;

  return (file: File) =>
    new Promise<UploadResult>((resolve, reject) => {
      const form = new FormData();
      form.append("blogId", String(opts.blogId));
      form.append("postId", String(opts.postId));
      form.append("file", file, file.name);

      const xhr = new XMLHttpRequest();
      xhr.open("POST", url, true);
      if (typeof $ !== "undefined" && $.dnnSF) {
        try {
          $.dnnSF(opts.moduleId).setModuleHeaders(xhr);
        } catch {
          setModuleHeaders(xhr, opts.moduleId);
        }
      } else {
        setModuleHeaders(xhr, opts.moduleId);
      }

      xhr.onload = () => {
        if (xhr.status === 403) {
          reject(new Error("This file type is not allowed."));
          return;
        }
        if (xhr.status < 200 || xhr.status >= 300) {
          let msg = xhr.responseText || xhr.statusText || "Upload failed";
          try {
            const parsed = JSON.parse(xhr.responseText);
            if (typeof parsed === "string") msg = parsed;
            else if (parsed?.Message) msg = parsed.Message;
            else if (parsed?.ExceptionMessage) msg = parsed.ExceptionMessage;
          } catch {
            /* keep text */
          }
          reject(new Error(msg));
          return;
        }
        try {
          resolve(normalizeUploadResult(JSON.parse(xhr.responseText)));
        } catch (err: any) {
          reject(new Error(err?.message || "Invalid upload response"));
        }
      };
      xhr.onerror = () => reject(new Error("Network error during upload"));
      xhr.send(form);
    });
}

export function isImageFile(file: File): boolean {
  if (file.type.startsWith("image/")) return true;
  return /\.(jpe?g|png|gif)$/i.test(file.name);
}
