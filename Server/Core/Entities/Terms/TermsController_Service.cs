using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
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

using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Blog.Entities.Terms
{
  public partial class TermsController : DnnApiController
  {

    private const string DisallowedCharacters = @"%?*&;:'\\";

    #region  Service Methods 
    [HttpGet()]
    [DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.View)]
    [ActionName("Search")]
    public HttpResponseMessage Search()
    {
      var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
      string searchString = queryString["term"];
      int vocab = int.Parse(queryString["vocab"]);
      var colTerms = GetTermsByVocabulary(ActiveModule.ModuleID, vocab, System.Threading.Thread.CurrentThread.CurrentCulture.Name).Values.Where(t => t.LocalizedName.IndexOfAny(DisallowedCharacters.ToCharArray()) == -1 & t.LocalizedName.ToLower().Contains(searchString.ToLower())).Select(t => t.LocalizedName);
      return Request.CreateResponse(HttpStatusCode.OK, colTerms);
    }

    [HttpGet()]
    [DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.View)]
    [ActionName("VocabularyML")]
    public HttpResponseMessage GetVocabularyML(int vocabularyId)
    {
      var res = new List<TermML>();
      foreach (TermInfo t in GetTermsByVocabulary(ActiveModule.ModuleID, vocabularyId, "").Values)
        res.Add(new TermML() { TermID = t.TermId, DefaultName = t.Name, LocNames = t.NameLocalizations.GetDictionary() });
      return Request.CreateResponse(HttpStatusCode.OK, res);
    }
    #endregion

    public struct TermML
    {
      public int TermID;
      public string DefaultName;
      public Dictionary<string, string> LocNames;
    }

  }
}