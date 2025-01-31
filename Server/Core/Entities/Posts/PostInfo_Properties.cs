using System;
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

using System.Runtime.Serialization;
using DotNetNuke.Modules.Blog.Common;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Entities.Posts
{
  public partial class PostInfo : DotNetNuke.Entities.Content.ContentItem
  {

    #region  Private Members 
    #endregion

    #region  Constructors 
    public PostInfo()
    {
    }
    #endregion

    #region  Public Properties 
    [DataMember()]
    public int BlogID { get; set; } = -1;
    [DataMember()]
    public string Title { get; set; } = "";
    [DataMember()]
    public string Summary { get; set; } = "";
    [DataMember()]
    public string Image { get; set; } = "";
    [DataMember()]
    public bool Published { get; set; } = false;
    [DataMember()]
    public DateTime PublishedOnDate { get; set; } = DateTime.Now.ToUniversalTime();
    [DataMember()]
    public bool AllowComments { get; set; } = true;
    [DataMember()]
    public bool DisplayCopyright { get; set; } = false;
    [DataMember()]
    public string Copyright { get; set; } = "";
    [DataMember()]
    public string Locale { get; set; } = null;
    [DataMember()]
    public int ViewCount { get; set; } = 0;
    [DataMember()]
    public string Username { get; set; } = "";
    [DataMember()]
    public string Email { get; set; } = "";
    [DataMember()]
    public string DisplayName { get; set; } = "";
    [DataMember()]
    public int NrComments { get; set; } = 0;
    #endregion

    #region  ML Properties 
    [DataMember()]
    public string AltLocale { get; set; } = "";
    [DataMember()]
    public string AltTitle { get; set; } = "";
    [DataMember()]
    public string AltSummary { get; set; } = "";
    [DataMember()]
    public string AltContent { get; set; } = "";

    public string LocalizedTitle
    {
      get
      {
        return Conversions.ToString(Interaction.IIf(string.IsNullOrEmpty(AltTitle), Title, AltTitle));
      }
    }

    public string LocalizedSummary
    {
      get
      {
        return Conversions.ToString(Interaction.IIf(string.IsNullOrEmpty(AltSummary), Summary, AltSummary));
      }
    }

    public string LocalizedContent
    {
      get
      {
        return Conversions.ToString(Interaction.IIf(string.IsNullOrEmpty(AltContent), Content, AltContent));
      }
    }

    /// <summary>
  /// ML text type to handle the title of the post
  /// </summary>
  /// <value></value>
  /// <returns></returns>
  /// <remarks></remarks>
    public LocalizedText TitleLocalizations
    {
      get
      {
        if (_titleLocalizations is null)
        {
          if (ContentItemId == -1)
          {
            _titleLocalizations = new LocalizedText();
          }
          else
          {
            _titleLocalizations = new LocalizedText(Data.DataProvider.Instance().GetPostLocalizations(ContentItemId), "Title");
          }
        }
        return _titleLocalizations;
      }
      set
      {
        _titleLocalizations = value;
      }
    }
    private LocalizedText _titleLocalizations;

    /// <summary>
  /// ML text type to handle the summary of the post
  /// </summary>
  /// <value></value>
  /// <returns></returns>
  /// <remarks></remarks>
    public LocalizedText SummaryLocalizations
    {
      get
      {
        if (_summaryLocalizations is null)
        {
          if (ContentItemId == -1)
          {
            _summaryLocalizations = new LocalizedText();
          }
          else
          {
            _summaryLocalizations = new LocalizedText(Data.DataProvider.Instance().GetPostLocalizations(ContentItemId), "Summary");
          }
        }
        return _summaryLocalizations;
      }
      set
      {
        _summaryLocalizations = value;
      }
    }
    private LocalizedText _summaryLocalizations;

    /// <summary>
  /// ML text type to handle the content of the post
  /// </summary>
  /// <value></value>
  /// <returns></returns>
  /// <remarks></remarks>
    public LocalizedText ContentLocalizations
    {
      get
      {
        if (_contentLocalizations is null)
        {
          if (BlogID == -1)
          {
            _contentLocalizations = new LocalizedText();
          }
          else
          {
            _contentLocalizations = new LocalizedText(Data.DataProvider.Instance().GetPostLocalizations(ContentItemId), "Content");
          }
        }
        return _contentLocalizations;
      }
      set
      {
        _contentLocalizations = value;
      }
    }
    private LocalizedText _contentLocalizations;
    #endregion

  }
}