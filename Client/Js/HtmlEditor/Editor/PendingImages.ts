/**
 * Images added in the editor stay in the browser (blob: URLs) until the post form is submitted.
 * On submit the files are attached to the form as "PendingImages", named after their pending id,
 * and the server matches them with the data-blog-pending attribute emitted by the serializer.
 */
type Pending = { id: string; file: File };

const registry = new Map<string, Pending>();
const FIELD_NAME = "PendingImages";
const MAX_BYTES = 20 * 1024 * 1024;

function extensionOf(file: File): string {
  const m = /\.[a-z0-9]+$/i.exec(file.name);
  if (m) return m[0].toLowerCase();
  if (file.type === "image/png") return ".png";
  if (file.type === "image/gif") return ".gif";
  return ".jpg";
}

function newId(): string {
  const c = (window as any).crypto;
  if (c && c.randomUUID) return c.randomUUID();
  return "p" + Date.now().toString(36) + Math.random().toString(36).slice(2, 10);
}

/** True when the browser can hand locally held files to a native form submit. */
export function canHoldImagesLocally(): boolean {
  try {
    return (
      typeof DataTransfer !== "undefined" &&
      new DataTransfer().files !== undefined &&
      typeof URL !== "undefined" &&
      typeof URL.createObjectURL === "function" &&
      !!document.querySelector("form textarea[name='Content']")
    );
  } catch {
    return false;
  }
}

export function canUseLocally(file: File): boolean {
  return /^image\/(jpeg|png|gif)$/i.test(file.type) && file.size <= MAX_BYTES;
}

export function registerLocalImage(file: File): { url: string; fileName: string } {
  const id = newId();
  const url = URL.createObjectURL(file);
  registry.set(url, { id, file });
  ensureSubmitHook();
  return { url, fileName: file.name };
}

export function getPendingId(url: string): string | undefined {
  return registry.get(url)?.id;
}

function attach(form: HTMLFormElement) {
  let input = form.querySelector<HTMLInputElement>(`input[type=file][name='${FIELD_NAME}']`);
  if (!input) {
    input = document.createElement("input");
    input.type = "file";
    input.name = FIELD_NAME;
    input.multiple = true;
    input.style.display = "none";
    form.appendChild(input);
  }
  const dt = new DataTransfer();
  registry.forEach(({ id, file }) => {
    dt.items.add(new File([file], id + extensionOf(file), { type: file.type }));
  });
  input.files = dt.files;
}

let hooked = false;
function ensureSubmitHook() {
  if (hooked) return;
  hooked = true;
  document.addEventListener(
    "submit",
    (e) => {
      const form = e.target as HTMLFormElement;
      if (form && form.querySelector && form.querySelector("textarea[name='Content']") && registry.size > 0) {
        attach(form);
      }
    },
    true
  );
}
