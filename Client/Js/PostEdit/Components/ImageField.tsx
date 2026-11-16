import * as React from "react";

export interface ImageFieldProps {
  imageUrl: string;
  removeImage: boolean;
  onRemoveChange: (remove: boolean) => void;
  onFileChange: (file: File | null) => void;
  labelRemove: string;
}

export default function ImageField(
  props: ImageFieldProps
): React.ReactElement {
  const { imageUrl, removeImage, onRemoveChange, onFileChange, labelRemove } =
    props;
  const inputRef = React.useRef<HTMLInputElement>(null);

  return (
    <div className="blog-admin__control">
      {imageUrl ? (
        <div className="blog-post-image">
          <img
            src={imageUrl}
            alt=""
            className="blog-post-image__preview"
          />
          <label className="blog-post-image__remove">
            <input
              type="checkbox"
              name="RemoveImage"
              value="true"
              checked={removeImage}
              onChange={(e) => onRemoveChange(e.target.checked)}
            />
            {labelRemove}
          </label>
        </div>
      ) : null}
      <input
        ref={inputRef}
        type="file"
        id="ImageFile"
        name="ImageFile"
        accept=".jpg,.jpeg,.png,.gif,.bmp"
        className="blog-admin__file"
        onChange={(e) => {
          const file = e.target.files && e.target.files[0] ? e.target.files[0] : null;
          onFileChange(file);
        }}
      />
    </div>
  );
}
