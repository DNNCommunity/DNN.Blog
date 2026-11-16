export type EditorMode = "html" | "markdown";

export interface EditorLocaleOption {
  code: string;
  name: string;
}

export interface UploadResult {
  url: string;
  fileName: string;
}

export type UploadFn = (file: File) => Promise<UploadResult>;

export interface EditorEngine {
  getValue(): string;
  setValue(value: string): void;
  focus(): void;
  destroy(): void;
}

export interface EngineHost {
  mount: HTMLElement;
  initialValue: string;
  placeholder?: string;
  disabled?: boolean;
  upload: UploadFn;
  onChange: (value: string) => void;
  onError: (message: string) => void;
}
