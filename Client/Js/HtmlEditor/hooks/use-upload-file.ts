import * as React from "react";
import { useMediaContext } from "@/Editor/MediaContext";
import { toast } from "@/lib/toast";

type UploadedFile = {
  name: string;
  size: number;
  type: string;
  url: string;
};

/**
 * Uploads through the DNN Media service when MediaProvider is present.
 * Falls back to a data URL only when no provider is mounted (tests / isolation).
 */
export function useUploadFile() {
  const media = useMediaContext();
  const [isUploading, setIsUploading] = React.useState(false);
  const [progress, setProgress] = React.useState(0);
  const [uploadedFile, setUploadedFile] = React.useState<UploadedFile>();
  const [uploadingFile, setUploadingFile] = React.useState<File>();

  const uploadFile = React.useCallback(
    (file: File) => {
      setIsUploading(true);
      setUploadingFile(file);
      setProgress(10);
      setUploadedFile(undefined);

      if (media) {
        media
          .uploadFile(file)
          .then((data) => {
            setProgress(100);
            setUploadedFile({
              name: data.fileName || file.name,
              size: file.size,
              type: file.type,
              url: data.url,
            });
          })
          .catch((error: Error) => {
            toast.error(error.message || "Upload failed");
          })
          .finally(() => {
            setIsUploading(false);
          });
        return;
      }

      const reader = new FileReader();
      reader.onprogress = (event) => {
        if (event.lengthComputable) {
          setProgress(Math.round((event.loaded / event.total) * 100));
        }
      };
      reader.onload = () => {
        setProgress(100);
        setUploadedFile({
          name: file.name,
          size: file.size,
          type: file.type,
          url: String(reader.result || ""),
        });
        setIsUploading(false);
      };
      reader.onerror = () => {
        setIsUploading(false);
        toast.error(reader.error?.message || "File read failed");
      };
      reader.readAsDataURL(file);
    },
    [media]
  );

  return {
    isUploading,
    progress,
    uploadedFile,
    uploadFile,
    uploadingFile,
  };
}
