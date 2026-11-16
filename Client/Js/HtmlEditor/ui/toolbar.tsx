import * as ToolbarPrimitive from "@radix-ui/react-toolbar";
import { cva, type VariantProps } from "class-variance-authority";
import { ChevronDown } from "lucide-react";
import * as React from "react";
import { cn } from "../lib/utils";
import { Separator } from "./separator";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "./tooltip";

export function Toolbar({
  className,
  ...props
}: React.ComponentProps<typeof ToolbarPrimitive.Root>) {
  return (
    <ToolbarPrimitive.Root
      className={cn("relative flex select-none items-center", className)}
      {...props}
    />
  );
}

export function ToolbarToggleGroup({
  className,
  ...props
}: React.ComponentProps<typeof ToolbarPrimitive.ToolbarToggleGroup>) {
  return (
    <ToolbarPrimitive.ToolbarToggleGroup
      className={cn("flex items-center", className)}
      {...props}
    />
  );
}

export function ToolbarToggleItem({
  className,
  size = "sm",
  variant,
  ...props
}: React.ComponentProps<typeof ToolbarPrimitive.ToggleItem> &
  VariantProps<typeof toolbarButtonVariants>) {
  return (
    <ToolbarPrimitive.ToggleItem
      className={cn(toolbarButtonVariants({ size, variant }), className)}
      {...props}
    />
  );
}

const toolbarButtonVariants = cva(
  "inline-flex cursor-pointer items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium outline-none transition-colors hover:bg-muted hover:text-muted-foreground focus-visible:ring-2 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50 aria-checked:bg-accent aria-checked:text-accent-foreground [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
  {
    defaultVariants: {
      size: "default",
      variant: "default",
    },
    variants: {
      size: {
        default: "h-9 min-w-9 px-2",
        lg: "h-10 min-w-10 px-2.5",
        sm: "h-8 min-w-8 px-1.5",
      },
      variant: {
        default: "bg-transparent",
        outline:
          "border border-input bg-transparent shadow-sm hover:bg-accent hover:text-accent-foreground",
      },
    },
  }
);

type ToolbarButtonProps = {
  isDropdown?: boolean;
  pressed?: boolean;
  tooltip?: React.ReactNode;
} & Omit<
  React.ComponentPropsWithoutRef<typeof ToolbarToggleItem>,
  "asChild" | "value"
> &
  VariantProps<typeof toolbarButtonVariants>;

export function ToolbarButton({
  children,
  className,
  isDropdown,
  pressed,
  size = "sm",
  tooltip,
  variant,
  ...props
}: ToolbarButtonProps) {
  const button =
    typeof pressed === "boolean" ? (
      <ToolbarToggleGroup disabled={props.disabled} type="single" value="single">
        <ToolbarToggleItem
          className={cn(
            toolbarButtonVariants({ size, variant }),
            isDropdown && "justify-between gap-1 pr-1",
            className
          )}
          value={pressed ? "single" : ""}
          {...props}
        >
          {isDropdown ? (
            <>
              <div className="flex flex-1 items-center gap-2 whitespace-nowrap">
                {children}
              </div>
              <ChevronDown className="size-3.5 text-muted-foreground" />
            </>
          ) : (
            children
          )}
        </ToolbarToggleItem>
      </ToolbarToggleGroup>
    ) : (
      <ToolbarPrimitive.Button
        className={cn(
          toolbarButtonVariants({ size, variant }),
          isDropdown && "pr-1",
          className
        )}
        {...props}
      >
        {children}
      </ToolbarPrimitive.Button>
    );

  if (!tooltip) return button;

  return (
    <TooltipProvider delayDuration={300}>
      <Tooltip>
        <TooltipTrigger asChild>{button}</TooltipTrigger>
        <TooltipContent>{tooltip}</TooltipContent>
      </Tooltip>
    </TooltipProvider>
  );
}

export function ToolbarSplitButton({
  className,
  ...props
}: React.ComponentPropsWithoutRef<typeof ToolbarButton>) {
  return (
    <ToolbarButton
      className={cn("group flex gap-0 px-0 hover:bg-transparent", className)}
      {...props}
    />
  );
}

export function ToolbarSplitButtonPrimary({
  children,
  className,
  size = "sm",
  variant,
  ...props
}: React.ComponentPropsWithoutRef<"span"> &
  VariantProps<typeof toolbarButtonVariants>) {
  return (
    <span
      className={cn(
        toolbarButtonVariants({ size, variant }),
        "rounded-r-none group-data-[pressed=true]:bg-accent group-data-[pressed=true]:text-accent-foreground",
        className
      )}
      {...props}
    >
      {children}
    </span>
  );
}

export function ToolbarSplitButtonSecondary({
  className,
  ...props
}: React.ComponentPropsWithoutRef<"span">) {
  return (
    <span
      className={cn(
        "inline-flex h-8 w-5 items-center justify-center rounded-r-md hover:bg-muted",
        "group-data-[pressed=true]:bg-accent group-data-[pressed=true]:text-accent-foreground",
        className
      )}
      onClick={(e) => e.stopPropagation()}
      role="button"
      {...props}
    >
      <ChevronDown className="size-3.5 text-muted-foreground" />
    </span>
  );
}

export function ToolbarGroup({
  children,
  className,
}: React.ComponentProps<"div">) {
  return (
    <div className={cn("group/toolbar-group relative flex", className)}>
      <div className="flex items-center">{children}</div>
      <div className="mx-1.5 py-0.5 group-last/toolbar-group:hidden">
        <Separator orientation="vertical" />
      </div>
    </div>
  );
}
