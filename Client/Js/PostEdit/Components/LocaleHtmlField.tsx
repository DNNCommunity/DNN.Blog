import * as React from "react";
import { HtmlEditor } from "../../HtmlEditor/Editor/HtmlEditor";
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
  rows?: number;
  translationsLabel: string;
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
    rows = 12,
    translationsLabel,
  } = props;
  const [open, setOpen] = React.useState(false);
  const [textareaEl, setTextareaEl] =
    React.useState<HTMLTextAreaElement | null>(null);
  const inputId = `blog-locale-${fieldName}`;
  const showLoc = useFullLocalization && otherLocales.length > 0;

  return (
    <div className="blog-locale-field blog-locale-field--html" data-blog-locale-field>
      <div className="blog-locale-field__default blog-locale-field__default--stack">
        {showLoc && (
          <div className="blog-locale-field__toolbar">
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
            <span className="blog-locale-field__label">{defaultLocaleName}</span>
            <button
              type="button"
              className={`blog-locale-field__toggle${open ? " is-open" : ""}`}
              aria-expanded={open}
              aria-controls={`${inputId}-translations`}
              title={translationsLabel}
              onClick={() => setOpen((v) => !v)}
            >
              <i className="fa fa-chevron-down" aria-hidden="true" />
              <span className="sr-only">{translationsLabel}</span>
            </button>
          </div>
        )}
        <div className="blog-locale-field__editor">
          {useRichText ? (
            <>
              <textarea
                ref={(node) => setTextareaEl(node)}
                id={inputId}
                name={fieldName}
                defaultValue={value}
                rows={rows}
                className="blog-admin__input blog-admin__textarea blog-admin__textarea--html"
                data-blog-html-editor="true"
                aria-hidden="true"
                tabIndex={-1}
              />
              {textareaEl ? (
                <HtmlEditor
                  module={module}
                  blogId={blogId}
                  postId={postId}
                  value={value}
                  onChange={onChange}
                  textarea={textareaEl}
                />
              ) : null}
            </>
          ) : (
            <textarea
              id={inputId}
              name={fieldName}
              value={value}
              onChange={(e) => onChange(e.target.value)}
              rows={rows}
              className="blog-admin__input blog-admin__textarea"
            />
          )}
        </div>
      </div>
      {showLoc && (
        <div
          className="blog-locale-field__translations"
          id={`${inputId}-translations`}
          hidden={!open}
        >
          {otherLocales.map((loc) => (
            <div
              className="blog-locale-field__row blog-locale-field__row--stack"
              key={loc.Code}
            >
              <div className="blog-locale-field__row-meta">
                {loc.FlagUrl ? (
                  <img
                    className="blog-locale-field__flag"
                    src={loc.FlagUrl}
                    alt={loc.Name}
                    title={loc.Name}
                  />
                ) : null}
                <span className="blog-locale-field__label">{loc.Name}</span>
              </div>
              <div className="blog-locale-field__editor">
                <textarea
                  name={`${valuesPrefix}[${loc.Code}]`}
                  value={values[loc.Code] || ""}
                  onChange={(e) => onValuesChange(loc.Code, e.target.value)}
                  rows={rows}
                  className="blog-admin__input blog-admin__textarea"
                  aria-label={loc.Name}
                />
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
