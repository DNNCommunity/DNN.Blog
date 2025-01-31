using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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
using static DotNetNuke.Modules.Blog.Common.Globals;
using DotNetNuke.Modules.Blog.Entities.Terms;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Blog.Common
{
  static class Extensions
  {

    #region  Collection Read Extensions 
    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref int Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToInteger(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref long Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToLong(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref string Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToString(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref bool Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToBoolean(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref DateTime Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToDate(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref SummaryType Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = (SummaryType)Conversions.ToInteger(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref LocalizationType Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = (LocalizationType)Conversions.ToInteger(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref Hashtable ValueTable, string ValueName, ref TimeSpan Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = TimeSpan.Parse(Conversions.ToString(ValueTable[ValueName]));
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref NameValueCollection ValueTable, string ValueName, ref int Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToInteger(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref NameValueCollection ValueTable, string ValueName, ref long Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToLong(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref NameValueCollection ValueTable, string ValueName, ref string Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = ValueTable[ValueName];
          Variable = new DotNetNuke.Security.PortalSecurity().InputFilter(Variable, DotNetNuke.Security.PortalSecurity.FilterFlag.NoMarkup | DotNetNuke.Security.PortalSecurity.FilterFlag.NoScripting);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref NameValueCollection ValueTable, string ValueName, ref bool Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToBoolean(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
          switch (ValueTable[ValueName].ToLower() ?? "")
          {
            case "on":
            case "yes":
              {
                Variable = true;
                break;
              }

            default:
              {
                Variable = false;
                break;
              }
          }
        }
      }
    }

    public static void ReadValue(ref NameValueCollection ValueTable, string ValueName, ref DateTime Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToDate(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref NameValueCollection ValueTable, string ValueName, ref TimeSpan Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = TimeSpan.Parse(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(this Dictionary<string, string> ValueTable, string ValueName, ref int Variable)
    {
      if (ValueTable.ContainsKey(ValueName))
      {
        try
        {
          Variable = Conversions.ToInteger(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(this Dictionary<string, string> ValueTable, string ValueName, ref string Variable)
    {
      if (ValueTable.ContainsKey(ValueName))
      {
        try
        {
          Variable = ValueTable[ValueName];
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(this Dictionary<string, string> ValueTable, string ValueName, ref bool Variable)
    {
      if (ValueTable.ContainsKey(ValueName))
      {
        try
        {
          Variable = Conversions.ToBoolean(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(this Dictionary<string, string> ValueTable, string ValueName, ref DateTime Variable)
    {
      if (ValueTable.ContainsKey(ValueName))
      {
        try
        {
          Variable = Conversions.ToDate(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(this Dictionary<string, string> ValueTable, string ValueName, ref TimeSpan Variable)
    {
      if (ValueTable.ContainsKey(ValueName))
      {
        try
        {
          Variable = TimeSpan.Parse(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref StateBag ValueTable, string ValueName, ref int Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToInteger(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref StateBag ValueTable, string ValueName, ref long Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToLong(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref StateBag ValueTable, string ValueName, ref string Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToString(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref StateBag ValueTable, string ValueName, ref bool Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToBoolean(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref StateBag ValueTable, string ValueName, ref DateTime Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = Conversions.ToDate(ValueTable[ValueName]);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref StateBag ValueTable, string ValueName, ref TimeSpan Variable)
    {
      if (ValueTable[ValueName] is not null)
      {
        try
        {
          Variable = TimeSpan.Parse(Conversions.ToString(ValueTable[ValueName]));
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref int Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = Conversions.ToInteger(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref long Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = Conversions.ToLong(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref string Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = ValueTable.SelectSingleNode(ValueName).InnerText;
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref bool Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = Conversions.ToBoolean(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref DateTime Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = Conversions.ToDate(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref SummaryType Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = (SummaryType)Conversions.ToInteger(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref LocalizationType Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = (LocalizationType)Conversions.ToInteger(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref TimeSpan Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        try
        {
          Variable = TimeSpan.Parse(ValueTable.SelectSingleNode(ValueName).InnerText);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void ReadValue(ref XmlNode ValueTable, string ValueName, ref LocalizedText Variable)
    {
      if (ValueTable.SelectSingleNode(ValueName) is not null)
      {
        if (ValueTable.SelectSingleNode(ValueName).SelectSingleNode("MLText") is not null)
        {
          if (Variable is null)
            Variable = new LocalizedText();
          foreach (XmlNode t in ValueTable.SelectSingleNode(ValueName).SelectSingleNode("MLText").SelectNodes("Text"))
            Variable.Add(t.Attributes["Locale"].InnerText, t.InnerText);
        }
      }
    }
    #endregion

    #region  Conversion Extensions 
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
      if (Information.IsNumeric(@var))
      {
        return int.Parse(@var);
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
    #endregion

    #region  Other 
    public static Control FindControlByID(this Control Control, string id)
    {
      Control found = null;
      if (Control is not null)
      {
        if ((Control.ID ?? "") == (id ?? ""))
        {
          found = Control;
        }
        else
        {
          found = Control.Controls.FindControlByID(id);
        }
      }
      return found;
    }

    public static Control FindControlByID(this ControlCollection Controls, string id)
    {
      Control found = null;
      if (Controls is not null && Controls.Count > 0)
      {
        for (int i = 0, loopTo = Controls.Count - 1; i <= loopTo; i++)
        {
          if ((Controls[i].ID ?? "") == (id ?? ""))
          {
            found = Controls[i];
          }
          else
          {
            found = Controls[i].Controls.FindControlByID(id);
          }
          if (found is not null)
            break;
        }
      }
      return found;
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
            return HttpUtility.HtmlDecode(encodedHtml).Replace("\"", @"\""").Replace("'", @"\'").Replace(Constants.vbCrLf, @"\r\n");
          }

        default:
          {
            if (Information.IsNumeric(strFormat))
            {
              return RemoveHtmlTags(HttpUtility.HtmlDecode(encodedHtml)).SubstringWithoutException(0, int.Parse(strFormat));
            }
            else
            {
              return HttpUtility.HtmlDecode(encodedHtml);
            }
          }
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
    #endregion

  }
}