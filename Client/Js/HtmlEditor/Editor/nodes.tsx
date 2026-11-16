import { getLinkAttributes } from "@platejs/link";
import { cva, type VariantProps } from "class-variance-authority";
import type { TImageElement, TLinkElement } from "platejs";
import {
  PlateElement,
  PlateLeaf,
  type PlateElementProps,
  type PlateLeafProps,
} from "platejs/react";
import { cn } from "../lib/utils";

export function ParagraphElement(props: PlateElementProps) {
  return (
    <PlateElement {...props} className={cn("m-0 px-0 py-1")}>
      {props.children}
    </PlateElement>
  );
}

const headingVariants = cva("relative mb-1 font-heading", {
  variants: {
    variant: {
      h1: "mt-[1.4em] pb-1 text-4xl font-bold",
      h2: "mt-[1.2em] pb-px text-2xl font-semibold tracking-tight",
      h3: "mt-[1em] pb-px text-xl font-semibold tracking-tight",
    },
  },
});

export function HeadingElement({
  variant = "h1",
  ...props
}: PlateElementProps & VariantProps<typeof headingVariants>) {
  return (
    <PlateElement
      as={variant!}
      className={headingVariants({ variant })}
      {...props}
    >
      {props.children}
    </PlateElement>
  );
}

export function H1Element(props: PlateElementProps) {
  return <HeadingElement variant="h1" {...props} />;
}

export function H2Element(props: PlateElementProps) {
  return <HeadingElement variant="h2" {...props} />;
}

export function H3Element(props: PlateElementProps) {
  return <HeadingElement variant="h3" {...props} />;
}

export function BlockquoteElement(props: PlateElementProps) {
  return (
    <PlateElement
      as="blockquote"
      className="my-1 border-l-2 border-border pl-6 italic text-muted-foreground"
      {...props}
    />
  );
}

export function HrElement(props: PlateElementProps) {
  return (
    <PlateElement {...props} className="py-2">
      <div contentEditable={false}>
        <hr className="border-border" />
      </div>
      {props.children}
    </PlateElement>
  );
}

export function LinkElement(props: PlateElementProps<TLinkElement>) {
  return (
    <PlateElement
      {...props}
      as="a"
      attributes={{
        ...props.attributes,
        ...getLinkAttributes(props.editor, props.element),
      }}
      className="font-medium text-brand underline decoration-brand underline-offset-4"
    >
      {props.children}
    </PlateElement>
  );
}

export function ImageElement(props: PlateElementProps<TImageElement>) {
  const { url, width } = props.element;
  return (
    <PlateElement {...props} className="my-2">
      <figure className="m-0" contentEditable={false}>
        <img
          src={url}
          alt=""
          className="block h-auto max-w-full rounded-md"
          style={width ? { width: String(width) } : undefined}
          draggable={false}
        />
      </figure>
      {props.children}
    </PlateElement>
  );
}

export function BoldLeaf(props: PlateLeafProps) {
  return (
    <PlateLeaf {...props} as="strong">
      {props.children}
    </PlateLeaf>
  );
}

export function ItalicLeaf(props: PlateLeafProps) {
  return (
    <PlateLeaf {...props} as="em">
      {props.children}
    </PlateLeaf>
  );
}

export function UnderlineLeaf(props: PlateLeafProps) {
  return (
    <PlateLeaf {...props} as="u">
      {props.children}
    </PlateLeaf>
  );
}

export function StrikethroughLeaf(props: PlateLeafProps) {
  return (
    <PlateLeaf {...props} as="s">
      {props.children}
    </PlateLeaf>
  );
}

export function CodeLeaf(props: PlateLeafProps) {
  return (
    <PlateLeaf
      {...props}
      as="code"
      className="rounded bg-muted px-[0.3em] py-[0.1em] font-mono text-[0.9em]"
    >
      {props.children}
    </PlateLeaf>
  );
}
