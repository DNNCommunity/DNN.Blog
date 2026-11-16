import { LinkIcon, FolderOpenIcon } from "lucide-react";
import { upsertLink } from "@platejs/link";
import { isUrl } from "platejs";
import { useEditorRef } from "platejs/react";
import * as React from "react";
import type DataService from "../Service";
import type { FileBrowserMode } from "../Service";
import { DnnFileBrowser } from "../FileBrowser/DnnFileBrowser";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import {
  ToolbarSplitButton,
  ToolbarSplitButtonPrimary,
  ToolbarSplitButtonSecondary,
} from "./toolbar";

export interface LinkToolbarButtonProps {
  service: DataService;
  onError?: (message: string) => void;
}

export function LinkToolbarButton({ service, onError }: LinkToolbarButtonProps) {
  const editor = useEditorRef();
  const [open, setOpen] = React.useState(false);
  const [urlOpen, setUrlOpen] = React.useState(false);
  const [url, setUrl] = React.useState("");
  const [browserOpen, setBrowserOpen] = React.useState(false);

  const applyLink = (href: string) => {
    const ok =
      isUrl(href) ||
      href.startsWith("/") ||
      href.startsWith("~") ||
      href.startsWith("FileID=");
    if (!ok) {
      onError?.("Invalid link URL");
      return;
    }
    upsertLink(editor, { url: href });
  };

  return (
    <>
      <ToolbarSplitButton
        pressed={open}
        onClick={() => {
          setUrl("");
          setUrlOpen(true);
        }}
        tooltip="Link"
      >
        <ToolbarSplitButtonPrimary>
          <LinkIcon />
        </ToolbarSplitButtonPrimary>
        <DropdownMenu modal={false} open={open} onOpenChange={setOpen}>
          <DropdownMenuTrigger asChild>
            <ToolbarSplitButtonSecondary />
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start" alignOffset={-32}>
            <DropdownMenuGroup>
              <DropdownMenuItem
                onSelect={() => {
                  setUrl("");
                  setUrlOpen(true);
                }}
              >
                <LinkIcon className="size-4" />
                Insert URL
              </DropdownMenuItem>
              <DropdownMenuItem
                onSelect={() => setBrowserOpen(true)}
              >
                <FolderOpenIcon className="size-4" />
                Browse DNN file
              </DropdownMenuItem>
            </DropdownMenuGroup>
          </DropdownMenuContent>
        </DropdownMenu>
      </ToolbarSplitButton>

      {urlOpen && (
        <div className="fixed inset-0 z-[100001] flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-md rounded-lg border border-border bg-background p-4 shadow-lg">
            <h3 className="mb-3 text-sm font-semibold">Insert link</h3>
            <input
              autoFocus
              type="url"
              className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-ring"
              placeholder="https:// or /path"
              value={url}
              onChange={(e) => setUrl(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  applyLink(url);
                  setUrlOpen(false);
                }
              }}
            />
            <div className="mt-3 flex justify-end gap-2">
              <button
                type="button"
                className="rounded-md border border-input px-3 py-1.5 text-sm hover:bg-muted"
                onClick={() => setUrlOpen(false)}
              >
                Cancel
              </button>
              <button
                type="button"
                className="rounded-md bg-primary px-3 py-1.5 text-sm text-primary-foreground"
                onClick={() => {
                  applyLink(url);
                  setUrlOpen(false);
                }}
              >
                Insert
              </button>
            </div>
          </div>
        </div>
      )}

      <DnnFileBrowser
        service={service}
        mode={"file" as FileBrowserMode}
        open={browserOpen}
        onClose={() => setBrowserOpen(false)}
        onSelect={(href) => applyLink(href)}
      />
    </>
  );
}
