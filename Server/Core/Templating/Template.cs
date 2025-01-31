using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
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

namespace DotNetNuke.Modules.Blog.Templating
{
  public class Template
  {

    #region  Private Members 
    public string FileName { get; set; } = "Template.html";
    public string Contents { get; set; } = "";
    public string ViewMapPath { get; set; } = "";
    public string ViewRelPath { get; set; } = "";
    public GenericTokenReplace Replacer { get; set; } = null;
    #endregion

    #region  Events 
    private void Template_GetData(string DataSource, Dictionary<string, string> Parameters, ref List<GenericTokenReplace> Replacers, ref List<string[]> Arguments, object callingObject)
    {
      GetData?.Invoke(DataSource, Parameters, ref Replacers, ref Arguments, callingObject);
    }

    public event GetDataEventHandler GetData;

    public delegate void GetDataEventHandler(string DataSource, Dictionary<string, string> Parameters, ref List<GenericTokenReplace> Replacers, ref List<string[]> Arguments, object callingObject);
    #endregion

    #region  Constructors 
    public Template(string ViewMapPath, string ViewRelPath, string Filename, GenericTokenReplace Replacer, TemplateRepeaterItem item)
    {
      this.ViewMapPath = ViewMapPath;
      this.ViewRelPath = ViewRelPath;
      FileName = Filename;
      this.Replacer = Replacer;
      this.Replacer.AddResources(ViewRelPath + "App_LocalResources/" + Filename);
      if (item is not null)
        this.Replacer.AddPropertySource("item", item);
    }

    public Template(string ViewMapPath, string ViewRelPath, string Filename, GenericTokenReplace Replacer, TemplateRepeaterItem item, string[] Arguments)
    {
      this.ViewMapPath = ViewMapPath;
      this.ViewRelPath = ViewRelPath;
      FileName = Filename;
      this.Replacer = Replacer;
      this.Replacer.AddCustomParameters(Arguments);
      this.Replacer.AddResources(ViewRelPath + "App_LocalResources/" + Filename);
      if (item is not null)
        this.Replacer.AddPropertySource("item", item);
    }
    #endregion

    #region  Public Methods 
    public string ReplaceContents()
    {

      try
      {
        Contents = Templating.GetTemplateFile(ViewMapPath + FileName);
        if (string.IsNullOrEmpty(Contents))
          return "";
        // Expand subtemplates
        // Simple conditional template e.g. [subtemplate|Widget.html|widget:isgood|True]
        Contents = Regex.Replace(Contents, @"(?i)\[subtemplate\|([^|\]]+)\|([^:|\]]+):([^|\]]+)\|?([^|\]]+)?\](?-i)", ReplaceConditionalTemplate);
        // Inline conditional e.g. [if|2][flight:flightid][>]4[/if] ... [endif|2]
        Contents = Regex.Replace(Contents, @"(?si)\[if\|(?<template>[^|\]]+)\](?<left>.*?)\[(?<comparison>\W{1,2})\](?<right>.*?)\[/if\](?<content>.*)\[endif\|\1\](?-is)", ReplaceIfThens);
        // Simple repeating template e.g. [subtemplate|Flight.html|flights|pagesize=6]
        Contents = Regex.Replace(Contents, @"(?i)\[subtemplate\|([^|\]]+)\|([^|\]]+)\|?([^|\]]+)?\](?-i)", ReplaceSubtemplates);
        if (Replacer is not null)
        {
          return Replacer.ReplaceTokens(Contents);
        }
        else
        {
          return Contents;
        }
      }
      catch (Exception ex)
      {
        return string.Format("<p>Error: {0}</p><p>In: {1}</p>", ex.Message, ex.StackTrace);
      }

    }
    #endregion

    #region  Private Methods 
    private string ReplaceConditionalTemplate(Match m)
    {

      string @file = m.Groups[1].Value.ToLower();
      string conditionObject = m.Groups[2].Value.ToLower();
      string conditionProperty = m.Groups[3].Value.ToLower();
      string shouldRender = Replacer.GetTokenValue(conditionObject, conditionProperty, "");
      if (string.IsNullOrEmpty(shouldRender))
        return "";
      shouldRender = shouldRender.ToLower();
      string compareValue = "";
      if (m.Groups[4] is not null && !string.IsNullOrEmpty(m.Groups[4].Value))
      {
        compareValue = m.Groups[4].Value.ToLower();
        if ((shouldRender ?? "") != (compareValue ?? ""))
        {
          return "";
        }
      }
      else
      {
        switch (shouldRender ?? "")
        {
          case "false":
          case "no":
          case "0":
            {
              return "";
            }
        }
      }

      var t = new Template(ViewMapPath, ViewRelPath, @file, Replacer, null);
      t.GetData += Template_GetData;
      return t.ReplaceContents();

    }

