import * as React from "react";
import { BlogRichEditor } from "../../RichEditor/react/BlogRichEditor";
import type { EditorMode } from "../../RichEditor/core/types";
import { IAppModule } from "../Models/IAppModule";
import { IPostLocaleOption } from "../Models/IPostEditBootstrap";

export interface LocaleHtmlFieldProps {
  module: IAppModule;
  blogId: number;
  postId: number;
  fieldName: string;
  valuesPrefix: string;
  value: string;
  onChange: (value: string) => void;
  values: Record<string, string>;
  onValuesChange: (locale: string, value: string) => void;
  defaultLocale: string;
  defaultFlagUrl?: string;
  defaultLocaleName: string;
  useFullLocalization: boolean;
  otherLocales: IPostLocaleOption[];
  useRichText: boolean;
  editorMode?: EditorMode;
  rows?: number;
  translationsLabel: string;
}

function isDefaultLocale(code: string, defaultLocale: string): boolean {
  return code.toLowerCase() === defaultLocale.toLowerCase();
}

export default function LocaleHtmlField(
  props: LocaleHtmlFieldProps
): React.ReactElement {
  const {
    module,
    blogId,
    postId,
    fieldName,
    valuesPrefix,
    value,
    onChange,
    values,
    onValuesChange,
    defaultLocale,
    defaultFlagUrl,
    defaultLocaleName,
    useFullLocalization,
    otherLocales,
    useRichText,
    editorMode: editorModeProp,
    rows = 12,
    translationsLabel,
  } = props;
  const editorMode: EditorMode = editorModeProp ?? "html";
  const [open, setOpen] = React.useState(false);
  const [selectedLocale, setSelectedLocale] = React.useState(defaultLocale);
  const [bufferEl, setBufferEl] = React.useState<HTMLTextAreaElement | null>(
    null
  );
  const inputId = `blog-locale-${fieldName}`;
  const showLoc = useFullLocalization && otherLocales.length > 0;

  const editorLocales = React.useMemo(() => {
    if (!showLoc) return [];
    return [
      {
        code: defaultLocale,
        name: defaultLocaleName,
      },
      ...otherLocales.map((loc) => ({
        code: loc.Code,
        name: loc.Name,
      })),
    ];
  }, [showLoc, defaultLocale, defaultLocaleName, otherLocales]);

  const activeIsDefault = isDefaultLocale(selectedLocale, defaultLocale);
  const activeValue = activeIsDefault
    ? value
    : values[selectedLocale] || "";

  const uploadFn = React.useMemo(
    () => module.service.createUpload?.(blogId, postId),
    [module.service, blogId, postId]
  );

  const persistActive = React.useCallback(
    (next: string) => {
      if (isDefaultLocale(selectedLocale, defaultLocale)) {
        onChange(next);
      } else {
        onValuesChange(selectedLocale, next);
      }
    },
    [selectedLocale, defaultLocale, onChange, onValuesChange]
  );

  const handleLocaleChange = React.useCallback((nextLocale: string) => {
    setSelectedLocale((prev) => (nextLocale === prev ? prev : nextLocale));
  }, []);

  // Plain (non-rich) path — PostEditApp routes plain Summary to LocaleTextField.
  if (!useRichText) {
    return (
      <div className="blog-locale-field blog-locale-field--html" data-blog-locale-field>
        <div className="blog-locale-field__default">
          <div className="blog-locale-field__control">
            <textarea
              id={inputId}
              name={fieldName}
              value={value}
              onChange={(e) => onChange(e.target.value)}
              rows={rows}
              className="blog-admin__input blog-admin__textarea"
            />
          </div>
          {showLoc && (
            <div className="blog-locale-field__meta">
              {defaultFlagUrl ? (
                <img
                  className="blog-locale-field__flag"
                  src={defaultFlagUrl}
                  alt={defaultLocaleName}
                  title={defaultLocaleName}
                />
              ) : (
                <span
                  className="blog-locale-field__locale-code"
                  title={defaultLocaleName}
                >
                  {defaultLocale}
                </span>
              )}
              <button
                type="button"
                className={`blog-locale-field__toggle${open ? " is-open" : ""}`}
                aria-expanded={open}
                aria-controls={`${inputId}-translations`}
                title={translationsLabel}
                onClick={(e) => {
                  e.preventDefault();
                  setOpen((v) => !v);
                }}
              >
                <i className="fa fa-chevron-down" aria-hidden="true" />
                <span className="sr-only">{translationsLabel}</span>
              </button>
            </div>
          )}
        </div>
        {showLoc && (
          <div
            className="blog-locale-field__translations"
            id={`${inputId}-translations`}
            hidden={!open}
          >
            {otherLocales.map((loc) => (
              <div className="blog-locale-field__row" key={loc.Code}>
                <div className="blog-locale-field__control">
                  <textarea
                    name={`${valuesPrefix}[${loc.Code}]`}
                    value={values[loc.Code] || ""}
                    onChange={(e) => onValuesChange(loc.Code, e.target.value)}
                    rows={rows}
                    className="blog-admin__input blog-admin__textarea"
                    aria-label={loc.Name}
                  />
                </div>
                <div className="blog-locale-field__row-meta">
                  {loc.FlagUrl ? (
                    <img
                      className="blog-locale-field__flag"
                      src={loc.FlagUrl}
                      alt={loc.Name}
                      title={loc.Name}
                    />
                  ) : (
                    <span
                      className="blog-locale-field__locale-code"
                      title={loc.Name}
                    >
                      {loc.Code}
                    </span>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="blog-locale-field blog-locale-field--html" data-blog-locale-field>
      <div className="blog-locale-field__editor">
        {/* Named fields for form post — always present for every locale */}
        <textarea
          name={fieldName}
          value={value}
          readOnly
          tabIndex={-1}
          aria-hidden="true"
          className="blog-locale-field__hidden-value"
        />
        {otherLocales.map((loc) => (
          <textarea
            key={loc.Code}
            name={`${valuesPrefix}[${loc.Code}]`}
            value={values[loc.Code] || ""}
            readOnly
            tabIndex={-1}
            aria-hidden="true"
            className="blog-locale-field__hidden-value"
          />
        ))}
        <textarea
          ref={(node) => setBufferEl(node)}
          id={inputId}
          defaultValue={activeValue}
          rows={rows}
          className="blog-admin__input blog-admin__textarea blog-admin__textarea--html"
          data-blog-html-editor="true"
          data-editor-mode={editorMode}
          aria-hidden="true"
          tabIndex={-1}
        />
        {bufferEl ? (
          <BlogRichEditor
            moduleId={module.moduleId}
            blogId={blogId}
            postId={postId}
            mode={editorMode}
            value={activeValue}
            onChange={persistActive}
            textarea={bufferEl}
            uploadFn={uploadFn}
            locales={editorLocales}
            locale={selectedLocale}
            onLocaleChange={handleLocaleChange}
            localeLabel={translationsLabel}
          />
        ) : null}
      </div>
    </div>
  );
}
