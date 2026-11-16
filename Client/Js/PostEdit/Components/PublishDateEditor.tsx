import * as React from "react";
import flatpickr from "flatpickr";
import type { Instance as FlatpickrInstance } from "flatpickr/dist/types/instance";
import "flatpickr/dist/flatpickr.min.css";
import "./PublishDateEditor.scss";

export interface PublishDateEditorProps {
  publishNow: boolean;
  publishDate: string;
  onPublishNowChange: (value: boolean) => void;
  onPublishDateChange: (value: string) => void;
  timezoneDisplay: string;
  labelNow: string;
  timezoneNote: string;
  dateTimeFormat: string;
}

export default function PublishDateEditor(
  props: PublishDateEditorProps
): React.ReactElement {
  const {
    publishNow,
    publishDate,
    onPublishNowChange,
    onPublishDateChange,
    timezoneDisplay,
    labelNow,
    timezoneNote,
    dateTimeFormat,
  } = props;
  const inputRef = React.useRef<HTMLInputElement>(null);
  const fpRef = React.useRef<FlatpickrInstance | null>(null);

  React.useEffect(() => {
    if (!inputRef.current || publishNow) {
      if (fpRef.current) {
        fpRef.current.destroy();
        fpRef.current = null;
      }
      return;
    }

    const format = dateTimeFormat || "m/d/Y H:i";
    fpRef.current = flatpickr(inputRef.current, {
      enableTime: true,
      allowInput: true,
      dateFormat: format,
      defaultDate: publishDate || undefined,
      onChange: (_dates, dateStr) => {
        onPublishDateChange(dateStr);
      },
      onClose: (_dates, dateStr) => {
        if (dateStr) onPublishDateChange(dateStr);
      },
    });

    return () => {
      if (fpRef.current) {
        fpRef.current.destroy();
        fpRef.current = null;
      }
    };
  }, [publishNow, dateTimeFormat]);

  React.useEffect(() => {
    if (fpRef.current && publishDate && !publishNow) {
      fpRef.current.setDate(publishDate, false);
    }
  }, [publishDate, publishNow]);

  return (
    <div className="blog-admin__control blog-publish-date">
      <label className="blog-publish-date__now">
        {/* Hidden false so unchecked posts as false (MVC checkbox binding). */}
        <input type="hidden" name="PublishNow" value="false" />
        <input
          type="checkbox"
          id="PublishNow"
          name="PublishNow"
          value="true"
          checked={publishNow}
          onChange={(e) => onPublishNowChange(e.target.checked)}
        />
        {labelNow}
      </label>
      {!publishNow && (
        <input
          ref={inputRef}
          type="text"
          id="PublishDate"
          name="PublishDate"
          value={publishDate}
          onChange={(e) => onPublishDateChange(e.target.value)}
          className="blog-admin__input blog-publish-date__input"
        />
      )}
      {publishNow && (
        <input type="hidden" name="PublishDate" value={publishDate} />
      )}
      <p className="blog-publish-date__note">
        <em>
          {timezoneNote}
          {timezoneDisplay}
        </em>
      </p>
    </div>
  );
}
