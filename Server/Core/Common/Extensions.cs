using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;

namespace DotNetNuke.Modules.Blog.Core.Common
{
  public static class Extensions
  {
    public static int ReadValue(this Hashtable valueTable, string valueName, int defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (int)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static long ReadValue(this Hashtable valueTable, string valueName, long defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (long)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static string ReadValue(this Hashtable valueTable, string valueName, string defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (string)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static bool ReadValue(this Hashtable valueTable, string valueName, bool defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (bool)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static DateTime ReadValue(this Hashtable valueTable, string valueName, DateTime defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (DateTime)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static SummaryType ReadValue(this Hashtable valueTable, string valueName, SummaryType defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (SummaryType)(int)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static LocalizationType ReadValue(this Hashtable valueTable, string valueName, LocalizationType defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (LocalizationType)(int)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static TimeSpan ReadValue(this Hashtable valueTable, string valueName, TimeSpan defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return TimeSpan.Parse((string)valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static int ReadValue(this NameValueCollection valueTable, string valueName, int defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return int.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static long ReadValue(this NameValueCollection valueTable, string valueName, long defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return long.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static string ReadValue(this NameValueCollection valueTable, string valueName, string defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static bool ReadValue(this NameValueCollection valueTable, string valueName, bool defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return bool.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
          switch (valueTable[valueName].ToLowerInvariant())
          {
            case "on":
            case "yes":
              {
                return true;
              }
            default:
              {
                return false;
              }
          }
        }
      }
      return defaultValue;
    }

    public static DateTime ReadValue(this NameValueCollection valueTable, string valueName, DateTime defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return DateTime.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static TimeSpan ReadValue(this NameValueCollection valueTable, string valueName, TimeSpan defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return TimeSpan.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static int ReadValue(this Dictionary<string, string> valueTable, string valueName, int defaultValue)
    {
      if (valueTable.ContainsKey(valueName))
      {
        try
        {
          return int.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static string ReadValue(this Dictionary<string, string> valueTable, string valueName, string defaultValue)
    {
      if (valueTable.ContainsKey(valueName))
      {
        try
        {
          return valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static bool ReadValue(this Dictionary<string, string> valueTable, string valueName, bool defaultValue)
    {
      if (valueTable.ContainsKey(valueName))
      {
        try
        {
          return bool.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static DateTime ReadValue(this Dictionary<string, string> valueTable, string valueName, DateTime defaultValue)
    {
      if (valueTable.ContainsKey(valueName))
      {
        try
        {
          return DateTime.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static TimeSpan ReadValue(this Dictionary<string, string> valueTable, string valueName, TimeSpan defaultValue)
    {
      if (valueTable.ContainsKey(valueName))
      {
        try
        {
          return TimeSpan.Parse(valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static int ReadValue(this StateBag valueTable, string valueName, int defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (int)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static long ReadValue(this StateBag valueTable, string valueName, long defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (long)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static string ReadValue(this StateBag valueTable, string valueName, string defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (string)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static bool ReadValue(this StateBag valueTable, string valueName, bool defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (bool)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static DateTime ReadValue(this StateBag valueTable, string valueName, DateTime defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return (DateTime)valueTable[valueName];
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static TimeSpan ReadValue(this StateBag valueTable, string valueName, TimeSpan defaultValue)
    {
      if (valueTable[valueName] != null)
      {
        try
        {
          return TimeSpan.Parse((string)valueTable[valueName]);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static int ReadValue(this XmlNode valueTable, string valueName, int defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return int.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static long ReadValue(this XmlNode valueTable, string valueName, long defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return long.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static string ReadValue(this XmlNode valueTable, string valueName, string defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return valueTable.SelectSingleNode(valueName).InnerText;
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static bool ReadValue(this XmlNode valueTable, string valueName, bool defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return bool.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static DateTime ReadValue(this XmlNode valueTable, string valueName, DateTime defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return DateTime.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static SummaryType ReadValue(this XmlNode valueTable, string valueName, SummaryType defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return (SummaryType)int.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static LocalizationType ReadValue(this XmlNode valueTable, string valueName, LocalizationType defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return (LocalizationType)int.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static TimeSpan ReadValue(this XmlNode valueTable, string valueName, TimeSpan defaultValue)
    {
      if (valueTable.SelectSingleNode(valueName) != null)
      {
        try
        {
          return TimeSpan.Parse(valueTable.SelectSingleNode(valueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static LocalizedText ReadValue(this XmlNode valueTable, string valueName, LocalizedText defaultValue)
    {

      if (valueTable.SelectSingleNode(valueName) != null)
      {
        if (valueTable.SelectSingleNode(valueName).SelectSingleNode("MLText") != null)
        {
          if (defaultValue is null)
            return new LocalizedText();
          foreach (XmlNode t in valueTable.SelectSingleNode(valueName).SelectSingleNode("MLText").SelectNodes("Text"))
            defaultValue.Add(t.Attributes["Locale"].InnerText, t.InnerText);
        }
      }
      return defaultValue;
    }

    public static bool ReadValue(this XmlReader valueTable, string valueName, bool defaultValue)
    {
      var stringValue = valueTable.ReadElement(valueName);
      if (!string.IsNullOrEmpty(stringValue))
      {
        try
        {
          return bool.Parse(stringValue);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static int ReadValue(this XmlReader valueTable, string valueName, int defaultValue)
    {
      var stringValue = valueTable.ReadElement(valueName);
      if (!string.IsNullOrEmpty(stringValue))
      {
        try
        {
          return int.Parse(stringValue);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static DateTime ReadValue(this XmlReader valueTable, string valueName, DateTime defaultValue)
    {
      var stringValue = valueTable.ReadElement(valueName);
      if (!string.IsNullOrEmpty(stringValue))
      {
        try
        {
          return DateTime.Parse(stringValue);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static SummaryType ReadValue(this XmlReader valueTable, string valueName, SummaryType defaultValue)
    {
      var stringValue = valueTable.ReadElement(valueName);
      if (!string.IsNullOrEmpty(stringValue))
      {
        try
        {
          return (SummaryType)int.Parse(stringValue);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static LocalizationType ReadValue(this XmlReader valueTable, string valueName, LocalizationType defaultValue)
    {
      var stringValue = valueTable.ReadElement(valueName);
      if (!string.IsNullOrEmpty(stringValue))
      {
        try
        {
          return (LocalizationType)int.Parse(stringValue);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static TimeSpan ReadValue(this XmlReader valueTable, string valueName, TimeSpan defaultValue)
    {
      var stringValue = valueTable.ReadElement(valueName);
      if (!string.IsNullOrEmpty(stringValue))
      {
        try
        {
          return TimeSpan.Parse(stringValue);
        }
        catch (Exception ex)
        {
        }
      }
      return defaultValue;
    }

    public static string ReadElement(this XmlReader reader, string ElementName)
    {
      if (!(reader.NodeType == XmlNodeType.Element) || (reader.Name ?? "") != (ElementName ?? ""))
      {
        reader.ReadToFollowing(ElementName);
      }
      if (reader.NodeType == XmlNodeType.Element)
      {
        return reader.ReadElementContentAsString();
      }
      else
      {
        return "";
      }
    }

    public static string OutputHtml(this string encodedHtml, string strFormat)
    {
      switch (strFormat.ToLower() ?? "")
      {
        case var @case when @case == "":
          {
            return HttpUtility.HtmlDecode(encodedHtml);
          }
        case "js":
          {
            return HttpUtility.HtmlDecode(encodedHtml).Replace("\"", @"\""").Replace("'", @"\'").Replace(Environment.NewLine, @"\r\n");
          }

        default:
          {
            if (int.TryParse(strFormat, out int res))
            {
              return Globals.RemoveHtmlTags(HttpUtility.HtmlDecode(encodedHtml)).SubstringWithoutException(0, res);
            }
            else
            {
              return HttpUtility.HtmlDecode(encodedHtml);
            }
          }
      }
    }

    public static int ToInt(this bool @var)
    {
      if (@var)
      {
        return 1;
      }
      else
      {
        return 0;
      }
    }

    public static string ToYesNo(this bool @var)
    {
      if (@var)
      {
        return "Yes";
      }
      else
      {
        return "No";
      }
    }

    public static int ToInt(this string @var)
    {
      if (int.TryParse(@var, out int res))
      {
        return res;
      }
      else
      {
        return -1;
      }
    }

    public static bool ToBool(this int @var)
    {
      return @var > 0;
    }

    public static string[] ToStringArray(this List<TermInfo> terms)
    {
      return terms.Select(x => x.Name).ToArray();
    }

    public static string ToTermIDString(this List<TermInfo> terms)
    {
      return terms.ToTermIDString(";");
    }

    public static string ToTermIDString(this List<TermInfo> terms, string separator)
    {
      var res = new List<string>();
      foreach (TermInfo t in terms)
        res.Add(t.TermId.ToString());
      return string.Join(separator, res.ToArray());
    }

    public static string ToStringOrZero(this int? value)
    {
      if (value is null)
      {
        return "0";
      }
      else
      {
        return value.ToString();
      }
    }

    public static string SubstringWithoutException(this string input, int startIndex, int length)
    {
      if (string.IsNullOrEmpty(input))
        return "";
      if (startIndex > 0)
      {
        if (startIndex >= input.Length)
        {
          return "";
        }
        if (startIndex + length > input.Length)
        {
          return input.Substring(startIndex, input.Length - startIndex);
        }
        else
        {
          return input.Substring(startIndex, length);
        }
      }
      else if (length > input.Length)
      {
        return input.Substring(0, input.Length - startIndex);
      }
      else
      {
        return input.Substring(0, length);
      }
    }

    public static void WriteAttachmentToXml(this BlogML.Xml.BlogMLAttachment attachment, XmlWriter writer)
    {
      writer.WriteStartElement("File");
      writer.WriteElementString("Path", attachment.Path);
      writer.WriteStartElement("Data");
      writer.WriteBase64(attachment.Data, 0, attachment.Data.Length - 1);
      writer.WriteEndElement(); // Data
      writer.WriteEndElement(); // File
    }

    public static string RemoveDiacritics(this string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return text;
      }

      string normalizedString = text.Normalize(NormalizationForm.FormD);
      var stringBuilder = new StringBuilder();

      foreach (char c in normalizedString)
      {
        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        {
          stringBuilder.Append(c);
        }
      }

      return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static void AddJavascriptFile(this Page page, string moduleVersion, string jsFilename, int priority)
    {
      if (DotNetNuke.Entities.Host.Host.CrmEnableCompositeFiles)
      {
        ClientResourceManager.RegisterScript(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/js/" + jsFilename), priority);
      }
      else
      {
        ClientResourceManager.RegisterScript(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/js/" + jsFilename) + "?_=" + moduleVersion, priority);
      }
    }

    public static void AddJavascriptFile(this Page page, string moduleVersion, string jsFilename, string name, string version, int priority)
    {
      if (DotNetNuke.Entities.Host.Host.CrmEnableCompositeFiles)
      {
        ClientResourceManager.RegisterScript(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/js/" + jsFilename), priority, "DnnBodyProvider", name, version);
      }
      else
      {
        ClientResourceManager.RegisterScript(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/js/" + jsFilename) + "?_=" + moduleVersion, priority, "DnnBodyProvider", name, version);
      }
    }

    public static void AddCssFile(this Page page, string moduleVersion, string cssFilename)
    {
      if (DotNetNuke.Entities.Host.Host.CrmEnableCompositeFiles)
      {
        ClientResourceManager.RegisterStyleSheet(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/css/" + cssFilename), FileOrder.Css.ModuleCss);
      }
      else
      {
        ClientResourceManager.RegisterStyleSheet(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/css/" + cssFilename) + "?_=" + moduleVersion, FileOrder.Css.ModuleCss);
      }
    }

    public static void AddCssFile(this Page page, string moduleVersion, string cssFilename, string name, string version)
    {
      if (DotNetNuke.Entities.Host.Host.CrmEnableCompositeFiles)
      {
        ClientResourceManager.RegisterStyleSheet(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/css/" + cssFilename), (int)FileOrder.Css.ModuleCss, "DnnPageHeaderProvider", name, version);
      }
      else
      {
        ClientResourceManager.RegisterStyleSheet(page, DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/Blog/css/" + cssFilename) + "?_=" + moduleVersion, (int)FileOrder.Css.ModuleCss, "DnnPageHeaderProvider", name, version);
      }
    }

  }
}