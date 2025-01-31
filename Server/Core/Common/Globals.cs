using System;
using System.Collections.Generic;
using System.Data;
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

using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Content.Taxonomy;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Common
{

  public class Globals
  {

    #region  Constants 
    public const string SharedResourceFileName = "~/DesktopModules/Blog/App_LocalResources/SharedResources.resx";
    public const string glbAppName = "Blog";
    public const string glbImageHandlerPath = "~/DesktopModules/Blog/BlogImage.ashx";
    public const string glbPermittedFileExtensions = ".jpg,.png,.gif,.bmp,";
    public const string glbTemplatesPath = "~/DesktopModules/Blog/Templates/";
    public const string glbServicesPath = "~/DesktopModules/Blog/API/";
    public const string BloggerPermission = "BLOGGER";

    public enum SummaryType
    {
      PlainTextIndependent = 0,
      HtmlIndependent = 1,
      HtmlPrecedesPost = 2
    }

    public enum LocalizationType
    {
      None = 0,
      Loose = 1,
      Strict = 2
    }
    #endregion

    #region  Dates 
    public static DateTime UtcToLocalTime(DateTime utcTime, TimeZoneInfo TimeZone)
    {
      return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZone);
    }

    public static DateTime ParseDate(string DateString, string Culture)
    {
      var dtf = new System.Globalization.CultureInfo(Culture, false).DateTimeFormat;
      try
      {
        return DateTime.Parse(DateString, dtf);
      }
      catch (Exception ex)
      {
        return default;
      }
    }

    public static bool IsValidDate(string DateString, string Culture)
    {
      var dtf = new System.Globalization.CultureInfo(Culture, false).DateTimeFormat;
      try
      {
        var oDate = DateTime.Parse(DateString, dtf);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static DateTime GetLocalAddedTime(DateTime AddedDate, int PortalId, DotNetNuke.Entities.Users.UserInfo user)
    {
      return TimeZoneInfo.ConvertTimeToUtc(AddedDate, user.Profile.PreferredTimeZone);
    }
    #endregion

    #region  Other 
    public static string GetBlogDirectoryMapPath(int blogId)
    {
      return string.Format(@"{0}Blog\Files\{1}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, blogId);
    }
    public static string GetBlogDirectoryPath(int blogId)
    {
      return string.Format("{0}Blog/Files/{1}/", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory, blogId);
    }
    public static string GetPostDirectoryMapPath(int blogId, int postId)
    {
      return string.Format(@"{0}Blog\Files\{1}\{2}\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, blogId, postId);
    }
    public static string GetPostDirectoryPath(int blogId, int postId)
    {
      return string.Format("{0}Blog/Files/{1}/{2}/", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory, blogId, postId);
    }
    public static string GetPostDirectoryMapPath(Entities.Posts.PostInfo post)
    {
      return GetPostDirectoryMapPath(post.BlogID, post.ContentItemId);
    }
    public static string GetPostDirectoryPath(Entities.Posts.PostInfo post)
    {
      return GetPostDirectoryPath(post.BlogID, post.ContentItemId);
    }
    public static string GetTempPostDirectoryMapPath(int blogId)
    {
      return string.Format(@"{0}Blog\Files\{1}\_temp_images\", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectoryMapPath, blogId);
    }
    public static string GetTempPostDirectoryPath(int blogId)
    {
      return string.Format("{0}Blog/Files/{1}/_temp_images/", DotNetNuke.Entities.Portals.PortalSettings.Current.HomeDirectory, blogId);
    }

    public static string ManifestFilePath(int tabId, int moduleId)
    {
      return string.Format("~/DesktopModules/Blog/API/Modules/Manifest?TabId={0}&ModuleId={1}", tabId, moduleId);
    }

    public static string GetAString(object Value)
    {
      if (Value is null)
      {
        return "";
      }
      else if (ReferenceEquals(Value, DBNull.Value))
      {
        return "";
      }
      else
      {
        return Conversions.ToString(Value);
      }
    }

    public static string ReadFile(string fileName)
    {
      if (!System.IO.File.Exists(fileName))
        return "";
      using (var sr = new System.IO.StreamReader(fileName))
      {
        return sr.ReadToEnd();
      }
    }

    public static void WriteToFile(string filePath, string text)
    {
      WriteToFile(filePath, text, false);
    }
    public static void WriteToFile(string filePath, string text, bool append)
    {
      using (var sw = new System.IO.StreamWriter(filePath, append))
      {
        sw.Write(text);
        sw.Flush();
      }
    }

    public static string GetResource(string resourceName)
    {
      string res = "";
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        using (var rdr = new System.IO.StreamReader(stream))
        {
          res = rdr.ReadToEnd();
        }
      }
      return res;
    }

    public static string FormatBoolean(bool value, string format)
    {
      if (string.IsNullOrEmpty(format))
      {
        return value.ToString();
      }
      if (format.Contains(";"))
      {
        if (value)
        {
          return Strings.Left(format, format.IndexOf(";"));
        }
        else
        {
          return Strings.Mid(format, format.IndexOf(";") + 2);
        }
      }
      return value.ToString();
    }

    public static string GetSummary(string body, int autoGenerateLength, SummaryType summaryModel, bool encoded)
    {
      if (string.IsNullOrEmpty(body))
        return "";
      string res = body;
      if (encoded)
      {
        res = HttpUtility.HtmlDecode(res);
      }
      res = TryToGetFirstParagraph(res);
      res = RemoveHtmlTags(res).SubstringWithoutException(0, autoGenerateLength);
      if (res.Length >= autoGenerateLength)
        res += " ...";
      if (!(summaryModel == SummaryType.PlainTextIndependent))
      {
        res = string.Format("<p>{0}</p>", res);
      }
      if (encoded)
      {
        return HttpUtility.HtmlEncode(res);
      }
      else
      {
        return res;
      }
    }

    public static string TryToGetFirstParagraph(string inputString)
    {
      var m = Regex.Match(inputString, "(?s)(?i)<p[^>]*>((?:(?!</p>).)*)(?-i)(?-s)");
      if (m.Success)
      {
        return m.Groups[1].Value;
      }
      return inputString;
    }

    public static string RemoveHtmlTags(string inputString)
    {
      inputString = Regex.Replace(inputString, "<[^>]+>", "");
      return new DotNetNuke.Security.PortalSecurity().InputFilter(inputString, DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting | DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup);
    }

    internal static List<Vocabulary> GetPortalVocabularies(int portalId)
    {
      var cntVocab = DotNetNuke.Entities.Content.Common.Util.GetVocabularyController();
      var colVocabularies = cntVocab.GetVocabularies();
      var portalVocabularies = from v in colVocabularies
                               where v.ScopeTypeId == 2 & v.ScopeId == portalId
                               select v;
      return portalVocabularies.ToList();
    }

    public static string GetSafePageName(string pageName)
    {
      return Regex.Replace(Regex.Replace(pageName, @"[^\w^\d]", "-").Trim('-'), "-+", "-").RemoveDiacritics();
    }

    public static void RemoveOldTimeStampedFiles(System.IO.DirectoryInfo dir)
    {
      string today = DateTime.Now.ToString("yyyy-MM-dd");
      var deleteList = new List<string>();
      foreach (System.IO.FileInfo f in dir.GetFiles())
      {
        var m = Regex.Match(f.Name, @"^(\d\d\d\d-\d\d-\d\d)-");
        if (m.Success)
        {
          if (Operators.CompareString(m.Groups[1].Value, today, false) < 0)
          {
            deleteList.Add(f.FullName);
          }
        }
      }
      foreach (string f in deleteList)
      {
        try
        {
          System.IO.File.Delete(f);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static List<DotNetNuke.Security.Roles.RoleInfo> GetRolesByGroup(int portalId, int roleGroupId)
    {
      return DotNetNuke.Security.Roles.RoleProvider.Instance().GetRoles(portalId).Cast<DotNetNuke.Security.Roles.RoleInfo>().Where(r => r.RoleGroupID == roleGroupId).ToList();
    }

    public static List<DotNetNuke.Security.Roles.RoleInfo> GetRolesByPortal(int portalId)
    {
      return DotNetNuke.Security.Roles.RoleProvider.Instance().GetRoles(portalId).Cast<DotNetNuke.Security.Roles.RoleInfo>().ToList();
    }

    public static string SafeString(string input, DotNetNuke.Security.PortalSecurity.FilterFlag filter)
    {
      var ps = new DotNetNuke.Security.PortalSecurity();
      return ps.InputFilter(input, filter);
    }

    public static string SafeString(string input)
    {
      return SafeString(input, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup & DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity & DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting & DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL);
    }

    public static string SafeHtml(string input)
    {
      return SafeString(input, DotNetNuke.Security.PortalSecurity.FilterFlag.NoProfanity & DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting & DotNetNuke.Security.PortalSecurity.FilterFlag.NoSQL);
    }

    public static string SafeStringSimpleHtml(string input)
    {
      input = SafeString(input);
      input = Regex.Replace(input, @"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~\/|\/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:\/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|\/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=?(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?", ReplaceLink);
      input = input.Replace(Constants.vbCrLf, "<br />");
      return input;
    }

    public static string CleanStringForXmlAttribute(string input)
    {
      return input.Replace("&", "and").Replace("\"", "&quot;");
    }

    public static string ReplaceLink(Match m)
    {
      string link = m.Value;
      return string.Format("<a href=\"{0}\">{0}</a>", link);
    }

    public static string readElement(System.Xml.XmlReader reader, string ElementName)
    {
      if (!(reader.NodeType == System.Xml.XmlNodeType.Element) || (reader.Name ?? "") != (ElementName ?? ""))
      {
        reader.ReadToFollowing(ElementName);
      }
      if (reader.NodeType == System.Xml.XmlNodeType.Element)
      {
        return reader.ReadElementContentAsString();
      }
      else
      {
        return "";
      }
    }

    public static string readAttribute(System.Xml.XmlReader reader, string attributeName)
    {
      if (!(reader.NodeType == System.Xml.XmlNodeType.Attribute) || (reader.Name ?? "") != (attributeName ?? ""))
      {
        reader.ReadToFollowing(attributeName);
      }
      if (reader.NodeType == System.Xml.XmlNodeType.Attribute)
      {
        return reader.ReadContentAsString();
      }
      else
      {
        return "";
      }
    }
    #endregion

  }

}