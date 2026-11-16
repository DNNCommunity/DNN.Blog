import {
  Heading1Icon,
  Heading2Icon,
  Heading3Icon,
  PilcrowIcon,
  QuoteIcon,
} from "lucide-react";
import { KEYS } from "platejs";
import { useEditorRef, useEditorSelector } from "platejs/react";
import * as React from "react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import { ToolbarButton } from "./toolbar";

const items = [
  { icon: <PilcrowIcon />, label: "Text", value: KEYS.p },
  { icon: <Heading1Icon />, label: "Heading 1", value: KEYS.h1 },
  { icon: <Heading2Icon />, label: "Heading 2", value: KEYS.h2 },
  { icon: <Heading3Icon />, label: "Heading 3", value: KEYS.h3 },
  { icon: <QuoteIcon />, label: "Quote", value: KEYS.blockquote },
];

export function TurnIntoToolbarButton() {
  const editor = useEditorRef();
  const [open, setOpen] = React.useState(false);

  const value = useEditorSelector((ed) => {
    const block = ed.api.block();
    return (block?.[0] as { type?: string } | undefined)?.type || KEYS.p;
  }, []);

  const selected = items.find((i) => i.value === value) || items[0];

  return (
    <DropdownMenu open={open} onOpenChange={setOpen} modal={false}>
      <DropdownMenuTrigger asChild>
        <ToolbarButton
          isDropdown
          pressed={open}
          tooltip="Turn into"
          className="min-w-[120px]"
        >
          {selected.label}
        </ToolbarButton>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="start" className="min-w-[180px]">
        <DropdownMenuRadioGroup
          value={value}
          onValueChange={(type) => {
            editor.tf.toggle.block({ type });
            setOpen(false);
          }}
        >
          {items.map((item) => (
            <DropdownMenuRadioItem
              key={item.value}
              value={item.value}
              className="min-w-[180px] gap-2"
            >
              {item.icon}
              {item.label}
            </DropdownMenuRadioItem>
          ))}
        </DropdownMenuRadioGroup>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
