using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Core.Common;
using DotNetNuke.Modules.Blog.Core.Entities.Blogs;
using DotNetNuke.Modules.Blog.Core.Entities.Comments;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Core.Templating;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Blog.Services
{
    /// <summary>
    /// Ports Blog.ascx.vb template GetData / EnsurePostList for MVC hosting.
    /// </summary>
    public class BlogTemplateDataSource
    {
        private readonly ModuleInfo _module;
        private readonly BlogContextInfo _blogContext;
        private readonly ModuleSettings _settings;
        private readonly ViewSettings _viewSettings;
        private readonly int _userId;
        private readonly Func<string, string> _localize;

        private int _pageSize = -1;
        private int _totalRecords;
        private int _reqPage = 1;
        private IEnumerable<PostInfo> _postList;

        public BlogTemplateDataSource(
            ModuleInfo module,
            BlogContextInfo blogContext,
            ModuleSettings settings,
            ViewSettings viewSettings,
            int userId,
            Func<string, string> localize = null)
        {
            _module = module;
            _blogContext = blogContext;
            _settings = settings;
            _viewSettings = viewSettings;
            _userId = userId;
            _localize = localize ?? (k => Localization.GetString(k, Globals.SharedResourceFileName));
            _reqPage = HttpContext.Current.Request.Params.ReadValue("Page", 1);
            _pageSize = viewSettings.TemplateSettings.ContainsKey("pagesize")
                ? viewSettings.TemplateSettings.ReadValue("pagesize", _pageSize)
                : _pageSize;
        }

        public void RegisterClientAssets(System.Web.UI.Page page)
        {
            if (page == null)
            {
                return;
            }
            var tmgr = new TemplateManager(PortalSettings.Current, _viewSettings.Template);
            var defaultReplacer = new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings);
            tmgr.RegisterClientAssets(page, defaultReplacer);
        }

        public string Render(string startTemplate = "Template.html")
        {
            var tmgr = new TemplateManager(PortalSettings.Current, _viewSettings.Template);
            var vt = new ViewTemplate
            {
                TemplatePath = tmgr.TemplatePath,
                TemplateRelPath = tmgr.TemplateRelPath,
                TemplateMapPath = tmgr.TemplateMapPath,
                StartTemplate = startTemplate,
                DefaultReplacer = new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings)
            };
            vt.GetData += OnGetData;
            vt.DataBind();
            return vt.GetContentsAsString();
        }

        public void OnGetData(string DataSource, Dictionary<string, string> Parameters,
            ref List<GenericTokenReplace> Replacers, ref List<string[]> Arguments, object callingObject)
        {
            switch ((DataSource ?? "").ToLowerInvariant())
            {
                case "blogs":
                    HandleBlogs(Parameters, Replacers);
                    break;
                case "posts":
                    _pageSize = Parameters.ReadValue("pagesize", _pageSize);
                    EnsurePostList(_pageSize);
                    foreach (var e in _postList)
                    {
                        e.ParentTabID = ResolveParentTabId();
                        Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, e));
                    }
                    break;
                case "postspager":
                    HandlePager(Parameters, Replacers, Arguments);
                    break;
                case "terms":
                    HandleTerms(callingObject, Replacers);
                    break;
                case "allterms":
                    foreach (var t in TermsController.GetTermsByModule(_blogContext.BlogModuleId, _blogContext.Locale))
                        Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, null, t));
                    break;
                case "keywords":
                case "tags":
                    HandleTags(callingObject, Replacers, false);
                    break;
                case "allkeywords":
                case "alltags":
                    HandleTags(null, Replacers, true);
                    break;
                case "categories":
                    HandleCategories(callingObject, Replacers);
                    break;
                case "allcategories":
                    foreach (var t in TermsController.GetTermsByVocabulary(_blogContext.BlogModuleId, _settings.VocabularyId, _blogContext.Locale).Values)
                    {
                        t.ParentTabID = ResolveParentTabId();
                        Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, null, t));
                    }
                    break;
                case "selectcategories":
                    foreach (var t in TermsController.GetTermsByModule(_blogContext.BlogModuleId, _blogContext.Locale)
                                 .Where(x => _viewSettings.CategoryList.Contains(x.TermId)))
                    {
                        t.ParentTabID = ResolveParentTabId();
                        Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, null, t));
                    }
                    break;
                case "comments":
                    HandleComments(Parameters, callingObject, Replacers, false);
                    break;
                case "allcomments":
                    HandleComments(Parameters, callingObject, Replacers, true);
                    break;
                case "calendar":
                case "blogcalendar":
                    foreach (var bci in BlogsController.GetBlogCalendar(_blogContext.BlogModuleId, _blogContext.BlogId, _blogContext.ShowLocale))
                    {
                        bci.ParentTabID = ResolveParentTabId();
                        Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, bci));
                    }
                    break;
                case "authors":
                case "allauthors":
                    HandleAuthors(DataSource, Parameters, Replacers);
                    break;
            }
        }

        private void HandleBlogs(Dictionary<string, string> Parameters, List<GenericTokenReplace> Replacers)
        {
            var blogList = BlogsController.GetBlogsByModule(_blogContext.BlogModuleId, _userId, _blogContext.Locale).Values
                .Where(b => b.Published).OrderBy(b => b.Title);
            _pageSize = Parameters.ReadValue("pagesize", _pageSize);
            if (_pageSize > 0)
            {
                int startRec = ((_reqPage - 1) * _pageSize) + 1;
                int endRec = _reqPage * _pageSize;
                int i = 1;
                foreach (var b in blogList)
                {
                    if (i >= startRec && i <= endRec)
                    {
                        b.ParentTabID = ResolveParentTabId();
                        Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, b));
                    }
                    i++;
                }
            }
            else
            {
                foreach (var b in blogList)
                {
                    b.ParentTabID = ResolveParentTabId();
                    Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, b));
                }
            }
        }

        private void HandlePager(Dictionary<string, string> Parameters, List<GenericTokenReplace> Replacers, List<string[]> Arguments)
        {
            _pageSize = Parameters.ReadValue("pagesize", _pageSize);
            EnsurePostList(_pageSize);
            string pagerType = Parameters.ReadValue("pagertype", "allpages");
            int remdr;
            int nrPages = Math.DivRem(_totalRecords, _pageSize < 1 ? 10 : _pageSize, out remdr);
            if (remdr > 0) nrPages++;
            if (nrPages < 2) return;

            Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings));
            switch (pagerType.ToLowerInvariant())
            {
                case "allpages":
                    for (int i = 1; i <= nrPages; i++)
                        Arguments.Add(new[] { "page=" + i, "pageiscurrent=" + (i == _reqPage), "pagename=" + i, "pagetype=number" });
                    break;
                case "somepages":
                    if (_reqPage > 3)
                        Arguments.Add(new[] { "page=1", "pageiscurrent=False", "pagename=1", "pagetype=firstpage" });
                    for (int i = Math.Max(_reqPage - 2, 1); i <= Math.Min(_reqPage + 2, nrPages); i++)
                        Arguments.Add(new[] { "page=" + i, "pageiscurrent=" + (i == _reqPage), "pagename=" + i, "pagetype=number" });
                    if (_reqPage < nrPages - 3)
                        Arguments.Add(new[] { "page=" + nrPages, "pageiscurrent=False", "pagename=" + nrPages, "pagetype=lastpage" });
                    break;
                case "newerolder":
                    if (_reqPage > 1)
                        Arguments.Add(new[] { "page=" + (_reqPage - 1), "pageiscurrent=False", "pagename=" + _localize("Newer"), "pagetype=previous" });
                    if (_reqPage < nrPages)
                        Arguments.Add(new[] { "page=" + (_reqPage + 1), "pageiscurrent=False", "pagename=" + _localize("Older"), "pagetype=next" });
                    break;
            }
        }

        private void HandleTerms(object callingObject, List<GenericTokenReplace> Replacers)
        {
            IEnumerable<TermInfo> terms;
            if (callingObject is PostInfo post)
                terms = post.Terms;
            else if (_blogContext.Post != null)
                terms = _blogContext.Post.Terms;
            else
                terms = TermsController.GetTermsByModule(_blogContext.BlogModuleId, _blogContext.Locale);

            foreach (var t in terms)
                Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, _blogContext.Post, t));
        }

        private void HandleTags(object callingObject, List<GenericTokenReplace> Replacers, bool all)
        {
            IEnumerable<TermInfo> tags;
            if (!all && callingObject is PostInfo post)
                tags = post.PostTags;
            else if (!all && _blogContext.Post != null)
                tags = _blogContext.Post.PostTags;
            else
                tags = TermsController.GetTermsByModule(_blogContext.BlogModuleId, _blogContext.Locale).Where(x => x.VocabularyId == 1);

            foreach (var t in tags)
            {
                t.ParentTabID = ResolveParentTabId();
                Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, all ? null : _blogContext.Post, t));
            }
        }

        private void HandleCategories(object callingObject, List<GenericTokenReplace> Replacers)
        {
            IEnumerable<TermInfo> cats;
            if (callingObject is PostInfo post)
                cats = post.PostCategories;
            else if (_blogContext.Post != null)
                cats = _blogContext.Post.PostCategories;
            else if (string.IsNullOrEmpty(_viewSettings.Categories))
                cats = TermsController.GetTermsByModule(_blogContext.BlogModuleId, _blogContext.Locale).Where(x => x.VocabularyId != 1);
            else
                cats = TermsController.GetTermsByModule(_blogContext.BlogModuleId, _blogContext.Locale)
                    .Where(x => _viewSettings.CategoryList.Contains(x.VocabularyId));

            foreach (var t in cats)
                Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, _blogContext.Post, t));
        }

        private void HandleComments(Dictionary<string, string> Parameters, object callingObject, List<GenericTokenReplace> Replacers, bool all)
        {
            if (!all && callingObject is PostInfo post)
            {
                foreach (var c in CommentsController.GetCommentsByContentItem(post.ContentItemId, false, _userId))
                    Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, _blogContext.Post, c));
                return;
            }
            if (!all && _blogContext.Post != null)
            {
                foreach (var c in CommentsController.GetCommentsByContentItem(_blogContext.Post.ContentItemId, false, _userId))
                    Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, _blogContext.Post, c));
                return;
            }

            _pageSize = Parameters.ReadValue("pagesize", _pageSize);
            if (_pageSize < 1) _pageSize = 10;
            bool loadPosts = Parameters.ReadValue("loadposts", false);
            foreach (var c in CommentsController.GetCommentsByModule(_blogContext.BlogModuleId, _userId, _reqPage - 1, _pageSize, "CREATEDONDATE DESC", ref _totalRecords).Values)
            {
                if (loadPosts)
                    Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings,
                        PostsController.GetPost(c.ContentItemId, _blogContext.BlogModuleId, _blogContext.Locale), c));
                else
                    Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, null, c));
            }
        }

        private void HandleAuthors(string DataSource, Dictionary<string, string> Parameters, List<GenericTokenReplace> Replacers)
        {
            int blogToShow = DataSource.Equals("allauthors", StringComparison.OrdinalIgnoreCase) ? -1 : _blogContext.BlogId;
            string sort = Parameters.ReadValue("sort", "");
            IEnumerable<PostAuthor> authors = PostsController.GetAuthors(_blogContext.BlogModuleId, blogToShow);
            switch (sort.ToLowerInvariant())
            {
                case "username": authors = authors.OrderBy(t => t.Username); break;
                case "email": authors = authors.OrderBy(t => t.Email); break;
                case "firstname": authors = authors.OrderBy(t => t.FirstName); break;
                case "displayname": authors = authors.OrderBy(t => t.DisplayName); break;
            }
            foreach (var u in authors)
            {
                u.ParentTabID = ResolveParentTabId();
                Replacers.Add(new BlogTokenReplace(_module, _blogContext, _settings, _viewSettings, new LazyLoadingUser(u)));
            }
        }

        /// <summary>
        /// Tab that post/term/author parenturl tokens should target.
        /// Widget instances use the configured parent blog module's tab; otherwise the current module's tab.
        /// </summary>
        private int ResolveParentTabId()
        {
            return _blogContext.ParentModule != null ? _blogContext.ParentModule.TabID : _module.TabID;
        }

        private void EnsurePostList(int pageSize)
        {
            if (_postList != null) return;
            if (pageSize < 1) pageSize = 10;

            int publishValue = 1;
            if (!string.IsNullOrEmpty(_blogContext.SearchString))
            {
                if (_blogContext.SearchUnpublished) publishValue = -1;
                if (string.IsNullOrEmpty(_blogContext.Categories))
                {
                    if (_blogContext.Term == null)
                        _postList = PostsController.SearchPosts(_settings.ModuleId, _blogContext.BlogId, _blogContext.Locale, _blogContext.SearchString, _blogContext.SearchTitle, _blogContext.SearchContents, publishValue, _blogContext.ShowLocale, _blogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", ref _totalRecords, _userId, _blogContext.Security.UserIsAdmin).Values;
                    else
                        _postList = PostsController.SearchPostsByTerm(_settings.ModuleId, _blogContext.BlogId, _blogContext.Locale, _blogContext.TermId, _blogContext.SearchString, _blogContext.SearchTitle, _blogContext.SearchContents, publishValue, _blogContext.ShowLocale, _blogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", ref _totalRecords, _userId, _blogContext.Security.UserIsAdmin).Values;
                }
                else
                    _postList = PostsController.SearchPostsByCategory(_settings.ModuleId, _blogContext.BlogId, _blogContext.Locale, _blogContext.Categories, _blogContext.SearchString, _blogContext.SearchTitle, _blogContext.SearchContents, publishValue, _blogContext.ShowLocale, _blogContext.EndDate, -1, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", ref _totalRecords, _userId, _blogContext.Security.UserIsAdmin).Values;
            }
            else if (string.IsNullOrEmpty(_blogContext.Categories))
            {
                publishValue = -1;
                var ps = PortalSettings.Current;
                if (_viewSettings.HideUnpublishedBlogsViewMode && ps.UserMode == PortalSettings.Mode.View) publishValue = 1;
                if (_viewSettings.HideUnpublishedBlogsEditMode && ps.UserMode == PortalSettings.Mode.Edit) publishValue = 1;
                if (_blogContext.Term == null)
                    _postList = PostsController.GetPosts(_settings.ModuleId, _blogContext.BlogId, _blogContext.Locale, publishValue, _blogContext.ShowLocale, _blogContext.EndDate, _blogContext.AuthorId, false, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", ref _totalRecords, _userId, _blogContext.Security.UserIsAdmin).Values;
                else
                    _postList = PostsController.GetPostsByTerm(_settings.ModuleId, _blogContext.BlogId, _blogContext.Locale, _blogContext.TermId, publishValue, _blogContext.ShowLocale, _blogContext.EndDate, _blogContext.AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", ref _totalRecords, _userId, _blogContext.Security.UserIsAdmin).Values;
            }
            else
            {
                publishValue = -1;
                var ps = PortalSettings.Current;
                if (_viewSettings.HideUnpublishedBlogsViewMode && ps.UserMode == PortalSettings.Mode.View) publishValue = 1;
                if (_viewSettings.HideUnpublishedBlogsEditMode && ps.UserMode == PortalSettings.Mode.Edit) publishValue = 1;
                _postList = PostsController.GetPostsByCategory(_settings.ModuleId, _blogContext.BlogId, _blogContext.Locale, _blogContext.Categories, publishValue, _blogContext.ShowLocale, _blogContext.EndDate, _blogContext.AuthorId, _reqPage - 1, pageSize, "PUBLISHEDONDATE DESC", ref _totalRecords, _userId, _blogContext.Security.UserIsAdmin).Values;
            }
        }
    }
}
