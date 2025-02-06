using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static DotNetNuke.Services.Localization.Localization;
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2010 by DotNetNuke Corp. 
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

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  /// <summary>
 /// BaseCustomTokenReplace  allows to add multiple sources implementing <see cref="IPropertyAccess">IPropertyAccess</see>
 /// </summary>
 /// <remarks></remarks>
  public abstract class BaseCustomTokenReplace : BaseTokenReplace
  {

    #region  Private Fields 
    private Scope _AccessLevel;
    private DotNetNuke.Entities.Users.UserInfo _AccessingUser;
    private bool _debugMessages;
    #endregion

    #region Protected Properties

    /// <summary>
  /// Gets/sets the current Access Level controlling access to critical user settings
  /// </summary>
  /// <value>A TokenAccessLevel as defined above</value>
    protected Scope CurrentAccessLevel
    {
      get
      {
        return _AccessLevel;
      }
      set
      {

        _AccessLevel = value;
      }
    }

    protected Dictionary<string, IPropertyAccess> PropertySource = new Dictionary<string, IPropertyAccess>();


    #endregion

    #region Public Properties

    /// <summary>
  /// Gets/sets the user object representing the currently accessing user (permission)
  /// </summary>
  /// <value>UserInfo oject</value>
    public DotNetNuke.Entities.Users.UserInfo AccessingUser
    {
      get
      {
        return _AccessingUser;
      }
      set
      {
        _AccessingUser = value;
      }
    }

    /// <summary>
  /// If DebugMessages are enabled, unknown Tokens are replaced with Error Messages 
  /// </summary>
  /// <value></value>
  /// <returns></returns>
  /// <remarks></remarks>
    public bool DebugMessages
    {
      get
      {
        return _debugMessages;
      }
      set
      {
        _debugMessages = value;
      }
    }

    #endregion

    #region Protected Methods

    protected override string replacedTokenValue(string strObjectName, string strPropertyName, string strFormat)
    {
      bool PropertyNotFound = false;
      string result = string.Empty;
      if (PropertySource.ContainsKey(strObjectName.ToLower()))
      {
        result = PropertySource[strObjectName.ToLower()].GetProperty(strPropertyName, strFormat, FormatProvider, AccessingUser, CurrentAccessLevel, ref PropertyNotFound);
      }
      else if (DebugMessages)
      {
        string message = GetString("TokenReplaceUnknownObject", SharedResourceFile, FormatProvider.ToString());
        if (string.IsNullOrEmpty(message))
          message = "Error accessing [{0}:{1}], {0} is an unknown datasource";
        result = string.Format(message, strObjectName, strPropertyName);
      }
      if (DebugMessages & PropertyNotFound)
      {
        string message;
        if ((result ?? "") == (PropertyAccess.ContentLocked ?? ""))
        {
          message = GetString("TokenReplaceRestrictedProperty", GlobalResourceFile, FormatProvider.ToString());
        }
        else
        {
          message = GetString("TokenReplaceUnknownProperty", GlobalResourceFile, FormatProvider.ToString());
        }

        if (string.IsNullOrEmpty(message))
          message = "Error accessing [{0}:{1}], {1} is unknown for datasource {0}";
        result = string.Format(message, strObjectName, strPropertyName);
      }
      return result;
    }

    #endregion

    #region Public Methods

    /// <summary>
  /// Checks for present [Object:Property] tokens
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <returns></returns>
  /// <history>
  ///    08/10/2007 [sleupold] created
  ///    10/19/2007 [sleupold] corrected to ignore unchanged text returned (issue DNN-6526)
  /// </history>
    public bool ContainsTokens(string strSourceText)
    {
      if (!string.IsNullOrEmpty(strSourceText))
      {
        foreach (Match currentMatch in TokenizerRegex.Matches(strSourceText))
        {
          if (currentMatch.Result("${object}").Length > 0)
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
  /// returns cacheability of the passed text regarding all contained tokens
  /// </summary>
  /// <param name="strSourcetext">the text to parse for tokens to replace</param>
  /// <returns>cacheability level (not - safe - fully)</returns>
  /// <remarks>always check cacheability before caching a module!</remarks>
  /// <history>
  ///    10/19/2007 [sleupold] corrected to handle non-empty strings
  /// </history>
    public CacheLevel Cacheability(string strSourcetext)
    {
      var IsSafe = CacheLevel.fullyCacheable;
      if (strSourcetext != null && !string.IsNullOrEmpty(strSourcetext))
      {

        // initialize PropertyAccess classes
        string DummyResult = ReplaceTokens(strSourcetext);

        var Result = new StringBuilder();
        foreach (Match currentMatch in TokenizerRegex.Matches(strSourcetext))
        {

          string strObjectName = currentMatch.Result("${object}");
          if (strObjectName.Length > 0)
          {
            if (strObjectName == "[")
            {
            }
            // nothing
            else if (!PropertySource.ContainsKey(strObjectName.ToLower()))
            {
            }
            // end if
            else
            {
              var c = PropertySource[strObjectName.ToLower()].Cacheability;
              if (c < IsSafe)
                IsSafe = c;
            }
          }
        }
      }
      return IsSafe;
    }

    #endregion

  }

}