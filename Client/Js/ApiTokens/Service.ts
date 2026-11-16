import {
  IApiToken,
  IApiTokenList,
  ICreateApiTokenResult,
} from "./Models/IApiToken";

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

function normalizeToken(raw: any): IApiToken {
  return {
    apiTokenId: pick(raw, "apiTokenId", "ApiTokenId", 0),
    tokenName: pick(raw, "tokenName", "TokenName", ""),
    createdOnDate: String(pick(raw, "createdOnDate", "CreatedOnDate", "")),
    expiresOn: String(pick(raw, "expiresOn", "ExpiresOn", "")),
    lastUsedOnDate: (() => {
      const v = pick<any>(raw, "lastUsedOnDate", "LastUsedOnDate", null);
      return v == null || v === "" ? null : String(v);
    })(),
    isRevoked: !!pick(raw, "isRevoked", "IsRevoked", false),
    isExpired: !!pick(raw, "isExpired", "IsExpired", false),
    keys: pick(raw, "keys", "Keys", ""),
    status: pick(raw, "status", "Status", ""),
  };
}

function normalizeList(raw: any): IApiTokenList {
  const itemsRaw = pick<any[]>(raw, "items", "Items", []) || [];
  return {
    items: itemsRaw.map(normalizeToken),
    page: pick(raw, "page", "Page", 0),
    pageSize: pick(raw, "pageSize", "PageSize", 25),
    total: pick(raw, "total", "Total", 0),
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

  public listTokens(
    page: number,
    pageSize: number,
    success: (data: IApiTokenList) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "GET",
      "ApiTokens",
      "List",
      { page, pageSize },
      (raw) => success(normalizeList(raw)),
      fail
    );
  }

  public createToken(
    tokenName: string,
    success: (data: ICreateApiTokenResult) => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "ApiTokens",
      "Create",
      { TokenName: tokenName },
      (raw) =>
        success({
          token: pick(raw, "token", "Token", ""),
        }),
      fail
    );
  }

  public revokeToken(
    apiTokenId: number,
    success: () => void,
    fail?: (error: string) => void
  ): void {
    this.ajaxCall(
      "POST",
      "ApiTokens",
      "Revoke",
      { ApiTokenId: apiTokenId },
      () => success(),
      fail
    );
  }
}
