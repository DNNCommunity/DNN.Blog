using System;
using System.Collections.Specialized;
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

using DotNetNuke.Services.Tokens;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Templating
{
  public class CustomParameters : IPropertyAccess
  {

    #region  Private Members 
    private NameValueCollection @params { get; set; }
    #endregion

    #region  Constructors 
    public CustomParameters(params string[] customParameters)
    {
      @params = new NameValueCollection();
      foreach (string sParameter in customParameters)
        @params.Add(Strings.Left(sParameter.ToLower(), sParameter.IndexOf('=')), Strings.Mid(sParameter, sParameter.IndexOf('=') + 2));
    }
    #endregion

    #region  IPropertyAccess 
    public CacheLevel Cacheability
    {
      get
      {
        return CacheLevel.notCacheable;
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
      if (@params[strPropertyName.ToLower()] is null)
        return DotNetNuke.Common.Utilities.Null.NullString;
      string value = @params[strPropertyName.ToLower()];
      try
      {
        if (Information.IsNumeric(value))
        {
          return Conversions.ToDouble(value).ToString(OutputFormat, formatProvider);
        }
        if (Information.IsDate(value))
        {
          return Conversions.ToDate(value).ToString(OutputFormat, formatProvider);
        }
        return Common.Globals.FormatBoolean(Conversions.ToBoolean(value), strFormat);
      }
      catch (Exception ex)
      {
        return value;
      }
    }
    #endregion

  }
}