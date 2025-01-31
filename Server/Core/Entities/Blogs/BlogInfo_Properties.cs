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

namespace DotNetNuke.Modules.Blog.Entities.Blogs
{
  public partial class BlogInfo
  {

    #region  Private Members 
    #endregion

    #region  Constructors 
    public BlogInfo()
    {
      try
      {
        Locale = DotNetNuke.Entities.Portals.PortalSettings.Current.DefaultLanguage;
      }
      catch (Exception ex)
      {
      }
    }
    #endregion

    #region  Public Properties 
    [DataMember()]
    public int BlogID { get; set; } = -1;
    [DataMember()]
    public int ModuleID { get; set; } = -1;
    [DataMember()]
    public string Title { get; set; } = "";
    [DataMember()]
    public string Description { get; set; } = "";
    [DataMember()]
    public string Image { get; set; } = "";
    [DataMember()]
    public string Locale { get; set; } = "";
    [DataMember()]
    public bool FullLocalization { get; set; } = false;
    [DataMember()]
    public bool Published { get; set; } = true;
    [DataMember()]
    public bool IncludeImagesInFeed { get; set; } = true;
    [DataMember()]
    public bool IncludeAuthorInFeed { get; set; } = false;
    [DataMember()]
    public bool Syndicated { get; set; } = true;
    [DataMember()]
    public string SyndicationEmail { get; set; } = "";
    [DataMember()]
    public string Copyright { get; set; } = "";
    [DataMember()]
    public bool MustApproveGhostPosts { get; set; } = false;
    [DataMember()]
    public bool PublishAsOwner { get; set; } = false;
    [DataMember()]
    public bool EnablePingBackSend { get; set; } = true;
    [DataMember()]
    public bool EnablePingBackReceive { get; set; } = true;
    [DataMember()]
    public bool AutoApprovePingBack { get; set; } = false;
    [DataMember()]
    public bool EnableTrackBackSend { get; set; } = true;
    [DataMember()]
    public bool EnableTrackBackReceive { get; set; } = false;
    [DataMember()]
    public bool AutoApproveTrackBack { get; set; } = false;
    [DataMember()]
    public int OwnerUserId { get; set; } = -1;
    [DataMember()]
    public int CreatedByUserID { get; set; } = -1;
    [DataMember()]
    public DateTime CreatedOnDate { get; set; } = DateTime.MinValue;
    [DataMember()]
    public int LastModifiedByUserID { get; set; } = -1;
    [DataMember()]
    public DateTime LastModifiedOnDate { get; set; } = DateTime.MinValue;
    [DataMember()]
    public string DisplayName { get; set; } = "";
    [DataMember()]
    public string Email { get; set; } = "";
    [DataMember()]
    public string Username { get; set; } = "";
    [DataMember()]
    public int NrPosts { get; set; } = -1;
    [DataMember()]
    public DateTime LastPublishDate { get; set; } = DateTime.MinValue;
    [DataMember()]
    public int NrViews { get; set; } = -1;
    [DataMember()]
    public DateTime FirstPublishDate { get; set; } = DateTime.MinValue;
    #endregion

    #region  ML Properties 
    [DataMember()]
    public string AltLocale { get; set; } = "";
    [DataMember()]
    public string AltTitle { get; set; } = "";
    [DataMember()]
    public string AltDescription { get; set; } = "";

    public string LocalizedTitle
    {
      get
      {
        return Conversions.ToString(Interaction.IIf(string.IsNullOrEmpty(AltTitle), Title, AltTitle));
      }
    }

    public string LocalizedDescription
    {
      get
      {
        return Conversions.ToString(Interaction.IIf(string.IsNullOrEmpty(AltDescription), Description, AltDescription));
      }
    }

    /// <summary>
  /// ML text type to handle the title of the blog
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
          if (BlogID == -1)
          {
            _titleLocalizations = new LocalizedText();
          }
          else
          {
            _titleLocalizations = new LocalizedText(Data.DataProvider.Instance().GetBlogLocalizations(BlogID), "Title");
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
  /// ML text type to handle the description of the blog
  /// </summary>
  /// <value></value>
  /// <returns></returns>
  /// <remarks></remarks>
    public LocalizedText DescriptionLocalizations
    {
      get
      {
        if (_descriptionLocalizations is null)
        {
          if (BlogID == -1)
          {
            _descriptionLocalizations = new LocalizedText();
          }
          else
          {
            _descriptionLocalizations = new LocalizedText(Data.DataProvider.Instance().GetBlogLocalizations(BlogID), "Description");
          }
        }
        return _descriptionLocalizations;
      }
      set
      {
        _descriptionLocalizations = value;
      }
    }
    private LocalizedText _descriptionLocalizations;
    #endregion

  }
}