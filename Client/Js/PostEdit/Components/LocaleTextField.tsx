import * as React from "react";
import { IPostLocaleOption } from "../Models/IPostEditBootstrap";

export interface LocaleTextFieldProps {
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
  required?: boolean;
  translationsLabel: string;
  multiline?: boolean;
  rows?: number;
}

function LocaleFlag(props: {
  flagUrl?: string;
  code: string;
  name: string;
}): React.ReactElement {
  const { flagUrl, code, name } = props;
  if (flagUrl) {
    return (
      <img
        className="blog-locale-field__flag"
        src={flagUrl}
        alt={name}
        title={name}
      />
    );
  }
  return (
    <span className="blog-locale-field__locale-code" title={name}>
      {code}
    </span>
  );
}

export default function LocaleTextField(
  props: LocaleTextFieldProps
): React.ReactElement {
  const {
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
    required,
    translationsLabel,
    multiline,
    rows = 8,
  } = props;
  const [open, setOpen] = React.useState(false);
  const inputId = `blog-locale-${fieldName}`;
  const showLoc = useFullLocalization && otherLocales.length > 0;

  const defaultControl = multiline ? (
    <textarea
      id={inputId}
      name={fieldName}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      rows={rows}
      className="blog-admin__input blog-admin__textarea"
      required={required}
    />
  ) : (
    <input
      type="text"
      id={inputId}
      name={fieldName}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className="blog-admin__input"
      required={required}
    />
  );

  return (
    <div
      className={`blog-locale-field${multiline ? " blog-locale-field--multiline" : ""}`}
      data-blog-locale-field
    >
      <div className="blog-locale-field__default">
        <div className="blog-locale-field__control">{defaultControl}</div>
        {showLoc && (
          <div className="blog-locale-field__meta">
            <LocaleFlag
              flagUrl={defaultFlagUrl}
              code={defaultLocale}
              name={defaultLocaleName}
            />
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
                {multiline ? (
                  <textarea
                    name={`${valuesPrefix}[${loc.Code}]`}
                    value={values[loc.Code] || ""}
                    onChange={(e) => onValuesChange(loc.Code, e.target.value)}
                    rows={rows}
                    className="blog-admin__input blog-admin__textarea"
                    aria-label={loc.Name}
                  />
                ) : (
                  <input
                    type="text"
                    name={`${valuesPrefix}[${loc.Code}]`}
                    value={values[loc.Code] || ""}
                    onChange={(e) => onValuesChange(loc.Code, e.target.value)}
                    className="blog-admin__input"
                    aria-label={loc.Name}
                  />
                )}
              </div>
              <div className="blog-locale-field__row-meta">
                <LocaleFlag
                  flagUrl={loc.FlagUrl}
                  code={loc.Code}
                  name={loc.Name}
                />
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
