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
  } = props;
  const [open, setOpen] = React.useState(false);
  const inputId = `blog-locale-${fieldName}`;
  const showLoc = useFullLocalization && otherLocales.length > 0;

  return (
    <div className="blog-locale-field" data-blog-locale-field>
      <div className="blog-locale-field__default">
        <div className="blog-locale-field__control">
          <input
            type="text"
            id={inputId}
            name={fieldName}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            className="blog-admin__input"
            required={required}
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
              onClick={() => setOpen((v) => !v)}
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
              <div className="blog-locale-field__control">
                <input
                  type="text"
                  name={`${valuesPrefix}[${loc.Code}]`}
                  value={values[loc.Code] || ""}
                  onChange={(e) => onValuesChange(loc.Code, e.target.value)}
                  className="blog-admin__input"
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
