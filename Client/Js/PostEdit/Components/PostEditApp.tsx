import * as React from "react";
import ConfirmDialog from "../../Common/ConfirmDialog";
import { IAppModule } from "../Models/IAppModule";
import {
  IPostEditBootstrap,
  IPostLocaleOption,
} from "../Models/IPostEditBootstrap";
import CategoryPicker from "./CategoryPicker";
import ImageField from "./ImageField";
import LocaleHtmlField from "./LocaleHtmlField";
import LocaleTextField from "./LocaleTextField";
import PublishDateEditor from "./PublishDateEditor";
import TagsAutocomplete from "./TagsAutocomplete";
import "./PostEditApp.scss";

export interface PostEditAppProps {
  module: IAppModule;
  bootstrap: IPostEditBootstrap;
}

type TabId = "content" | "metadata" | "publishing";

function parseTags(raw: string): string[] {
  if (!raw) return [];
  return raw
    .split(",")
    .map((t) => t.trim())
    .filter((t, i, arr) => t.length > 0 && arr.indexOf(t) === i);
}

function defaultLocaleOption(
  locales: IPostLocaleOption[],
  code: string
): IPostLocaleOption {
  return (
    locales.find(
      (l) => l.Code.toLowerCase() === (code || "").toLowerCase()
    ) || { Code: code, Name: code, FlagUrl: "" }
  );
}

