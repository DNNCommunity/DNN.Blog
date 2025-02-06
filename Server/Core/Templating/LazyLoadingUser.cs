// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Services.Tokens;
using System.Web;

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  public class LazyLoadingUser : IPropertyAccess
  {

    #region  Properties 
    public int PortalId { get; set; } = -1;
    public int UserId { get; set; } = -1;
    public string Username { get; set; } = "";

    private UserInfo _user;
    public UserInfo User
    {
      get
      {
        if (_user is null)
        {
          if (UserId > -1)
          {
            _user = UserController.GetUserById(PortalId, UserId);
          }
          else if (!string.IsNullOrEmpty(Username))
          {
            _user = UserController.GetCachedUser(PortalId, Username);
          }
          else
          {
            _user = new UserInfo();
          }
        }
        return _user;
      }
      set
      {
        _user = value;
      }
    }

    private PostAuthor _postAuthor = null;
    #endregion

    #region  Constructors 
    public LazyLoadingUser(int portalId, int userId)
    {
      PortalId = portalId;
      UserId = userId;
    }

    public LazyLoadingUser(int portalId, int userId, string userName)
    {
      PortalId = portalId;
      UserId = userId;
      Username = userName;
    }

    public LazyLoadingUser(UserInfo user)
    {
      User = user;
      PortalId = user.PortalID;
      UserId = user.UserID;
    }

    public LazyLoadingUser(PostAuthor user)
    {
      User = user;
      PortalId = user.PortalID;
      UserId = user.UserID;
      _postAuthor = user;
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      switch (strPropertyName.ToLower() ?? "")
      {
        case "profilepic":
          {
            if (int.TryParse(strFormat, out int res1))
            {
              return DotNetNuke.Common.Globals.ResolveUrl(string.Format("~/DnnImageHandler.ashx?mode=profilepic&userId={0}&w={1}&h={1}", UserId, strFormat));
            }
            else
            {
              return DotNetNuke.Common.Globals.ResolveUrl(string.Format("~/DnnImageHandler.ashx?mode=profilepic&userId={0}&w={1}&h={1}", UserId, 50));
            }
          }
        case "profileurl":
          {
            return DotNetNuke.Common.Globals.UserProfileURL(UserId);
          }
      }
      string res = "";
      if (_postAuthor != null)
      {
        res = _postAuthor.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, ref PropertyNotFound);
      }
      else
      {
        res = User.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, AccessLevel, ref PropertyNotFound);
      }
      if (PropertyNotFound)
      {
        res = HttpUtility.HtmlDecode(User.Profile.GetPropertyValue(strPropertyName));
      }
      if (!string.IsNullOrEmpty(res))
      {
        PropertyNotFound = false;
        return res;
      }
      PropertyNotFound = true;
      return Null.NullString;
    }

    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }
    #endregion

  }
}