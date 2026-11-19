export interface IPostLocaleOption {
  Code: string;
  Name: string;
  FlagUrl?: string;
}

export interface IPostCategoryNode {
  TermId: number;
  Name: string;
  Depth: number;
  Selected: boolean;
  Children: IPostCategoryNode[];
}

export interface IPostEditLabels {
  pageTitle: string;
  save: string;
  cancel: string;
  delete: string;
  contentTab: string;
  metadataTab: string;
  publishingTab: string;
  title: string;
  contents: string;
  summary: string;
  language: string;
  image: string;
  removeImage: string;
  categories: string;
  tags: string;
  tagsPlaceholder: string;
  published: string;
  allowComments: string;
  displayCopyright: string;
  copyright: string;
  publishDate: string;
  now: string;
  timezoneNote: string;
  summaryPrecedingWarning: string;
  deleteConfirm: string;
  translations: string;
  contentFormat: string;
  contentFormatHelp: string;
  contentFormatHtml: string;
  contentFormatMarkdown: string;
  contentFormatSwitchTitle: string;
  contentFormatSwitchMessage: string;
  contentFormatSwitchConfirm: string;
}

export interface IPostEditBootstrap {
  blogId: number;
  postId: number;
  blogTitle: string;
  isNew: boolean;
  title: string;
  summary: string;
  content: string;
  titles: Record<string, string>;
  summaries: Record<string, string>;
  contents: Record<string, string>;
  published: boolean;
  publishCanChange: boolean;
  publishNow: boolean;
  publishDate: string;
  timezoneDisplay: string;
  allowComments: boolean;
  displayCopyright: boolean;
  copyright: string;
  locale: string;
  showLocale: boolean;
  useFullLocalization: boolean;
  defaultLocale: string;
  locales: IPostLocaleOption[];
  otherLocales: IPostLocaleOption[];
  localeDropdownOptions: IPostLocaleOption[];
  tags: string;
  imageKey: string;
  imageUrl: string;
  showCategories: boolean;
  selectedCategoryIds: number[];
  categories: IPostCategoryNode[];
  summaryIsHtml: boolean;
  showSummaryPrecedingWarning: boolean;
  contentFormat: number;
  editorMode: "html" | "markdown";
  validationMessage: string;
  returnUrl: string;
  cancelUrl: string;
  editUrl: string;
  deleteUrl: string;
  antiforgeryToken: string;
  antiforgeryFieldName: string;
  labels: IPostEditLabels;
  dateTimeFormat: string;
}
