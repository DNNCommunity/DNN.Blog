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

using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Common;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Services
{

  public class ModulesController : DnnApiController
  {

    public class AddModuleDTO
    {
      public string paneName { get; set; }
      public int position { get; set; }
      public string title { get; set; }
      public string template { get; set; }
    }

    #region  Private Members 
    #endregion

    #region  Service Methods 
    [HttpPost()]
    [DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.Edit)]
    [ValidateAntiForgeryToken()]
    [ActionName("Add")]
    public HttpResponseMessage AddModule(AddModuleDTO postData)
    {

      var newSettings = new ViewSettings(ActiveModule.TabModuleID);
      newSettings.BlogModuleId = ActiveModule.ModuleID;
      newSettings.Template = postData.template;

      var objModule = new ModuleInfo();
      objModule.Initialize(PortalSettings.PortalId);
      objModule.PortalID = PortalSettings.PortalId;
      objModule.TabID = PortalSettings.ActiveTab.TabID;
      objModule.ModuleOrder = postData.position;
      if (string.IsNullOrEmpty(postData.title))
      {
        objModule.ModuleTitle = "Blog";
      }
      else
      {
        objModule.ModuleTitle = postData.title;
      }
      objModule.PaneName = postData.paneName;
      objModule.ModuleDefID = ActiveModule.ModuleDefID;
      objModule.InheritViewPermissions = true;
      objModule.AllTabs = false;
      objModule.Alignment = "";

      var objModules = new ModuleController();
      objModule.ModuleID = objModules.AddModule(objModule);
      newSettings.UpdateSettings(objModule.TabModuleID);

      return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });

    }

    [HttpGet()]
    [AllowAnonymous()]
    [ActionName("Manifest")]
    public HttpResponseMessage GetManifest()
    {

      var res = new HttpResponseMessage(HttpStatusCode.OK);

      using (var @out = new StringWriterWithEncoding(Encoding.UTF8))
      {
        using (var output = new XmlTextWriter(out))
        {
          var bs = ModuleSettings.GetModuleSettings(ActiveModule.ModuleID);

          output.Formatting = Formatting.Indented;
          output.WriteStartDocument();
          output.WriteStartElement("manifest");
          output.WriteAttributeString("xmlns", "http://schemas.microsoft.com/wlw/manifest/weblog");
          output.WriteStartElement("options");

          output.WriteElementString("clientType", "Metaweblog");
          output.WriteElementString("supportsMultipleCategories", bs.AllowMultipleCategories.ToYesNo());
          output.WriteElementString("supportsCategories", (bs.VocabularyId != -1).ToYesNo());
          output.WriteElementString("supportsCustomDate", "Yes");
          output.WriteElementString("supportsKeywords", "Yes");
          output.WriteElementString("supportsTrackbacks", "Yes");
          output.WriteElementString("supportsEmbeds", "No");
          output.WriteElementString("supportsAuthor", "No");
          output.WriteElementString("supportsExcerpt", (bs.SummaryModel == Common.Globals.SummaryType.PlainTextIndependent).ToYesNo());
          output.WriteElementString("supportsPassword", "No");
          output.WriteElementString("supportsPages", "No");
          output.WriteElementString("supportsPageParent", "No");
          output.WriteElementString("supportsPageOrder", "No");
          output.WriteElementString("supportsEmptyTitles", "No");
          output.WriteElementString("supportsExtendedEntries", (bs.SummaryModel == Common.Globals.SummaryType.HtmlPrecedesPost).ToYesNo());
          output.WriteElementString("supportsCommentPolicy", "No");
          output.WriteElementString("supportsPingPolicy", "No");
          output.WriteElementString("supportsPostAsDraft", "Yes");
          output.WriteElementString("supportsFileUpload", "Yes");
          output.WriteElementString("supportsSlug", "No");
          output.WriteElementString("supportsHierarchicalCategories", "Yes");
          output.WriteElementString("supportsCategoriesInline", "Yes");
          output.WriteElementString("supportsNewCategories", "No");
          output.WriteElementString("supportsNewCategoriesInline", "No");
          output.WriteElementString("requiresXHTML", "Yes");

          output.WriteEndElement(); // options
          output.WriteEndElement(); // manifest
          output.Flush();

        }

        res.Content = new StringContent(out.ToString(), Encoding.UTF8, "application/xml");

      }

      return res;

    }
    #endregion

    #region  Private Methods 
    #endregion

  }

  #region  Helper Class for Manifest Writing 
  public class StringWriterWithEncoding : System.IO.StringWriter
  {

    private Encoding _Encoding { get; set; }

    public StringWriterWithEncoding(Encoding enc) : base()
    {
      _Encoding = enc;
    }

    public override Encoding Encoding
    {
      get
      {
        return _Encoding;
      }
    }

  }
  #endregion

}