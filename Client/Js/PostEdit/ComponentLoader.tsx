import React from "react";
import { createRoot } from "react-dom/client";
import { AppManager } from "./AppManager";
import PostEditApp from "./Components/PostEditApp";
import { IPostEditBootstrap } from "./Models/IPostEditBootstrap";

export class ComponentLoader {
  public static load(): void {
    document
      .querySelectorAll<HTMLElement>(".Blog[data-component='post-edit']")
      .forEach((el) => this.mount(el));
  }

  private static mount(el: HTMLElement): void {
    const moduleId = el.dataInt("moduleid");
    const key = moduleId.toString();
    if (!AppManager.Modules.ContainsKey(key)) {
      console.warn("PostEdit: no DotNetNukeBlog context for module", moduleId);
      return;
    }
    const module = AppManager.Modules.Item(key);
    let bootstrap: any = el.dataObject("bootstrap");
    if (!bootstrap) {
      const script = document.getElementById("blog-post-edit-bootstrap");
      if (script && script.textContent) {
        try {
          bootstrap = JSON.parse(script.textContent);
        } catch (e) {
          console.warn("PostEdit: failed to parse bootstrap JSON", e);
        }
      }
    }
    if (!bootstrap) {
      console.warn("PostEdit: missing bootstrap data");
      return;
    }

    // Normalize possible PascalCase from Newtonsoft default
    const data = normalizeBootstrap(bootstrap);

    const afInput = document.querySelector(
      '#blog-post-edit-af input[name="__RequestVerificationToken"]'
    ) as HTMLInputElement | null;
    if (afInput) {
      data.antiforgeryToken = afInput.value;
      data.antiforgeryFieldName = afInput.name || "__RequestVerificationToken";
    }

    const root = createRoot(el);
    root.render(<PostEditApp module={module} bootstrap={data} />);
  }
}

function pick<T>(obj: any, camel: string, pascal: string, fallback: T): T {
  if (obj == null) return fallback;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel] as T;
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal] as T;
  return fallback;
}

function normalizeLocale(raw: any) {
  return {
    Code: pick(raw, "code", "Code", ""),
    Name: pick(raw, "name", "Name", ""),
    FlagUrl: pick(raw, "flagUrl", "FlagUrl", ""),
  };
}

function normalizeCategory(raw: any): any {
  const children = pick<any[]>(raw, "children", "Children", []) || [];
  return {
    TermId: pick(raw, "termId", "TermId", 0),
    Name: pick(raw, "name", "Name", ""),
    Depth: pick(raw, "depth", "Depth", 0),
    Selected: !!pick(raw, "selected", "Selected", false),
    Children: children.map(normalizeCategory),
  };
}

