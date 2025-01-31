using System.Web;
using static DotNetNuke.Modules.Blog.Common.Globals;
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

using DotNetNuke.Modules.Blog.Entities.Posts;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Blog.Common
{
  public class PostBodyAndSummary
  {

    public string Body { get; set; } = "";
    public LocalizedText BodyLocalizations { get; set; } = new LocalizedText();
    public string Summary { get; set; } = "";
    public LocalizedText SummaryLocalizations { get; set; } = new LocalizedText();

    #region  Constructors 
    public PostBodyAndSummary(Controls.LongTextEdit contentEditor, Controls.LongTextEdit summaryEditor, SummaryType summaryModel, bool includeLocalizations, bool autoGenerateSummaryIfEmpty, int autoGenerateLength)
    {
      Body = contentEditor.DefaultText;
      Summary = Strings.Trim(summaryEditor.DefaultText);
      if (Summary == "&lt;p&gt;&amp;#160;&lt;/p&gt;")
        Summary = ""; // an empty editor in DNN returns this
      if (includeLocalizations)
      {
        BodyLocalizations = contentEditor.LocalizedTexts;
        SummaryLocalizations = summaryEditor.LocalizedTexts;
      }
      switch (summaryModel)
      {
        case SummaryType.HtmlIndependent:
          {
            break;
          }
        case SummaryType.HtmlPrecedesPost:
          {
            Body = Summary + Body;
            foreach (string l in SummaryLocalizations.Locales)
              BodyLocalizations[l] = SummaryLocalizations[l] + BodyLocalizations[l];
            break;
          }
        case SummaryType.PlainTextIndependent:
          {
            Summary = RemoveHtmlTags(Summary);
            foreach (string l in SummaryLocalizations.Locales)
              SummaryLocalizations[l] = RemoveHtmlTags(SummaryLocalizations[l]);
            break;
          }
      }
      if (autoGenerateSummaryIfEmpty & autoGenerateLength > 0)
      {
        if (string.IsNullOrEmpty(Summary))
          Summary = GetSummary(Body, autoGenerateLength, summaryModel, true);
        foreach (string l in SummaryLocalizations.Locales)
        {
          if (string.IsNullOrEmpty(SummaryLocalizations[l]))
            SummaryLocalizations[l] = GetSummary(BodyLocalizations[l], autoGenerateLength, summaryModel, true);
        }
      }
    }

    public PostBodyAndSummary(PostInfo Post, SummaryType summaryModel, bool includeLocalizations, bool autoGenerateSummaryIfEmpty, int autoGenerateLength)
    {
      Body = HttpUtility.HtmlDecode(Post.Content);
      if (Body is null)
        Body = "";
      if (summaryModel == SummaryType.PlainTextIndependent)
      {
        Summary = Post.Summary;
      }
      else
      {
        Summary = HttpUtility.HtmlDecode(Post.Summary);
      }
      if (summaryModel == SummaryType.HtmlPrecedesPost)
      {
        Body = Body.Substring(Summary.Length);
      }
      if (includeLocalizations)
      {
        foreach (string l in Post.TitleLocalizations.Locales)
        {
          string lBody = "";
          if (Post.ContentLocalizations.ContainsKey(l))
            lBody = Post.ContentLocalizations[l];
          string lSummary = "";
          if (Post.SummaryLocalizations.ContainsKey(l))
            lSummary = Post.SummaryLocalizations[l];
          BodyLocalizations.Add(l, lBody);
          if (summaryModel == SummaryType.PlainTextIndependent)
          {
            SummaryLocalizations.Add(l, lSummary);
          }
          else
          {
            SummaryLocalizations.Add(l, HttpUtility.HtmlDecode(lSummary));
          }
          if (summaryModel == SummaryType.HtmlPrecedesPost)
          {
            BodyLocalizations[l] = BodyLocalizations[l].Substring(SummaryLocalizations[l].Length);
          }
        }
      }
      if (autoGenerateSummaryIfEmpty & autoGenerateLength > 0)
      {
        if (string.IsNullOrEmpty(Summary))
          Summary = GetSummary(Body, autoGenerateLength, summaryModel, false);
        foreach (string l in SummaryLocalizations.Locales)
        {
          if (string.IsNullOrEmpty(SummaryLocalizations[l]))
            SummaryLocalizations[l] = GetSummary(BodyLocalizations[l], autoGenerateLength, summaryModel, false);
        }
      }
    }

    public PostBodyAndSummary(Services.WLW.MetaWeblog.Post post, SummaryType summaryModel, bool autoGenerateSummaryIfEmpty, int autoGenerateLength)
    {
      switch (summaryModel)
      {
        case SummaryType.HtmlIndependent:
          {
            Body = post.description;
            break;
          }
        case SummaryType.HtmlPrecedesPost:
          {
            Body = post.mt_text_more;
            Summary = post.description; // plain text
            break;
          }

        default:
          {
            Body = post.description;
            Summary = post.mt_excerpt;
            break;
          }
      }
      if (autoGenerateSummaryIfEmpty & autoGenerateLength > 0)
      {
        if (string.IsNullOrEmpty(Summary))
          Summary = GetSummary(Body, autoGenerateLength, summaryModel, false);
      }
    }
    #endregion

    #region  Writing 
    public void WriteToPost(ref PostInfo Post, SummaryType summaryModel, bool htmlEncode, bool includeLocalizations)
    {
      if (htmlEncode)
      {
        Body = HttpUtility.HtmlEncode(Body);
        if (!(summaryModel == SummaryType.PlainTextIndependent))
        {
          Summary = HttpUtility.HtmlEncode(Summary);
        }
        if (includeLocalizations)
        {
          foreach (string l in SummaryLocalizations.Locales)
          {
            BodyLocalizations[l] = HttpUtility.HtmlEncode(BodyLocalizations[l]);
            SummaryLocalizations[l] = HttpUtility.HtmlEncode(SummaryLocalizations[l]);
          }
        }
      }
      Post.Content = Body;
      Post.Summary = Summary;
      if (includeLocalizations)
      {
        Post.ContentLocalizations = BodyLocalizations;
        Post.SummaryLocalizations = SummaryLocalizations;
      }
    }

    public void WriteToPost(ref Services.WLW.MetaWeblog.Post post, SummaryType summaryModel)
    {
      switch (summaryModel)
      {
        case SummaryType.HtmlIndependent:
          {
            post.description = Body;
            break;
          }
        case SummaryType.HtmlPrecedesPost:
          {
            post.mt_text_more = Body;
            post.description = Summary; // plain text
            break;
          }

        default:
          {
            post.description = Body;
            post.mt_excerpt = Summary;
            break;
          }
      }
    }
    #endregion

  }
}