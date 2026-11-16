import * as React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import ConfirmDialog from "../../Common/ConfirmDialog";
import { IApiToken } from "../Models/IApiToken";
import { IAppModule } from "../Models/IAppModule";
import CreateTokenDialog from "./CreateTokenDialog";
import TokenSecretDialog from "./TokenSecretDialog";

export interface ApiTokensManagerLabels {
  addLabel: string;
  nameLabel: string;
  createTitle: string;
  createLabel: string;
  cancelLabel: string;
  tokenCreatedTitle: string;
  tokenCreatedWarning: string;
  closeLabel: string;
  copyLabel: string;
  revokeLabel: string;
  revokeConfirm: string;
  nameHeader: string;
  createdHeader: string;
  expiresHeader: string;
  lastUsedHeader: string;
  statusHeader: string;
  actionsHeader: string;
  statusActive: string;
  statusRevoked: string;
  statusExpired: string;
  emptyLabel: string;
}

export interface ApiTokensManagerProps {
  module: IAppModule;
  labels: ApiTokensManagerLabels;
}

const PAGE_SIZE = 25;

function formatDate(value: string | null | undefined, locale: string): string {
  if (!value) return "—";
  const d = new Date(value);
  if (Number.isNaN(d.getTime())) return value;
  try {
    return d.toLocaleString(locale || undefined);
  } catch {
    return d.toLocaleString();
  }
}

function statusLabel(token: IApiToken, labels: ApiTokensManagerLabels): string {
  if (token.isRevoked) return labels.statusRevoked;
  if (token.isExpired) return labels.statusExpired;
  if (token.status === "Revoked") return labels.statusRevoked;
  if (token.status === "Expired") return labels.statusExpired;
  return labels.statusActive;
}

export default function ApiTokensManager(
  props: ApiTokensManagerProps
): React.ReactElement {
  const { module, labels } = props;
  const [items, setItems] = useState<IApiToken[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [createdToken, setCreatedToken] = useState<string | null>(null);
  const [pendingRevoke, setPendingRevoke] = useState<IApiToken | null>(null);
  const requestId = useRef(0);

  const load = useCallback(() => {
    const id = ++requestId.current;
    setLoading(true);
    setError(null);
    module.service.listTokens(
      0,
      PAGE_SIZE,
      (data) => {
        if (id !== requestId.current) return;
        setItems(data.items || []);
        setLoading(false);
      },
      (err) => {
        if (id !== requestId.current) return;
        setError(err || "Failed to load API tokens.");
        setItems([]);
        setLoading(false);
      }
    );
  }, [module.service]);

  useEffect(() => {
    load();
  }, [load]);

  const onCreate = (name: string) => {
    setBusy(true);
    setError(null);
    module.service.createToken(
      name,
      (result) => {
        setBusy(false);
        setShowCreate(false);
        setCreatedToken(result.token || "");
      },
      (err) => {
        setBusy(false);
        setError(err || "Failed to create token.");
      }
    );
  };

  const onSecretClose = () => {
    setCreatedToken(null);
    load();
  };

  const onRevokeConfirm = () => {
    if (!pendingRevoke) return;
    setBusy(true);
    setError(null);
    module.service.revokeToken(
      pendingRevoke.apiTokenId,
      () => {
        setBusy(false);
        setPendingRevoke(null);
        load();
      },
      (err) => {
        setBusy(false);
        setError(err || "Failed to revoke token.");
      }
    );
  };

  return (
    <div className="blog-api-tokens">
      <div className="blog-manage__section-actions">
        <button
          type="button"
          className="blog-manage__btn blog-manage__btn--secondary"
          onClick={() => setShowCreate(true)}
          disabled={busy}
        >
          {labels.addLabel}
        </button>
      </div>

      {error ? (
        <p className="blog-manage__status" role="alert">
          {error}
        </p>
      ) : null}

      <div className="blog-manage__table-wrap">
        <table className="blog-manage__table">
          <thead>
            <tr>
              <th>{labels.nameHeader}</th>
              <th>{labels.createdHeader}</th>
              <th>{labels.expiresHeader}</th>
              <th>{labels.lastUsedHeader}</th>
              <th>{labels.statusHeader}</th>
              <th>{labels.actionsHeader}</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={6}>…</td>
              </tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={6}>{labels.emptyLabel}</td>
              </tr>
            ) : (
              items.map((token) => {
                const canRevoke = !token.isRevoked && !token.isExpired;
                return (
                  <tr key={token.apiTokenId}>
                    <td>{token.tokenName}</td>
                    <td>{formatDate(token.createdOnDate, module.locale)}</td>
                    <td>{formatDate(token.expiresOn, module.locale)}</td>
                    <td>
                      {formatDate(token.lastUsedOnDate, module.locale)}
                    </td>
                    <td>{statusLabel(token, labels)}</td>
                    <td className="blog-manage__actions-cell">
                      {canRevoke ? (
                        <button
                          type="button"
                          className="blog-manage__btn blog-manage__btn--secondary"
                          disabled={busy}
                          onClick={() => setPendingRevoke(token)}
                        >
                          {labels.revokeLabel}
                        </button>
                      ) : null}
                    </td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>

      <CreateTokenDialog
        open={showCreate}
        title={labels.createTitle}
        nameLabel={labels.nameLabel}
        createLabel={labels.createLabel}
        cancelLabel={labels.cancelLabel}
        busy={busy}
        onCreate={onCreate}
        onCancel={() => {
          if (!busy) setShowCreate(false);
        }}
      />

      <TokenSecretDialog
        open={createdToken != null}
        title={labels.tokenCreatedTitle}
        warning={labels.tokenCreatedWarning}
        token={createdToken || ""}
        copyLabel={labels.copyLabel}
        closeLabel={labels.closeLabel}
        onClose={onSecretClose}
      />

      <ConfirmDialog
        open={pendingRevoke != null}
        title={labels.revokeLabel}
        message={labels.revokeConfirm}
        confirmLabel={labels.revokeLabel}
        cancelLabel={labels.cancelLabel}
        busy={busy}
        onConfirm={onRevokeConfirm}
        onCancel={() => {
          if (!busy) setPendingRevoke(null);
        }}
      />
    </div>
  );
}