function normalizeBootstrap(raw: any): IPostEditBootstrap {
  const labelsRaw = pick<any>(raw, "labels", "Labels", {});
  return {
    blogId: pick(raw, "blogId", "BlogId", -1),
    postId: pick(raw, "postId", "PostId", -1),
    blogTitle: pick(raw, "blogTitle", "BlogTitle", ""),
    isNew: !!pick(raw, "isNew", "IsNew", true),
    title: pick(raw, "title", "Title", ""),
    summary: pick(raw, "summary", "Summary", ""),
    content: pick(raw, "content", "Content", ""),
    titles: pick(raw, "titles", "Titles", {}),
    summaries: pick(raw, "summaries", "Summaries", {}),
    contents: pick(raw, "contents", "Contents", {}),
    published: !!pick(raw, "published", "Published", true),
    publishCanChange: pick<boolean>(raw, "publishCanChange", "PublishCanChange", true) !== false,
    publishNow: !!pick(raw, "publishNow", "PublishNow", true),
    publishDate: pick(raw, "publishDate", "PublishDate", ""),
    timezoneDisplay: pick(raw, "timezoneDisplay", "TimezoneDisplay", ""),
    allowComments: !!pick(raw, "allowComments", "AllowComments", true),
    displayCopyright: !!pick(raw, "displayCopyright", "DisplayCopyright", false),
    copyright: pick(raw, "copyright", "Copyright", ""),
    locale: pick(raw, "locale", "Locale", ""),
    showLocale: !!pick(raw, "showLocale", "ShowLocale", false),
    useFullLocalization: !!pick(raw, "useFullLocalization", "UseFullLocalization", false),
    defaultLocale: pick(raw, "defaultLocale", "DefaultLocale", ""),
    locales: (pick<any[]>(raw, "locales", "Locales", []) || []).map(normalizeLocale),
    otherLocales: (pick<any[]>(raw, "otherLocales", "OtherLocales", []) || []).map(
      normalizeLocale
    ),
    localeDropdownOptions: (
      pick<any[]>(raw, "localeDropdownOptions", "LocaleDropdownOptions", []) || []
    ).map(normalizeLocale),
    tags: pick(raw, "tags", "Tags", ""),
    imageKey: pick(raw, "imageKey", "ImageKey", ""),
    imageUrl: pick(raw, "imageUrl", "ImageUrl", ""),
    showCategories: !!pick(raw, "showCategories", "ShowCategories", false),
    selectedCategoryIds: (() => {
      const ids = pick<number[]>(raw, "selectedCategoryIds", "SelectedCategoryIds", []) || [];
      if (ids.length > 0) return ids;
      const cats = pick<any[]>(raw, "categories", "Categories", []) || [];
      const collected: number[] = [];
      const walk = (nodes: any[]) => {
        for (const n of nodes) {
          if (pick(n, "selected", "Selected", false)) {
            collected.push(pick(n, "termId", "TermId", 0));
          }
          walk(pick(n, "children", "Children", []) || []);
        }
      };
      walk(cats);
      return collected.filter((id) => id > 0);
    })(),
    categories: (pick<any[]>(raw, "categories", "Categories", []) || []).map(
      normalizeCategory
    ),
    summaryIsHtml: !!pick(raw, "summaryIsHtml", "SummaryIsHtml", false),
    showSummaryPrecedingWarning: !!pick(
      raw,
      "showSummaryPrecedingWarning",
      "ShowSummaryPrecedingWarning",
      false
    ),
    editorMode: (() => {
      const mode = String(pick(raw, "editorMode", "EditorMode", "html") || "html").toLowerCase();
      return mode === "markdown" ? "markdown" : "html";
    })(),
    validationMessage: pick(raw, "validationMessage", "ValidationMessage", ""),
    returnUrl: pick(raw, "returnUrl", "ReturnUrl", ""),
    cancelUrl: pick(raw, "cancelUrl", "CancelUrl", ""),
    editUrl: pick(raw, "editUrl", "EditUrl", ""),
    deleteUrl: pick(raw, "deleteUrl", "DeleteUrl", ""),
    antiforgeryToken: pick(raw, "antiforgeryToken", "AntiforgeryToken", ""),
    antiforgeryFieldName: pick(
      raw,
      "antiforgeryFieldName",
      "AntiforgeryFieldName",
      "__RequestVerificationToken"
    ),
    dateTimeFormat: pick(raw, "dateTimeFormat", "DateTimeFormat", "m/d/Y H:i"),
    labels: {
      pageTitle: pick(labelsRaw, "pageTitle", "PageTitle", "Edit Post"),
      save: pick(labelsRaw, "save", "Save", "Save"),
      cancel: pick(labelsRaw, "cancel", "Cancel", "Cancel"),
      delete: pick(labelsRaw, "delete", "Delete", "Delete"),
      contentTab: pick(labelsRaw, "contentTab", "ContentTab", "Contents"),
      metadataTab: pick(labelsRaw, "metadataTab", "MetadataTab", "Summary and Metadata"),
      publishingTab: pick(labelsRaw, "publishingTab", "PublishingTab", "Publishing Details"),
      title: pick(labelsRaw, "title", "Title", "Title"),
      contents: pick(labelsRaw, "contents", "Contents", "Contents"),
      summary: pick(labelsRaw, "summary", "Summary", "Summary"),
      language: pick(labelsRaw, "language", "Language", "Language"),
      image: pick(labelsRaw, "image", "Image", "Image"),
      removeImage: pick(labelsRaw, "removeImage", "RemoveImage", "Remove Image"),
      categories: pick(labelsRaw, "categories", "Categories", "Categories"),
      tags: pick(labelsRaw, "tags", "Tags", "Tags"),
      tagsPlaceholder: pick(
        labelsRaw,
        "tagsPlaceholder",
        "TagsPlaceholder",
        "Enter the tags for this post here ..."
      ),
      published: pick(labelsRaw, "published", "Published", "Is Visible"),
      allowComments: pick(labelsRaw, "allowComments", "AllowComments", "Enable Comments"),
      displayCopyright: pick(
        labelsRaw,
        "displayCopyright",
        "DisplayCopyright",
        "Display Copyright"
      ),
      copyright: pick(labelsRaw, "copyright", "Copyright", "Copyright"),
      publishDate: pick(labelsRaw, "publishDate", "PublishDate", "Publish Date"),
      now: pick(labelsRaw, "now", "Now", "Now"),
      timezoneNote: pick(
        labelsRaw,
        "timezoneNote",
        "TimezoneNote",
        "The publish date is based on your profile timezone of: "
      ),
      summaryPrecedingWarning: pick(
        labelsRaw,
        "summaryPrecedingWarning",
        "SummaryPrecedingWarning",
        "Note that the summary precedes the article body."
      ),
      deleteConfirm: pick(labelsRaw, "deleteConfirm", "DeleteConfirm", "Delete this post?"),
      translations: pick(labelsRaw, "translations", "Translations", "Translations"),
    },
  };
}
