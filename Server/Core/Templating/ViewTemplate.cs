using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
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

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  public class ViewTemplate : UserControl
  {

    #region  Properties 
    public Template Template { get; set; }
    public string TemplatePath { get; set; } = "";
    public string TemplateRelPath { get; set; } = "";
    public string TemplateMapPath { get; set; } = "";
    public GenericTokenReplace DefaultReplacer { get; set; }
    public string StartTemplate { get; set; } = "Template.html";
    private string ViewPath { get; set; } = "";
    private string ViewMapPath { get; set; } = "";

    public ViewTemplate()
    {
      PreRender += Page_PreRender;
    }
    #endregion

    #region  Public Methods 
    public string GetContentsAsString()
    {
      var sb = new StringBuilder();
      using (var tw = new System.IO.StringWriter(sb))
      {
        using (var w = new HtmlTextWriter(tw))
        {
          Render(w);
          w.Flush();
        }
      }
      return sb.ToString();
    }
    #endregion

    #region  Overrides 
    protected override void Render(HtmlTextWriter writer)
    {
      if (Template != null)
      {
        writer.Write(Template.ReplaceContents());
      }
    }

    public override void DataBind()
    {

      ViewMapPath = TemplateMapPath;
      ViewPath = TemplatePath;

      var dataSrc = new List<GenericTokenReplace>();
      var args = new List<string[]>();
      var @params = new Dictionary<string, string>();

      GetData?.Invoke("", @params, ref dataSrc, ref args, null);
      if (dataSrc.Count > 0)
      {
        Template = new Template(ViewMapPath, TemplateRelPath, StartTemplate, dataSrc[0], null);
      }
      else
      {
        Template = new Template(ViewMapPath, TemplateRelPath, StartTemplate, DefaultReplacer, null);
      }
      Template.GetData += Template_GetData;

    }
    #endregion

    #region  Events 
    private void Template_GetData(string DataSource, Dictionary<string, string> Parameters, ref List<GenericTokenReplace> Replacers, ref List<string[]> Arguments, object callingObject)
    {
      GetData?.Invoke(DataSource, Parameters, ref Replacers, ref Arguments, callingObject);
    }

    public event GetDataEventHandler GetData;

    public delegate void GetDataEventHandler(string DataSource, Dictionary<string, string> Parameters, ref List<GenericTokenReplace> Replacers, ref List<string[]> Arguments, object callingObject);
    #endregion

    #region  Event Handlers 
    private void Page_PreRender(object sender, EventArgs e)
    {
      string path = string.IsNullOrEmpty(ViewPath) ? TemplatePath : ViewPath;
      string mapPath = string.IsNullOrEmpty(ViewMapPath) ? TemplateMapPath : ViewMapPath;
      TemplateManager.RegisterClientAssets(Page, path, mapPath, DefaultReplacer);
    }
    #endregion

  }
}