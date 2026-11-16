import { FolderOpenIcon, ImageIcon, LinkIcon, UploadIcon } from "lucide-react";
import { isUrl, KEYS } from "platejs";
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

export interface MediaToolbarButtonProps {
  service: DataService;
  blogId: number;
  postId: number;
  onError?: (message: string) => void;
}

export function MediaToolbarButton({
  service,
  blogId,
  postId,
  onError,
}: MediaToolbarButtonProps) {
  const editor = useEditorRef();
  const fileInputRef = React.useRef<HTMLInputElement>(null);
  const [open, setOpen] = React.useState(false);
  const [urlOpen, setUrlOpen] = React.useState(false);
  const [url, setUrl] = React.useState("");
  const [browserOpen, setBrowserOpen] = React.useState(false);
  const [uploading, setUploading] = React.useState(false);

  const insertImage = (imageUrl: string) => {
    if (!isUrl(imageUrl) && !imageUrl.startsWith("/")) {
      onError?.("Invalid image URL");
      return;
    }
    editor.tf.insertNodes({
      type: KEYS.img,
      url: imageUrl,
      children: [{ text: "" }],
    });
  };

  const uploadFile = (file: File) => {
    setUploading(true);
    service.uploadImage(
      blogId,
      postId,
      file,
      (result) => {
        setUploading(false);
        insertImage(result.url);
      },
      (err) => {
        setUploading(false);
        onError?.(err || "Upload failed");
      }
    );
  };

  return (
    <>
      <ToolbarSplitButton
        pressed={open}
        disabled={uploading}
        tooltip="Image"
        onClick={() => fileInputRef.current?.click()}
      >
        <ToolbarSplitButtonPrimary>
          <ImageIcon />
        </ToolbarSplitButtonPrimary>
        <DropdownMenu modal={false} open={open} onOpenChange={setOpen}>
          <DropdownMenuTrigger asChild>
            <ToolbarSplitButtonSecondary />
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start" alignOffset={-32}>
            <DropdownMenuGroup>
              <DropdownMenuItem onSelect={() => fileInputRef.current?.click()}>
                <UploadIcon className="size-4" />
                Upload from computer
              </DropdownMenuItem>
              <DropdownMenuItem
                onSelect={() => {
                  setUrl("");
                  setUrlOpen(true);
                }}
              >
                <LinkIcon className="size-4" />
                Insert via URL
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => setBrowserOpen(true)}>
                <FolderOpenIcon className="size-4" />
                Browse DNN
              </DropdownMenuItem>
            </DropdownMenuGroup>
          </DropdownMenuContent>
        </DropdownMenu>
      </ToolbarSplitButton>

      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        hidden
        onChange={(e) => {
          const file = e.target.files?.[0];
          if (file) uploadFile(file);
          e.target.value = "";
        }}
      />

      {urlOpen && (
        <div className="fixed inset-0 z-[100001] flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-md rounded-lg border border-border bg-background p-4 shadow-lg">
            <h3 className="mb-3 text-sm font-semibold">Insert image via URL</h3>
            <input
              autoFocus
              type="url"
              className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-ring"
              placeholder="https://"
              value={url}
              onChange={(e) => setUrl(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  insertImage(url);
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
                  insertImage(url);
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
        mode={"image" as FileBrowserMode}
        open={browserOpen}
        onClose={() => setBrowserOpen(false)}
        onSelect={(href) => insertImage(href)}
      />
    </>
  );
}
