// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Services.Journal;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.Blog.Core.Integration
{

    public class JournalController
    {

        #region  Public Methods 
        /// <summary>
        /// Informs the core journal that the user has posted a blog Post.
        /// </summary>
        /// <param name="objPost"></param>
        /// <param name="portalId"></param>
        /// <param name="tabId"></param>
        /// <param name="journalUserId"></param>
        /// <param name="url"></param>
        /// <remarks></remarks>
        public static void AddBlogPostToJournal(PostInfo objPost, int portalId, int tabId, int journalUserId, string url)
        {
            if (journalUserId == -1)
                return;
            string objectKey = Integration.ContentTypeName + "_" + Integration.ContentTypeName + "_" + string.Format("{0}:{1}", objPost.BlogID, objPost.ContentItemId);
            var ji = Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.GetJournalItemByKey(portalId, objectKey);

            if (ji != null)
            {
                Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.DeleteJournalItemByKey(portalId, objectKey);
            }

            ji = new JournalItem();

            ji.PortalId = portalId;
            ji.ProfileId = journalUserId;
            ji.UserId = journalUserId;
            ji.ContentItemId = objPost.ContentItemId;
            ji.Title = objPost.Title;
            ji.ItemData = new ItemData();
            ji.ItemData.Url = url;
            ji.Summary = HttpUtility.HtmlDecode(objPost.Summary);
            ji.Body = null;
            ji.JournalTypeId = GetBlogJournalTypeID(portalId);
            ji.ObjectKey = objectKey;
            ji.SecuritySet = "E,";

            var moduleInfo = Framework.ServiceLocator<IModuleController, ModuleController>.Instance.GetModule(objPost.ModuleID, tabId, false);
            Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.SaveJournalItem(ji, moduleInfo);
        }

        /// <summary>
        /// Deletes a journal item associated with the specified blog Post.
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="PostId"></param>
        /// <param name="portalId"></param>
        /// <remarks></remarks>
        public static void RemoveBlogPostFromJournal(int blogId, int PostId, int portalId)
        {
            string objectKey = Integration.ContentTypeName + "_" + Integration.ContentTypeName + "_" + string.Format("{0}:{1}", blogId, PostId);
            Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.DeleteJournalItemByKey(portalId, objectKey);
        }

        /// <summary>
        /// Informs the core journal that the user has commented on a blog Post.
        /// </summary>
        /// <param name="objPost"></param>
        /// <param name="objComment"></param>
        /// <param name="portalId"></param>
        /// <param name="tabId"></param>
        /// <param name="journalUserId"></param>
        /// <param name="url"></param>
        public static void AddOrUpdateCommentInJournal(BlogInfo objBlog, PostInfo objPost, Entities.Comments.CommentInfo objComment, int portalId, int tabId, int journalUserId, string url)
        {
            if (journalUserId == -1)
                return;
            string objectKey = Integration.ContentTypeName + "_" + Integration.JournalCommentTypeName + "_" + string.Format("{0}:{1}", objPost.ContentItemId.ToString(), objComment.CommentID.ToString());
            var ji = Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.GetJournalItemByKey(portalId, objectKey);
            if (ji != null)
            {
                Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.DeleteJournalItemByKey(portalId, objectKey);
            }

            ji = new JournalItem();

            ji.PortalId = portalId;
            ji.ProfileId = journalUserId;
            ji.UserId = journalUserId;
            ji.ContentItemId = objPost.ContentItemId;
            ji.Title = objPost.Title;
            ji.ItemData = new ItemData();
            ji.ItemData.Url = url;
            ji.Summary = HttpUtility.HtmlDecode(objComment.Comment);
            ji.Body = null;
            ji.JournalTypeId = GetCommentJournalTypeID(portalId);
            ji.ObjectKey = objectKey;
            ji.SecuritySet = "E,";

            var moduleInfo = Framework.ServiceLocator<IModuleController, ModuleController>.Instance.GetModule(objPost.ModuleID, tabId, false);
            Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.SaveJournalItem(ji, moduleInfo);

            if (objBlog.OwnerUserId != journalUserId)
            {
                string title = DotNetNuke.Services.Localization.Localization.GetString("CommentAddedNotify", Globals.SharedResourceFileName);
                string summary = "<a target='_blank' href='" + url + "'>" + objPost.Title + "</a>";
                NotificationController.CommentAdded(objComment, objPost, objBlog, portalId, summary, title);
            }

        }

        /// <summary>
        /// Deletes a journal item associated with the specific comment.
        /// </summary>
        /// <param name="PostId"></param>
        /// <param name="commentId"></param>
        /// <param name="portalId"></param>
        public static void RemoveCommentFromJournal(int PostId, int commentId, int portalId)
        {
            string objectKey = Integration.ContentTypeName + "_" + Integration.JournalCommentTypeName + "_" + string.Format("{0}:{1}", PostId, commentId);
            Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.DeleteJournalItemByKey(portalId, objectKey);
        }
        #endregion

        #region  Private Methods 
        /// <summary>
        /// Returns a journal type associated with blog Posts (using one of the core built in journal types)
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static int GetBlogJournalTypeID(int portalId)
        {
            IEnumerable<JournalTypeInfo> colJournalTypes;
            colJournalTypes = (from t in Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.GetJournalTypes(portalId)
                               where t.JournalType == Integration.JournalBlogTypeName
                               select t);
            int journalTypeId;

            if (colJournalTypes.Count() > 0)
            {
                var journalType = colJournalTypes.Single();
                journalTypeId = journalType.JournalTypeId;
            }
            else
            {
                journalTypeId = 7;
            }

            return journalTypeId;
        }

        /// <summary>
        /// Returns a journal type associated with commenting (using one of the core built in journal types)
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static int GetCommentJournalTypeID(int portalId)
        {
            IEnumerable<JournalTypeInfo> colJournalTypes;
            colJournalTypes = (from t in Framework.ServiceLocator<IJournalController, DotNetNuke.Services.Journal.JournalController>.Instance.GetJournalTypes(portalId)
                               where t.JournalType == Integration.JournalCommentTypeName
                               select t);
            int journalTypeId;

            if (colJournalTypes.Count() > 0)
            {
                var journalType = colJournalTypes.Single();
                journalTypeId = journalType.JournalTypeId;
            }
            else
            {
                journalTypeId = 18;
            }

            return journalTypeId;
        }
        #endregion

    }

}