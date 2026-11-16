import * as React from "react";
import type { HtmlEditorMediaService } from "./HtmlEditor";
import type { FileBrowserMode } from "../Service";
import { DnnFileBrowser } from "../FileBrowser/DnnFileBrowser";
import { canHoldImagesLocally, canUseLocally, registerLocalImage } from "./PendingImages";

export type DnnBrowseRequest = {
  mode: FileBrowserMode;
  onSelect: (url: string) => void;
};

export type MediaContextValue = {
  service: HtmlEditorMediaService;
  blogId: number;
  postId: number;
  onError?: (message: string) => void;
  uploadFile: (file: File) => Promise<{ url: string; fileName: string }>;
  openDnnBrowser: (request: DnnBrowseRequest) => void;
};

const MediaContext = React.createContext<MediaContextValue | null>(null);

export function useMediaContext() {
  return React.useContext(MediaContext);
}

export function useMediaContextRequired() {
  const ctx = useMediaContext();
  if (!ctx) {
    throw new Error("MediaContext is required");
  }
  return ctx;
}

export function MediaProvider({
  service,
  blogId,
  postId,
  onError,
  children,
}: {
  service: HtmlEditorMediaService;
  blogId: number;
  postId: number;
  onError?: (message: string) => void;
  children: React.ReactNode;
}) {
  const [browse, setBrowse] = React.useState<DnnBrowseRequest | null>(null);

  const uploadFile = React.useCallback(
    (file: File) =>
      new Promise<{ url: string; fileName: string }>((resolve, reject) => {
        // preferred: keep the image in the browser until the post is submitted
        if (canUseLocally(file) && canHoldImagesLocally()) {
          resolve(registerLocalImage(file));
          return;
        }
        service.uploadImage(
          blogId,
          postId,
          file,
          (data) => resolve(data),
          (error) => {
            const message = error || "Upload failed";
            onError?.(message);
            reject(new Error(message));
          }
        );
      }),
    [service, blogId, postId, onError]
  );

  const openDnnBrowser = React.useCallback((request: DnnBrowseRequest) => {
    setBrowse(request);
  }, []);

  const value = React.useMemo(
    () => ({
      service,
      blogId,
      postId,
      onError,
      uploadFile,
      openDnnBrowser,
    }),
    [service, blogId, postId, onError, uploadFile, openDnnBrowser]
  );

  return (
    <MediaContext.Provider value={value}>
      {children}
      {browse ? (
        <DnnFileBrowser
          service={service as any}
          mode={browse.mode}
          open
          onSelect={(url) => {
            browse.onSelect(url);
            setBrowse(null);
          }}
          onClose={() => setBrowse(null)}
        />
      ) : null}
    </MediaContext.Provider>
  );
}
