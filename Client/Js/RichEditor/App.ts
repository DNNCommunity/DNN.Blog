import { AppManager } from "./AppManager";
import { ComponentLoader } from "./ComponentLoader";
import { defineBlogRichEditor } from "./core/BlogRichEditor";

defineBlogRichEditor();

function boot() {
  AppManager.loadData();
  ComponentLoader.load();
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", boot);
} else {
  boot();
}
