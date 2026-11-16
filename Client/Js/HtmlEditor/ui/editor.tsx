import { cva, type VariantProps } from "class-variance-authority";
import type { PlateContentProps } from "platejs/react";
import { PlateContainer, PlateContent } from "platejs/react";
import * as React from "react";
import { cn } from "../lib/utils";

const editorContainerVariants = cva(
  "relative w-full cursor-text select-text overflow-y-auto caret-foreground focus-visible:outline-none",
  {
    defaultVariants: {
      variant: "default",
    },
    variants: {
      variant: {
        default: "h-full",
        demo: "h-[520px]",
      },
    },
  }
);

export function EditorContainer({
  className,
  variant,
  ...props
}: React.ComponentProps<"div"> & VariantProps<typeof editorContainerVariants>) {
  return (
    <PlateContainer
      className={cn(editorContainerVariants({ variant }), className)}
      {...props}
    />
  );
}

const editorVariants = cva(
  cn(
    "group/editor relative w-full cursor-text select-text overflow-x-hidden break-words whitespace-pre-wrap",
    "rounded-md focus-visible:outline-none",
    "[&_strong]:font-bold"
  ),
  {
    defaultVariants: {
      variant: "default",
    },
    variants: {
      variant: {
        default:
          "size-full px-10 pt-6 pb-24 text-base sm:px-16 sm:px-[max(4rem,calc(50%-22rem))]",
        demo: "size-full px-10 pt-6 pb-24 text-base sm:px-16",
        fullWidth: "size-full px-8 pt-4 pb-16 text-base",
      },
    },
  }
);

export type EditorProps = PlateContentProps &
  VariantProps<typeof editorVariants>;

export function Editor({
  className,
  variant = "fullWidth",
  ...props
}: EditorProps) {
  return (
    <PlateContent
      className={cn(editorVariants({ variant }), className)}
      disableDefaultStyles
      {...props}
    />
  );
}
