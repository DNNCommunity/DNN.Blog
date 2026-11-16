import {
  BlogRichEditorElement,
  defineBlogRichEditor,
} from "../core/BlogRichEditor";
import type { EditorMode, UploadFn } from "../core/types";

defineBlogRichEditor();

/**
 * Thin Vue 3 wrapper. Usage:
 *   import { BlogRichEditorVue } from '.../vue/BlogRichEditor'
 *   app.component('BlogRichEditor', BlogRichEditorVue)
 */
export const BlogRichEditorVue = {
  name: "BlogRichEditor",
  props: {
    mode: { type: String, default: "html" },
    modelValue: { type: String, default: "" },
    blogId: { type: Number, required: true },
    postId: { type: Number, required: true },
    moduleId: { type: Number, required: true },
    uploadUrl: { type: String, default: "" },
    uploadFn: { type: Function, default: null },
    disabled: { type: Boolean, default: false },
    placeholder: { type: String, default: "" },
  },
  emits: ["update:modelValue", "change"],
  mounted(this: any) {
    const el = this.$refs.el as BlogRichEditorElement;
    this.syncProps(el);
    el.addEventListener("change", this.onChange);
  },
  beforeUnmount(this: any) {
    const el = this.$refs.el as BlogRichEditorElement;
    el?.removeEventListener("change", this.onChange);
    el?.destroy();
  },
  watch: {
    modelValue(this: any, v: string) {
      const el = this.$refs.el as BlogRichEditorElement;
      if (el && el.getValue() !== v) el.setValue(v);
    },
    mode(this: any) {
      this.syncProps(this.$refs.el);
    },
  },
  methods: {
    syncProps(this: any, el: BlogRichEditorElement) {
      if (!el) return;
      el.mode = (this.mode === "markdown" ? "markdown" : "html") as EditorMode;
      el.blogId = this.blogId;
      el.postId = this.postId;
      el.moduleId = this.moduleId;
      el.uploadUrl = this.uploadUrl || "";
      el.disabled = this.disabled;
      if (this.placeholder) el.setAttribute("placeholder", this.placeholder);
      if (this.uploadFn) el.setUploadFn(this.uploadFn as UploadFn);
      el.setValue(this.modelValue || "");
    },
    onChange(this: any, e: Event) {
      const value =
        (e as CustomEvent).detail?.value ??
        (this.$refs.el as BlogRichEditorElement).getValue();
      this.$emit("update:modelValue", value);
      this.$emit("change", value);
    },
  },
  template: `<blog-rich-editor ref="el"></blog-rich-editor>`,
};

export default BlogRichEditorVue;
