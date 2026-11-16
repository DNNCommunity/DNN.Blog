import { AppManager } from "./AppManager";
import { ComponentLoader } from "./ComponentLoader";

function boot() {
  AppManager.loadData();
  ComponentLoader.load();
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", boot);
} else {
  boot();
}
