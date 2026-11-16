import type { BlogRichEditorElement } from "./core/BlogRichEditor";
import type { DetailedHTMLProps, HTMLAttributes } from "react";

declare module "*.css" {
  const css: string;
  export default css;
}

declare module "react" {
  namespace JSX {
    interface IntrinsicElements {
      "blog-rich-editor": DetailedHTMLProps<
        HTMLAttributes<BlogRichEditorElement> & {
          mode?: string;
          "blog-id"?: string;
          "post-id"?: string;
          "module-id"?: string;
          "upload-url"?: string;
          value?: string;
          disabled?: boolean;
        },
        BlogRichEditorElement
      >;
    }
  }
}

export {};