    private string ReplaceSubtemplates(Match m)
    {

      string @file = m.Groups[1].Value.ToLower();
      string datasource = m.Groups[2].Value.ToLower();
      string[] properties = Array.Empty<string>();
      if (m.Groups[3] is not null && !string.IsNullOrEmpty(m.Groups[3].Value))
      {
        properties = m.Groups[3].Value.Split(',');
      }
      var @params = new Dictionary<string, string>();
      foreach (string p in properties)
      {
        if (p.IndexOf('=') > -1)
        {
          @params.Add(Strings.Left(p, p.IndexOf('=')).ToLower(), Strings.Mid(p, p.IndexOf('=') + 2));
        }
        else
        {
          @params.Add(p.ToLower(), "");
        }
      }

      var dataSrc = new List<GenericTokenReplace>();
      var args = new List<string[]>();
      GetData?.Invoke(datasource, @params, ref dataSrc, ref args, Replacer.PrimaryObject);

      var res = new StringBuilder();
      int totalItems = dataSrc.Count;
      if (totalItems == 1 & args.Count > 0)
      {
        foreach (string[] arg in args)
        {
          var tri = new TemplateRepeaterItem(totalItems, 1);
          var t = new Template(ViewMapPath, ViewRelPath, @file, dataSrc[0], tri, arg);
          t.GetData += Template_GetData;
          res.Append(t.ReplaceContents());
        }
      }
      else if (args.Count > 0)
      {
        string[] arg = args[0];
        int i = 1;
        foreach (GenericTokenReplace d in dataSrc)
        {
          var tri = new TemplateRepeaterItem(totalItems, i);
          var t = new Template(ViewMapPath, ViewRelPath, @file, d, tri, arg);
          t.GetData += Template_GetData;
          res.Append(t.ReplaceContents());
          i += 1;
        }
      }
      else
      {
        int i = 1;
        foreach (GenericTokenReplace d in dataSrc)
        {
          var tri = new TemplateRepeaterItem(totalItems, i);
          var t = new Template(ViewMapPath, ViewRelPath, @file, d, tri);
          t.GetData += Template_GetData;
          res.Append(t.ReplaceContents());
          i += 1;
        }
      }
      return res.ToString();

    }

    private string ReplaceIfThens(Match m)
    {

      bool result = false;
      string inlineContents = "";
      string templateFilename = "";
      if (m.Groups["content"].Success)
      {
        inlineContents = m.Groups["content"].Value;
      }
      else
      {
        templateFilename = m.Groups["template"].Value.ToLower();
      }

      string leftside = Replacer.ReplaceTokens(m.Groups["left"].Value).ToLower();
      if (m.Groups["comparison"].Success)
      {
        string comparison = m.Groups["comparison"].Value.ToLower();
        string rightside = Replacer.ReplaceTokens(m.Groups["right"].Value.ToLower());
        switch (comparison ?? "")
        {
          case "=":
            {
              result = (leftside ?? "") == (rightside ?? "");
              break;
            }
          case "<":
            {
              if (Information.IsNumeric(leftside) & Information.IsNumeric(rightside))
              {
                result = float.Parse(leftside) < float.Parse(rightside);
              }
              else
              {
                result = Operators.CompareString(leftside, rightside, false) < 0;
              }

              break;
            }
          case "<=":
            {
              if (Information.IsNumeric(leftside) & Information.IsNumeric(rightside))
              {
                result = float.Parse(leftside) <= float.Parse(rightside);
              }
              else
              {
                result = Operators.CompareString(leftside, rightside, false) <= 0;
              }

              break;
            }
          case ">=":
            {
              if (Information.IsNumeric(leftside) & Information.IsNumeric(rightside))
              {
                result = float.Parse(leftside) >= float.Parse(rightside);
              }
              else
              {
                result = Operators.CompareString(leftside, rightside, false) >= 0;
              }

              break;
            }
          case ">":
            {
              if (Information.IsNumeric(leftside) & Information.IsNumeric(rightside))
              {
                result = float.Parse(leftside) > float.Parse(rightside);
              }
              else
              {
                result = Operators.CompareString(leftside, rightside, false) > 0;
              }

              break;
            }
          case "!=":
          case "<>":
            {
              if (Information.IsNumeric(leftside) & Information.IsNumeric(rightside))
              {
                result = float.Parse(leftside) != float.Parse(rightside);
              }
              else
              {
                result = (leftside ?? "") != (rightside ?? "");
              }

              break;
            }

          default:
            {
              break;
            }
        }
      }
      else
      {
        result = Conversions.ToBoolean(leftside);
      }

      if (result)
      {
        return inlineContents;
      }
      else
      {
        return "";
      }

    }
    #endregion

  }
}