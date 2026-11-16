import type { UploadFn } from "../core/types";

export interface IContextSecurity {
  UserId?: number;
  CanView?: boolean;
  CanEdit?: boolean;
  IsAdmin?: boolean;
}

export interface RichEditorMediaService {
  uploadFile: UploadFn;
  moduleId: number;
}

export interface IAppModule {
  moduleId: number;
  tabId: number;
  locale: string;
  resources: any;
  common: any;
  security: IContextSecurity;
  service: RichEditorMediaService;
}

export class AppModule implements IAppModule {
  constructor(
    public moduleId: number,
    public tabId: number,
    public locale: string,
    public resources: any,
    public common: any,
    public security: IContextSecurity,
    public service: RichEditorMediaService
  ) {}
}
