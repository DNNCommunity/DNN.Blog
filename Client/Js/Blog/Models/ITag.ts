export interface ITagLocale {
  Code: string;
  Name: string;
}

export interface ITagItem {
  TermId: number;
  Name: string;
  Localizations: Record<string, string>;
}

export interface ITagList {
  Items: ITagItem[];
  Page: number;
  PageSize: number;
  Total: number;
  DefaultLocale: string;
  Locales: ITagLocale[];
}

export interface ITagListQuery {
  page: number;
  pageSize: number;
  search?: string;
}
