export interface IContextSecurity {
  UserId: number;
  CanView: boolean;
  CanEdit: boolean;
  IsAdmin: boolean;
  CanViewComments?: boolean;
  CanAddComment?: boolean;
  CanApproveComment?: boolean;
  LoggedIn?: boolean;
}
