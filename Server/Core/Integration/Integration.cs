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
using DotNetNuke.Entities.Content.Taxonomy;

namespace DotNetNuke.Modules.Blog.Integration
{
  public class Integration
  {

    public const string ContentTypeName = "DNN_Blog_Post";
    public const string JournalBlogTypeName = "blog";
    public const string JournalCommentTypeName = "comment";
    public const string NotificationPublishingTypeName = "DNN_Blog_Post_Publishing";
    public const string NotificationCommentApprovalTypeName = "DNN_Blog_Post_CommentApproval";
    public const string NotificationCommentReportedTypeName = "DNN_Blog_Post_CommentReported";
    public const string NotificationCommentAddedTypeName = "DNN_Blog_Post_CommentAdded";

    public static Vocabulary CreateNewVocabulary(int portalId)
    {

      string name = "Blog Categories";
      var cntScope = new ScopeTypeController();
      var cntVocabulary = new VocabularyController();
      int i = 1;
      while (cntVocabulary.GetVocabularies().Where(v => v.Name == name).Count() > 0)
        name = "Blog Categories " + i.ToString();
      var objScope = cntScope.GetScopeTypes().Where(s => s.ScopeType == "Portal").SingleOrDefault();
      var objVocab = new Vocabulary();
      objVocab.Name = name;
      objVocab.IsSystem = false;
      objVocab.Weight = 0;
      objVocab.Description = "Automatically generated for blog module.";
      objVocab.ScopeId = portalId;
      objVocab.ScopeTypeId = objScope.ScopeTypeId;
      objVocab.Type = VocabularyType.Hierarchy;
      objVocab.VocabularyId = cntVocabulary.AddVocabulary(objVocab);
      return objVocab;

    }

  }
}