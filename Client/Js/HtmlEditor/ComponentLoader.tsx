import React from "react";
import { createRoot } from "react-dom/client";
import { AppManager } from "./AppManager";
import { HtmlEditor } from "./Editor/HtmlEditor";

export class ComponentLoader {
  public static load(): void {
    const hosts = document.querySelectorAll<HTMLElement>(
      ".Blog[data-component='html-editor']"
    );

    if (hosts.length > 0) {
      hosts.forEach((el) => this.mountHost(el));
      return;
    }

    // Fallback: attach to textareas marked for rich HTML editing
    document
      .querySelectorAll<HTMLTextAreaElement>("textarea[data-blog-html-editor]")
      .forEach((textarea) => this.mountTextarea(textarea));
  }

  private static resolveModule(moduleId: number) {
    const key = moduleId.toString();
    if (!AppManager.Modules.ContainsKey(key)) {
      console.warn("HtmlEditor: no DotNetNukeBlog context for module", moduleId);
      return null;
    }
    return AppManager.Modules.Item(key);
  }

  private static mountHost(el: HTMLElement): void {
    const moduleId = el.dataInt("moduleid");
    const module = this.resolveModule(moduleId);
    if (!module) return;

    const blogId = el.dataInt("blogid");
    const postId = el.dataInt("postid");
    const targetId = el.dataString("textarea-id", "");
    const textarea = (
      targetId
        ? document.getElementById(targetId)
        : el.querySelector("textarea")
    ) as HTMLTextAreaElement | null;

    if (!textarea) {
      console.warn("HtmlEditor: textarea not found for host", el);
      return;
    }

    const mount = document.createElement("div");
    mount.className = "blog-he-mount";
    textarea.parentElement?.insertBefore(mount, textarea);
    textarea.setAttribute("data-blog-html-editor", "true");

    const root = createRoot(mount);
    root.render(
      <HtmlEditor
        module={module}
        textarea={textarea}
        blogId={blogId}
        postId={postId}
      />
    );
  }

  private static mountTextarea(textarea: HTMLTextAreaElement): void {
    if (textarea.dataset.blogHeMounted === "1") return;

    const context =
      document.querySelector(".DotNetNukeBlog") ||
      textarea.closest("[data-moduleid]");
    if (!context) {
      console.warn("HtmlEditor: missing DotNetNukeBlog context");
      return;
    }

    const moduleId = (context as Element).dataInt("moduleid");
    const module = this.resolveModule(moduleId);
    if (!module) return;

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

    textarea.dataset.blogHeMounted = "1";
    const mount = document.createElement("div");
    mount.className = "blog-he-mount";
    textarea.parentElement?.insertBefore(mount, textarea);

    const root = createRoot(mount);
    root.render(
      <HtmlEditor
        module={module}
        textarea={textarea}
        blogId={blogId}
        postId={postId}
      />
    );
  }
}
