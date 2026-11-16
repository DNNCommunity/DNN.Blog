export interface ICategoryLocale {
  Code: string;
  Name: string;
}

export interface ICategoryNode {
  TermId: number;
  Name: string;
  ParentTermId: number;
  ViewOrder: number;
  Localizations: Record<string, string>;
  Children: ICategoryNode[];
}

export interface ICategoryTree {
  Items: ICategoryNode[];
  DefaultLocale: string;
  Locales: ICategoryLocale[];
  IsMultilingual: boolean;
}
