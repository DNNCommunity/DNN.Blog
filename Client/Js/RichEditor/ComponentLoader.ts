import { AppManager } from "./AppManager";
import {
  BlogRichEditorElement,
  defineBlogRichEditor,
} from "./core/BlogRichEditor";
import type { EditorMode } from "./core/types";
import type DataService from "./Service";

const HOST_STYLE_ID = "blog-rich-editor-host-style";

function ensureHostStyles(): void {
  if (document.getElementById(HOST_STYLE_ID)) return;
  const style = document.createElement("style");
  style.id = HOST_STYLE_ID;
  style.textContent = `
    textarea[data-blog-html-editor] {
      position: absolute !important;
      width: 1px !important;
      height: 1px !important;
      padding: 0 !important;
      margin: -1px !important;
      overflow: hidden !important;
      clip: rect(0, 0, 0, 0) !important;
      border: 0 !important;
    }
    .blog-he-mount { display: block; width: 100%; }
  `;
  document.head.appendChild(style);
}

export class ComponentLoader {
  public static load(): void {
    defineBlogRichEditor();
    ensureHostStyles();

    const hosts = document.querySelectorAll<HTMLElement>(
      ".Blog[data-component='html-editor']"
    );

    if (hosts.length > 0) {
      hosts.forEach((el) => this.mountHost(el));
      return;
    }

    document
      .querySelectorAll<HTMLTextAreaElement>("textarea[data-blog-html-editor]")
      .forEach((textarea) => this.mountTextarea(textarea));
  }

  private static resolveModule(moduleId: number) {
    const key = moduleId.toString();
    if (!AppManager.Modules.ContainsKey(key)) {
      console.warn("RichEditor: no DotNetNukeBlog context for module", moduleId);
      return null;
    }
    return AppManager.Modules.Item(key);
  }

  private static mountEditor(
    textarea: HTMLTextAreaElement,
    moduleId: number,
    blogId: number,
    postId: number,
    mode: EditorMode
  ): void {
    if (textarea.dataset.blogHeMounted === "1") return;
    const module = this.resolveModule(moduleId);
    if (!module) return;

    textarea.dataset.blogHeMounted = "1";
    const mount = document.createElement("div");
    mount.className = "blog-he-mount";
    textarea.parentElement?.insertBefore(mount, textarea);

    const editor = document.createElement("blog-rich-editor") as BlogRichEditorElement;
    editor.mode = mode;
    editor.moduleId = moduleId;
    editor.blogId = blogId;
    editor.postId = postId;
    editor.value = textarea.value || "";
    editor.bindTextarea(textarea);
    const service = module.service as DataService;
    if (service?.createUpload) {
      editor.setUploadFn(service.createUpload(blogId, postId));
    }
    mount.appendChild(editor);
  }

  private static resolveMode(el: Element): EditorMode {
    const mode = (
      el.getAttribute("data-editor-mode") ||
      el.dataString?.("editor-mode", "html") ||
      "html"
    ).toLowerCase();
    return mode === "markdown" ? "markdown" : "html";
  }

  private static mountHost(el: HTMLElement): void {
    const moduleId = el.dataInt("moduleid");
    const module = this.resolveModule(moduleId);
    if (!module) return;

    const blogId = el.dataInt("blogid");
    const postId = el.dataInt("postid");
    const targetId = el.dataString("textarea-id", "");
    const textarea = (
      targetId ? document.getElementById(targetId) : el.querySelector("textarea")
    ) as HTMLTextAreaElement | null;

    if (!textarea) {
      console.warn("RichEditor: textarea not found for host", el);
      return;
    }

    this.mountEditor(
      textarea,
      moduleId,
      blogId,
      postId,
      this.resolveMode(el)
    );
  }

  private static mountTextarea(textarea: HTMLTextAreaElement): void {
    if (textarea.dataset.blogHeMounted === "1") return;

    const context =
      document.querySelector(".DotNetNukeBlog") ||
      textarea.closest("[data-moduleid]");
    if (!context) {
      console.warn("RichEditor: missing DotNetNukeBlog context");
      return;
    }

    const moduleId = (context as Element).dataInt("moduleid");
    const blogId =
      (context as Element).dataInt("blogid") ||
      parseInt(
        (document.querySelector('input[name="BlogId"]') as HTMLInputElement)
          ?.value || "0",
        10
      );
    const postId =
      (context as Element).dataInt("postid") ||
      parseInt(
        (document.querySelector('input[name="PostId"]') as HTMLInputElement)
          ?.value || "0",
        10
      );

    const mode = this.resolveMode(textarea);
    this.mountEditor(textarea, moduleId, blogId, postId, mode);
  }
}
