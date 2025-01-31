
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
using static DotNetNuke.Services.Localization.Localization;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Templating
{
  public class Resources : IPropertyAccess
  {

    private string ResourcesPath { get; set; } = "";
    private string PrimaryResourceFile { get; set; } = "";
    private string SecondaryResourceFile { get; set; } = "";

    #region  Constructors 
    public Resources(string TemplateRelPath)
    {
      PrimaryResourceFile = TemplateRelPath;
      ResourcesPath = TemplateRelPath.Substring(0, TemplateRelPath.LastIndexOf("/"));
      if (!PrimaryResourceFile.ToLower().EndsWith(".resx"))
      {
        PrimaryResourceFile = PrimaryResourceFile + ".resx";
      }
      SecondaryResourceFile = ResourcesPath + "/SharedResources.ascx.resx";
    }
    #endregion

    #region  IPropertyAccess Implementation 
    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.fullyCacheable;
      }
    }

    public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
    {
      string OutputFormat = string.Empty;
      if (string.IsNullOrEmpty(strFormat))
      {
        OutputFormat = "D";
      }
      else
      {
        OutputFormat = strFormat;
      }
      string res = GetString(strPropertyName, PrimaryResourceFile);
      if (string.IsNullOrEmpty(res))
      {
        res = GetString(strPropertyName, SecondaryResourceFile);
      }
      if (res is not null)
      {
        switch (OutputFormat.ToLower() ?? "")
        {
          case "js":
          case "jssafe":
            {
              res = UI.Utilities.ClientAPI.GetSafeJSString(res);
              break;
            }
        }
        return res;
      }
      return Null.NullString;
    }
    #endregion

  }
}