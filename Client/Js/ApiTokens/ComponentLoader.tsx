import React from "react";
import { createRoot } from "react-dom/client";
import { AppManager } from "./AppManager";
import ApiTokensManager, {
  ApiTokensManagerLabels,
} from "./Components/ApiTokensManager";

export class ComponentLoader {
  public static load(): void {
    document
      .querySelectorAll<HTMLElement>(
        ".BlogApiTokens[data-component='api-tokens-manager']"
      )
      .forEach((el) => this.mount(el));
  }

  private static mount(el: HTMLElement): void {
    const moduleId = el.dataInt("moduleid");
    const key = moduleId.toString();
    if (!AppManager.Modules.ContainsKey(key)) {
      console.warn("ApiTokens: no DotNetNukeBlog context for module", moduleId);
      return;
    }
    const module = AppManager.Modules.Item(key);
    const labels: ApiTokensManagerLabels = {
      addLabel: el.dataString("add-label", "Add Token"),
      nameLabel: el.dataString("name-label", "Token name"),
      createTitle: el.dataString("create-title", "Create API Token"),
      createLabel: el.dataString("create-label", "Create"),
      cancelLabel: el.dataString("cancel-label", "Cancel"),
      tokenCreatedTitle: el.dataString(
        "token-created-title",
        "Your API Token"
      ),
      tokenCreatedWarning: el.dataString(
        "token-created-warning",
        "Copy this token now. It cannot be shown again."
      ),
      closeLabel: el.dataString("close-label", "Close"),
      copyLabel: el.dataString("copy-label", "Copy"),
      revokeLabel: el.dataString("revoke-label", "Revoke"),
      revokeConfirm: el.dataString(
        "revoke-confirm",
        "Revoke this API token? It will stop working immediately."
      ),
      nameHeader: el.dataString("name-header", "Name"),
      createdHeader: el.dataString("created-header", "Created"),
      expiresHeader: el.dataString("expires-header", "Expires"),
      lastUsedHeader: el.dataString("last-used-header", "Last Used"),
      statusHeader: el.dataString("status-header", "Status"),
      actionsHeader: el.dataString("actions-header", "Actions"),
      statusActive: el.dataString("status-active", "Active"),
      statusRevoked: el.dataString("status-revoked", "Revoked"),
      statusExpired: el.dataString("status-expired", "Expired"),
      emptyLabel: el.dataString("empty-label", "No API tokens yet."),
    };

    const root = createRoot(el);
    root.render(<ApiTokensManager module={module} labels={labels} />);
  }
}
