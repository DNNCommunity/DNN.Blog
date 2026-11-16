import type { EditorEngine, EditorMode, UploadFn } from "./types";
import { createDnnUploadFn } from "./upload";
import { createHtmlEngine } from "../html/editor";
import { createMarkdownEngine } from "../markdown/editor";
import styles from "../styles/editorCss";

const TAG = "blog-rich-editor";

export class BlogRichEditorElement extends HTMLElement {
  static get observedAttributes() {
    return ["mode", "value", "blog-id", "post-id", "module-id", "upload-url", "disabled", "placeholder"];
  }

  #shadow: ShadowRoot;
  #engine: EditorEngine | null = null;
  #status: HTMLElement | null = null;
  #mount: HTMLElement | null = null;
  #textarea: HTMLTextAreaElement | null = null;
  #uploadFn: UploadFn | null = null;
  #syncing = false;
  #submitHandler: (() => void) | null = null;

  constructor() {
    super();
    this.#shadow = this.attachShadow({ mode: "open" });
    const style = document.createElement("style");
    style.textContent = styles;
    this.#shadow.appendChild(style);

    const root = document.createElement("div");
    root.className = "bre";
    this.#mount = document.createElement("div");
    this.#status = document.createElement("div");
    this.#status.className = "bre__status";
    this.#status.setAttribute("role", "status");
    root.appendChild(this.#mount);
    root.appendChild(this.#status);
    this.#shadow.appendChild(root);
  }

  get mode(): EditorMode {
    const m = (this.getAttribute("mode") || "html").toLowerCase();
    return m === "markdown" ? "markdown" : "html";
  }
  set mode(v: EditorMode) {
    this.setAttribute("mode", v);
  }

  get value(): string {
    return this.#engine?.getValue() ?? this.getAttribute("value") ?? "";
  }
  set value(v: string) {
    const next = v ?? "";
    const current = this.#engine?.getValue() ?? this.getAttribute("value") ?? "";
    if (current === next && this.getAttribute("value") === next) return;
    this.#syncing = true;
    try {
      this.setAttribute("value", next);
      if (this.#engine && this.#engine.getValue() !== next) {
        this.#engine.setValue(next);
      }
    } finally {
      this.#syncing = false;
    }
  }

  get blogId(): number {
    return parseInt(this.getAttribute("blog-id") || "0", 10) || 0;
  }
  set blogId(v: number) {
    this.setAttribute("blog-id", String(v));
  }

  get postId(): number {
    return parseInt(this.getAttribute("post-id") || "0", 10) || 0;
  }
  set postId(v: number) {
    this.setAttribute("post-id", String(v));
  }

  get moduleId(): number {
    return parseInt(this.getAttribute("module-id") || "0", 10) || 0;
  }
  set moduleId(v: number) {
    this.setAttribute("module-id", String(v));
  }

  get uploadUrl(): string {
    return this.getAttribute("upload-url") || "";
  }
  set uploadUrl(v: string) {
    if (v) this.setAttribute("upload-url", v);
    else this.removeAttribute("upload-url");
  }

  get disabled(): boolean {
    return this.hasAttribute("disabled");
  }
  set disabled(v: boolean) {
    if (v) this.setAttribute("disabled", "");
    else this.removeAttribute("disabled");
  }

  /** Optional external upload function (overrides DNN default). */
  setUploadFn(fn: UploadFn | null): void {
    this.#uploadFn = fn;
  }

  bindTextarea(textarea: HTMLTextAreaElement | null): void {
    this.#unbindForm();
    this.#textarea = textarea;
    if (!textarea) return;
    textarea.setAttribute("data-blog-html-editor", "true");
    textarea.setAttribute("aria-hidden", "true");
    textarea.tabIndex = -1;
    if (!this.hasAttribute("value") && textarea.value) {
      this.setAttribute("value", textarea.value);
    }
    const form = textarea.form || textarea.closest("form");
    if (form) {
      this.#submitHandler = () => this.flushToTextarea();
      form.addEventListener("submit", this.#submitHandler);
    }
  }

  getValue(): string {
    return this.value;
  }

  setValue(v: string): void {
    this.value = v;
  }

  focus(): void {
    this.#engine?.focus();
  }

  destroy(): void {
    this.#teardownEngine();
    this.#unbindForm();
  }

  connectedCallback(): void {
    this.#bootEngine();
  }

  disconnectedCallback(): void {
    this.destroy();
  }

  attributeChangedCallback(name: string, oldVal: string | null, newVal: string | null): void {
    if (oldVal === newVal) return;
    if (name === "mode") {
      this.#bootEngine();
      return;
    }
    if (name === "value" && this.#engine && !this.#syncing) {
      const next = newVal || "";
      if (this.#engine.getValue() !== next) {
        this.#engine.setValue(next);
      }
    }
    if (
      name === "blog-id" ||
      name === "post-id" ||
      name === "module-id" ||
      name === "upload-url"
    ) {
      // upload fn rebuilt lazily on next upload via #resolveUpload
    }
  }

  #unbindForm(): void {
    if (this.#submitHandler && this.#textarea) {
      const form = this.#textarea.form || this.#textarea.closest("form");
      form?.removeEventListener("submit", this.#submitHandler);
    }
    this.#submitHandler = null;
  }

  #resolveUpload(): UploadFn {
    if (this.#uploadFn) return this.#uploadFn;
    return createDnnUploadFn({
      moduleId: this.moduleId,
      blogId: this.blogId,
      postId: this.postId,
      uploadUrl: this.uploadUrl || undefined,
    });
  }

  #setStatus(message: string, isError = false): void {
    if (!this.#status) return;
    this.#status.textContent = message;
    this.#status.classList.toggle("is-error", isError);
  }

  #emit(value: string): void {
    this.#syncing = true;
    try {
      this.setAttribute("value", value);
      this.flushToTextarea();
      this.dispatchEvent(
        new CustomEvent("change", {
          bubbles: true,
          composed: true,
          detail: { value, mode: this.mode },
        })
      );
      this.dispatchEvent(
        new CustomEvent("input", {
          bubbles: true,
          composed: true,
          detail: { value, mode: this.mode },
        })
      );
    } finally {
      this.#syncing = false;
    }
  }

  flushToTextarea(): void {
    if (!this.#textarea) return;
    const value = this.#engine?.getValue() ?? this.getAttribute("value") ?? "";
    if (this.#textarea.value !== value) {
      this.#textarea.value = value;
      this.#textarea.dispatchEvent(new Event("input", { bubbles: true }));
      this.#textarea.dispatchEvent(new Event("change", { bubbles: true }));
    }
  }

  #teardownEngine(): void {
    this.#engine?.destroy();
    this.#engine = null;
    if (this.#mount) this.#mount.innerHTML = "";
  }

  #bootEngine(): void {
    if (!this.isConnected || !this.#mount) return;
    this.#teardownEngine();
    this.#setStatus("");

    const host = {
      mount: this.#mount,
      initialValue: this.getAttribute("value") || this.#textarea?.value || "",
      placeholder: this.getAttribute("placeholder") || undefined,
      disabled: this.disabled,
      upload: (file: File) => this.#resolveUpload()(file),
      onChange: (value: string) => this.#emit(value),
      onError: (message: string) => this.#setStatus(message, true),
    };

    this.#engine =
      this.mode === "markdown"
        ? createMarkdownEngine(host)
        : createHtmlEngine(host);

    this.flushToTextarea();
  }
}

export function defineBlogRichEditor(): void {
  if (!customElements.get(TAG)) {
    customElements.define(TAG, BlogRichEditorElement);
  }
}

declare global {
  interface HTMLElementTagNameMap {
    "blog-rich-editor": BlogRichEditorElement;
  }
}
