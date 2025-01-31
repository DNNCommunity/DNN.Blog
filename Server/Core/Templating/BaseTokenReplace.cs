
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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.Modules.Blog.Templating
{

  /// <summary>
 /// The BaseTokenReplace class provides the tokenization of tokens formatted  
 /// [object:property] or [object:property|format|ifEmpty] or [custom:no] within a string
 /// with the appropriate current property/custom values.
 /// </summary>
 /// <remarks></remarks>
  public abstract class BaseTokenReplace
  {

    #region Regexp Tokenizer


    // Private Const ExpressionDefault As String = "(?:\[(?:(?<object>[^\]\[:]+):(?<property>[^\]\[\|]+))(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?\])|(?<text>\[[^\]\[]+\])|(?<text>[^\]\[]+)"
    private const string ExpressionDefault = @"\[(?<object>[^\]\[:\r\n]+):(?<property>[^\]\[\|\r\n]+)((\])|(\|(?<format>[^\]\[\r\n]+)((\])|(\|(?<ifEmpty>[^\]\[\r\n]+)))))";

    private const string ExpressionObjectLess = @"(?:\[(?:(?<object>[^\]\[:]+):(?<property>[^\]\[\|]+))(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?\])" + @"|(?:(?<object>\[)(?<property>[A-Z0-9._]+)(?:\|(?:(?<format>[^\]\[]+)\|(?<ifEmpty>[^\]\[]+))|\|(?:(?<format>[^\|\]\[]+)))?\])" + @"|(?<text>\[[^\]\[]+\])" + @"|(?<text>[^\]\[]+)";



    private bool _UseObjectLessExpression = false;

    protected bool UseObjectLessExpression
    {
      get
      {
        return _UseObjectLessExpression;
      }
      set
      {
        _UseObjectLessExpression = value;
      }
    }

    private string TokenReplaceCacheKey
    {
      get
      {
        if (UseObjectLessExpression)
        {
          return "Blog_TokenReplaceRegEx_Objectless";
        }
        else
        {
          return "Blog_TokenReplaceRegEx_Default";
        }
      }
    }

    private string RegExpression
    {
      get
      {
        if (UseObjectLessExpression)
        {
          return ExpressionObjectLess;
        }
        else
        {
          return ExpressionDefault;
        }
      }
    }

    protected const string ObjectLessToken = "no_object";

    /// <summary>
  /// Gets the Regular expression for the token to be replaced
  /// </summary>
  /// <value>A regular Expression</value>
    protected Regex TokenizerRegex
    {
      get
      {
        Regex tokenizer = (Regex)DataCache.GetCache(TokenReplaceCacheKey);
        if (tokenizer is null)
        {
          tokenizer = new Regex(RegExpression, RegexOptions.Compiled);
          DataCache.SetCache(TokenReplaceCacheKey, tokenizer);
        }
        return tokenizer;
      }
    }
    #endregion

    #region Fields & Properties
    private string _Language;
    private CultureInfo _FormatProvider;

    /// <summary>
  /// Gets/sets the language to be used, e.g. for date format
  /// </summary>
  /// <value>A string, representing the locale</value>
    public string Language
    {
      get
      {
        return _Language;
      }
      set
      {
        _Language = value;
        _FormatProvider = new CultureInfo(_Language);
      }
    }

    /// <summary>
  /// Gets the Format provider as Culture info from stored language or current culture
  /// </summary>
  /// <value>An CultureInfo</value>
    protected CultureInfo FormatProvider
    {
      get
      {
        if (_FormatProvider is null)
        {
          _FormatProvider = System.Threading.Thread.CurrentThread.CurrentUICulture;
        }
        return _FormatProvider;
      }
    }
    #endregion

    protected virtual string ReplaceTokens(string strSourceText)
    {
      if (strSourceText is null)
        return string.Empty;
      var Result = new StringBuilder();
      return TokenizerRegex.Replace(strSourceText, ReplaceTokenMatch);
    }

    private string ReplaceTokenMatch(Match m)
    {

      string strObjectName = m.Result("${object}");
      if (strObjectName.Length > 0)
      {
        if (strObjectName == "[")
          strObjectName = ObjectLessToken;
        string strPropertyName = m.Result("${property}");
        string strFormat = m.Result("${format}");
        string strIfEmptyReplacment = m.Result("${ifEmpty}");
        string strConversion = replacedTokenValue(strObjectName, strPropertyName, strFormat);
        if (strIfEmptyReplacment.Length > 0 && strConversion.Length == 0)
          strConversion = strIfEmptyReplacment;
        return strConversion;
      }
      else
      {
        return m.Value;
      }

    }


    protected abstract string replacedTokenValue(string strObjectName, string strPropertyName, string strFormat);

  }
}