import * as React from "react";
import * as ReactDOM from "react-dom";
import { AppManager } from "./AppManager";
import { ComponentLoader } from "./ComponentLoader";

document.addEventListener("DOMContentLoaded", () => {
  AppManager.loadData();
  ComponentLoader.load();
});
