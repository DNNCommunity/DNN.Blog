export interface IApiToken {
  apiTokenId: number;
  tokenName: string;
  createdOnDate: string;
  expiresOn: string;
  lastUsedOnDate: string | null;
  isRevoked: boolean;
  isExpired: boolean;
  keys: string;
  status: string;
}

export interface IApiTokenList {
  items: IApiToken[];
  page: number;
  pageSize: number;
  total: number;
}

export interface ICreateApiTokenResult {
  token: string;
}
