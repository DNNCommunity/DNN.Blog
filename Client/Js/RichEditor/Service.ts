import type { UploadFn, UploadResult } from "./core/types";
import { createDnnUploadFn } from "./core/upload";
import type { RichEditorMediaService } from "./Models/IAppModule";

export default class DataService implements RichEditorMediaService {
  public moduleId: number;
  public uploadFile: UploadFn;

  constructor(moduleId: number) {
    this.moduleId = moduleId;
    // blog/post filled per-call via wrapper in mount
    this.uploadFile = async () => {
      throw new Error("uploadFile requires blog/post context");
    };
  }

  public createUpload(blogId: number, postId: number): UploadFn {
    return createDnnUploadFn({
      moduleId: this.moduleId,
      blogId,
      postId,
    });
  }

  public uploadImage(
    blogId: number,
    postId: number,
    file: File,
    success: (data: UploadResult) => void,
    fail?: (error: string) => void
  ): void {
    this.createUpload(blogId, postId)(file).then(success).catch((e) => fail?.(e.message));
  }
}