export default function PostEditApp(
  props: PostEditAppProps
): React.ReactElement {
  const { module, bootstrap: b } = props;
  const L = b.labels;
  const defaultLoc = defaultLocaleOption(b.locales, b.defaultLocale);

  const [tab, setTab] = React.useState<TabId>("content");
  const [deleteOpen, setDeleteOpen] = React.useState(false);
  const [title, setTitle] = React.useState(b.title || "");
  const [titles, setTitles] = React.useState<Record<string, string>>({
    ...(b.titles || {}),
  });
  const [content, setContent] = React.useState(b.content || "");
  const [contents, setContents] = React.useState<Record<string, string>>({
    ...(b.contents || {}),
  });
  const [summary, setSummary] = React.useState(b.summary || "");
  const [summaries, setSummaries] = React.useState<Record<string, string>>({
    ...(b.summaries || {}),
  });
  const [locale, setLocale] = React.useState(b.locale || "");
  const [removeImage, setRemoveImage] = React.useState(false);
  const [selectedCategoryIds, setSelectedCategoryIds] = React.useState(
    () => new Set<number>(b.selectedCategoryIds || [])
  );
  const [tags, setTags] = React.useState(() => parseTags(b.tags || ""));
  const [published, setPublished] = React.useState(!!b.published);
  const [allowComments, setAllowComments] = React.useState(!!b.allowComments);
  const [displayCopyright, setDisplayCopyright] = React.useState(
    !!b.displayCopyright
  );
  const [copyright, setCopyright] = React.useState(b.copyright || "");
  const [publishNow, setPublishNow] = React.useState(!!b.publishNow);
  const [publishDate, setPublishDate] = React.useState(b.publishDate || "");

  const tabs: { id: TabId; label: string }[] = [
    { id: "content", label: L.contentTab },
    { id: "metadata", label: L.metadataTab },
    { id: "publishing", label: L.publishingTab },
  ];

  return (
    <>
      <header className="blog-admin__header">
        <h1 className="blog-admin__title">{L.pageTitle}</h1>
        <p className="blog-admin__subtitle">{b.blogTitle}</p>
      </header>

      {b.validationMessage ? (
        <div className="blog-admin__message blog-admin__message--error" role="alert">
          {b.validationMessage}
        </div>
      ) : null}

      <form
        action={b.editUrl}
        method="post"
        encType="multipart/form-data"
        className="blog-admin__form"
      >
        <input
          type="hidden"
          name={b.antiforgeryFieldName || "__RequestVerificationToken"}
          value={b.antiforgeryToken}
        />
        <input type="hidden" name="BlogId" value={b.blogId} />
        <input type="hidden" name="PostId" value={b.postId} />
        <input type="hidden" name="IsNew" value={b.isNew ? "true" : "false"} />
        <input type="hidden" name="ImageKey" value={b.imageKey || ""} />
        <input type="hidden" name="ReturnUrl" value={b.returnUrl || ""} />

        <div className="blog-admin__tabs">
          <div className="blog-admin__tablist" role="tablist">
            {tabs.map((t) => (
              <button
                key={t.id}
                type="button"
                className={`blog-admin__tab${tab === t.id ? " is-active" : ""}`}
                role="tab"
                aria-selected={tab === t.id}
                onClick={() => setTab(t.id)}
              >
                {t.label}
              </button>
            ))}
          </div>

          <div
            className={`blog-admin__tabpanel${tab === "content" ? " is-active" : ""}`}
            role="tabpanel"
            hidden={tab !== "content"}
          >
            <section className="blog-admin__section">
              <div className="blog-admin__fields">
                <div className="blog-admin__field blog-admin__field--top blog-admin__field--full">
                  <label className="blog-admin__label" htmlFor="blog-locale-Title">
                    {L.title}
                  </label>
                  <div className="blog-admin__control">
                    <LocaleTextField
                      fieldName="Title"
                      valuesPrefix="Titles"
                      value={title}
                      onChange={setTitle}
                      values={titles}
                      onValuesChange={(code, v) =>
                        setTitles((prev) => ({ ...prev, [code]: v }))
                      }
                      defaultLocale={b.defaultLocale}
                      defaultFlagUrl={defaultLoc.FlagUrl}
                      defaultLocaleName={defaultLoc.Name}
                      useFullLocalization={b.useFullLocalization}
                      otherLocales={b.otherLocales || []}
                      required
                      translationsLabel={L.translations}
                    />
                  </div>
                </div>

                {b.showSummaryPrecedingWarning ? (
                  <div className="blog-admin__field blog-admin__field--full">
                    <div className="blog-admin__message blog-admin__message--warning">
                      {L.summaryPrecedingWarning}
                    </div>
                  </div>
                ) : null}

                <div className="blog-admin__field blog-admin__field--top blog-admin__field--full">
                  <label className="blog-admin__label">{L.contents}</label>
                  <div className="blog-admin__control">
                    <LocaleHtmlField
                      module={module}
                      blogId={b.blogId}
                      postId={b.postId}
                      fieldName="Content"
                      valuesPrefix="Contents"
                      value={content}
                      onChange={setContent}
                      values={contents}
                      onValuesChange={(code, v) =>
                        setContents((prev) => ({ ...prev, [code]: v }))
                      }
                      defaultLocale={b.defaultLocale}
                      defaultFlagUrl={defaultLoc.FlagUrl}
                      defaultLocaleName={defaultLoc.Name}
                      useFullLocalization={b.useFullLocalization}
                      otherLocales={b.otherLocales || []}
                      useRichText
                      editorMode={b.editorMode}
                      rows={14}
                      translationsLabel={L.translations}
                    />
                  </div>
                </div>
              </div>
            </section>
          </div>

          <div
            className={`blog-admin__tabpanel${tab === "metadata" ? " is-active" : ""}`}
            role="tabpanel"
            hidden={tab !== "metadata"}
          >
            <section className="blog-admin__section">
              <div className="blog-admin__fields">
                <div className="blog-admin__field blog-admin__field--top blog-admin__field--full">
                  <label className="blog-admin__label">{L.summary}</label>
                  <div className="blog-admin__control">
                    <LocaleHtmlField
                      module={module}
                      blogId={b.blogId}
                      postId={b.postId}
                      fieldName="Summary"
                      valuesPrefix="Summaries"
                      value={summary}
                      onChange={setSummary}
                      values={summaries}
                      onValuesChange={(code, v) =>
                        setSummaries((prev) => ({ ...prev, [code]: v }))
                      }
                      defaultLocale={b.defaultLocale}
                      defaultFlagUrl={defaultLoc.FlagUrl}
                      defaultLocaleName={defaultLoc.Name}
                      useFullLocalization={b.useFullLocalization}
                      otherLocales={b.otherLocales || []}
                      useRichText={b.summaryIsHtml}
                      editorMode={b.editorMode}
                      rows={8}
                      translationsLabel={L.translations}
                    />
                  </div>
                </div>

                {b.showLocale ? (
                  <div className="blog-admin__field">
                    <label className="blog-admin__label" htmlFor="Locale">
                      {L.language}
                    </label>
                    <div className="blog-admin__control">
                      <select
                        id="Locale"
                        name="Locale"
                        className="blog-admin__input"
                        value={locale}
                        onChange={(e) => setLocale(e.target.value)}
                      >
                        {(b.localeDropdownOptions || []).map((opt) => (
                          <option key={opt.Code || "__default"} value={opt.Code}>
                            {opt.Name}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                ) : null}

                <div className="blog-admin__field blog-admin__field--top">
                  <label className="blog-admin__label" htmlFor="ImageFile">
                    {L.image}
                  </label>
                  <ImageField
                    imageUrl={b.imageUrl || ""}
                    removeImage={removeImage}
                    onRemoveChange={setRemoveImage}
                    onFileChange={() => {}}
                    labelRemove={L.removeImage}
                  />
                </div>

                {b.showCategories ? (
                  <div className="blog-admin__field blog-admin__field--top">
                    <label className="blog-admin__label">{L.categories}</label>
                    <div className="blog-admin__control">
                      <CategoryPicker
                        nodes={b.categories || []}
                        selectedIds={selectedCategoryIds}
                        onChange={setSelectedCategoryIds}
                      />
                    </div>
                  </div>
                ) : null}

                <div className="blog-admin__field">
                  <label className="blog-admin__label">{L.tags}</label>
                  <div className="blog-admin__control">
                    <TagsAutocomplete
                      module={module}
                      tags={tags}
                      onChange={setTags}
                      placeholder={L.tagsPlaceholder}
                    />
                  </div>
                </div>
              </div>
            </section>
          </div>

          <div
            className={`blog-admin__tabpanel${
              tab === "publishing" ? " is-active" : ""
            }`}
            role="tabpanel"
            hidden={tab !== "publishing"}
          >
            <section className="blog-admin__section">
              <div className="blog-admin__fields">
                <div className="blog-admin__field">
                  <label className="blog-admin__label" htmlFor="Published">
                    {L.published}
                  </label>
                  <div className="blog-admin__control">
                    <input
                      type="checkbox"
                      id="Published"
                      name="Published"
                      value="true"
                      checked={published}
                      disabled={!b.publishCanChange}
                      onChange={(e) => setPublished(e.target.checked)}
                    />
                    {!b.publishCanChange ? (
                      <input type="hidden" name="Published" value="false" />
                    ) : null}
                  </div>
                </div>

                <div className="blog-admin__field">
                  <label className="blog-admin__label" htmlFor="AllowComments">
                    {L.allowComments}
                  </label>
                  <div className="blog-admin__control">
                    <input
                      type="checkbox"
                      id="AllowComments"
                      name="AllowComments"
                      value="true"
                      checked={allowComments}
                      onChange={(e) => setAllowComments(e.target.checked)}
                    />
                  </div>
                </div>

                <div className="blog-admin__field">
                  <label className="blog-admin__label" htmlFor="DisplayCopyright">
                    {L.displayCopyright}
                  </label>
                  <div className="blog-admin__control">
                    <input
                      type="checkbox"
                      id="DisplayCopyright"
                      name="DisplayCopyright"
                      value="true"
                      checked={displayCopyright}
                      onChange={(e) => setDisplayCopyright(e.target.checked)}
                    />
                  </div>
                </div>

                {displayCopyright ? (
                  <div className="blog-admin__field">
                    <label className="blog-admin__label" htmlFor="Copyright">
                      {L.copyright}
                    </label>
                    <div className="blog-admin__control">
                      <input
                        type="text"
                        id="Copyright"
                        name="Copyright"
                        value={copyright}
                        onChange={(e) => setCopyright(e.target.value)}
                        className="blog-admin__input"
                      />
                    </div>
                  </div>
                ) : (
                  <input type="hidden" name="Copyright" value={copyright} />
                )}

                <div className="blog-admin__field blog-admin__field--top">
                  <label className="blog-admin__label">{L.publishDate}</label>
                  <PublishDateEditor
                    publishNow={publishNow}
                    publishDate={publishDate}
                    onPublishNowChange={setPublishNow}
                    onPublishDateChange={setPublishDate}
                    timezoneDisplay={b.timezoneDisplay || ""}
                    labelNow={L.now}
                    timezoneNote={L.timezoneNote}
                    dateTimeFormat={b.dateTimeFormat || "m/d/Y H:i"}
                  />
                </div>
              </div>
            </section>
          </div>
        </div>

        <div className="blog-admin__actions">
          {!b.isNew ? (
            <div className="blog-admin__actions-secondary">
              <button
                type="button"
                className="blog-admin__btn blog-admin__btn--danger"
                onClick={() => setDeleteOpen(true)}
              >
                {L.delete}
              </button>
            </div>
          ) : null}
          <button type="submit" className="blog-admin__btn blog-admin__btn--primary">
            {L.save}
          </button>
          <a
            href={b.cancelUrl || "#"}
            className="blog-admin__btn blog-admin__btn--secondary"
          >
            {L.cancel}
          </a>
        </div>
      </form>

      {!b.isNew ? (
        <form id="blog-post-delete" action={b.deleteUrl} method="post" hidden>
          <input
            type="hidden"
            name={b.antiforgeryFieldName || "__RequestVerificationToken"}
            value={b.antiforgeryToken}
          />
          <input type="hidden" name="Blog" value={b.blogId} />
          <input type="hidden" name="Post" value={b.postId} />
        </form>
      ) : null}

      <ConfirmDialog
        open={deleteOpen}
        title={L.delete}
        message={L.deleteConfirm}
        confirmLabel={L.delete}
        cancelLabel={L.cancel}
        onCancel={() => setDeleteOpen(false)}
        onConfirm={() => {
          setDeleteOpen(false);
          const form = document.getElementById(
            "blog-post-delete"
          ) as HTMLFormElement | null;
          form?.submit();
        }}
      />
    </>
  );
}
