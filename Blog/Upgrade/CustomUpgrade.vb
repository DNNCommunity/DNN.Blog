'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.IO
Imports DotNetNuke.Modules.Blog.Providers.Data
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Modules.Blog.Components.Business
Imports DotNetNuke.Modules.Blog.Components.Controllers
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Modules.Blog.Components.Entities
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Modules.Blog.Components.Integration
Imports System.Linq
Imports DotNetNuke.Common.Utilities

Public Class CustomUpgrade

#Region "Forum/Blog Import"

    Public Function ImportForumBlog(ByVal GroupID As Integer, ByVal BlogID As Integer, ByVal ModuleID As Integer) As Boolean
        Try
            Dim m_PortalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim m_Forum_GroupsController As New Forum_GroupsController
            Dim m_Forum_ForumsController As New Forum_ForumsController
            Dim m_ForumThreadsController As New ForumThreadsController
            Dim m_Forum_PostsController As New Forum_PostsController
            Dim m_Forum_ThreadRatingController As New Forum_ThreadRatingController
            Dim m_BlogController As New BlogController
            Dim m_EntryController As New EntryController
            Dim m_CommentController As New CommentController

            Dim ForumGroup As New Forum_GroupsInfo
            Dim ForumList As New ArrayList
            ForumGroup = m_Forum_GroupsController.GetByGroupID(GroupID)
            Dim m_BlogInfo As BlogInfo = m_BlogController.GetBlog(BlogID)
            ForumList = m_Forum_ForumsController.List(GroupID)

            For Each fi As Forum_ForumsInfo In ForumList
                If IsNothing(fi.Description) Or fi.Description = "" Then
                    m_BlogInfo.Description = fi.Name
                Else
                    m_BlogInfo.Description = fi.Description
                End If
                m_BlogInfo.Title = fi.Name
                m_BlogInfo.ParentBlogID = BlogID
                Dim NewBlogID As Integer = m_BlogController.AddBlog(m_BlogInfo)
                Dim ThreadList As New ArrayList
                ThreadList = m_ForumThreadsController.List(fi.ForumID)
                For Each ti As Forum_ThreadsInfo In ThreadList
                    Dim PostList As New ArrayList
                    PostList = m_Forum_PostsController.List(ti.ThreadID)
                    Dim m_EntryInfo As EntryInfo = New EntryInfo
                    m_EntryInfo.BlogID = NewBlogID

                    For Each pi As Forum_PostsInfo In PostList
                        If m_EntryInfo.AddedDate < pi.CreatedDate Then
                            m_EntryInfo.AddedDate = pi.CreatedDate
                        End If
                        If m_EntryInfo.Title = "" Then
                            m_EntryInfo.Title = pi.Subject
                        End If
                        m_EntryInfo.Description += pi.Subject & System.Environment.NewLine
                        If pi.Body <> "" Then
                            m_EntryInfo.Entry += "" & pi.Body & System.Environment.NewLine
                        End If
                    Next
                    If m_EntryInfo.Entry = "" Then
                        m_EntryInfo.Entry += "No Content"
                    End If
                    m_EntryInfo.Published = True
                    m_EntryInfo.UserFullName = m_BlogInfo.UserFullName
                    m_EntryInfo.UserID = m_BlogInfo.UserID
                    m_EntryInfo.UserName = m_BlogInfo.UserName
                    m_EntryInfo.ModuleID = ModuleID

                    Dim EntryID As Integer = m_EntryController.AddEntry(m_EntryInfo, -1).EntryID
                    Dim CommentList As New ArrayList
                    CommentList = m_Forum_ThreadRatingController.List(ti.ThreadID)
                    For Each ci As Forum_ThreadRatingInfo In CommentList
                        Dim m_CommentInfo As New CommentInfo
                        m_CommentInfo.AddedDate = ti.PinnedDate
                        m_CommentInfo.Comment = ci.Comment
                        m_CommentInfo.EntryID = EntryID
                        m_CommentInfo.Title = m_EntryInfo.Title
                        m_CommentInfo.UserID = ci.UserID
                        m_CommentController.AddComment(m_CommentInfo)
                    Next
                Next
            Next
            Return True
        Catch ex As Exception
            Exceptions.LogException(ex)
            Return False
        End Try
    End Function

#End Region

#Region "Forum/Blog Upgrade"

    Public Function UpgradeForumBlog() As String
        Dim _return As String = ""
        Try
            Dim _desktopModuleController As New DesktopModuleController
            Dim _desktopModuleInfo As DesktopModuleInfo
            _desktopModuleInfo = _desktopModuleController.GetDesktopModuleByModuleName("DNN_Blog")
            If Not IsNothing(_desktopModuleInfo) Then         ' old blog is installed
                _desktopModuleInfo.FriendlyName = "Blog (old)"
                _desktopModuleController.UpdateDesktopModule(_desktopModuleInfo)
                _return = "Forum/Blog renamed"
            End If
        Catch ex As Exception
            Exceptions.LogException(ex)
        End Try
        Return _return
    End Function

#End Region

#Region "NewBlog Cross-Grade"

    Public Function UpgradeNewBlog() As String
        Try
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim _desktopModuleController As New DesktopModuleController

            If IsNothing(_desktopModuleController.GetDesktopModuleByModuleName("NewBlog")) Then
                Return "NewBlog not installed"
            End If

            Dim _moduleDefinitionControler As New ModuleDefinitionController
            Dim New_View_BlogModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Blog").DesktopModuleID, "View_Blog").ModuleDefID
            Dim New_Blog_ListModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Blog").DesktopModuleID, "Blog_List").ModuleDefID
            Dim New_New_BlogModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Blog").DesktopModuleID, "New_Blog").ModuleDefID
            Dim New_Search_BlogModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Blog").DesktopModuleID, "Search_Blog").ModuleDefID
            Dim New_Blog_ArchiveModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Blog").DesktopModuleID, "Blog_Archive").ModuleDefID

            Dim Old_View_BlogModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("NewBlog").DesktopModuleID, "View Blog").ModuleDefID
            Dim Old_Blog_ListModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("NewBlog").DesktopModuleID, "Blog List").ModuleDefID
            Dim Old_New_BlogModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("NewBlog").DesktopModuleID, "New Blog").ModuleDefID
            Dim Old_Search_BlogModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("NewBlog").DesktopModuleID, "Search NewBlog").ModuleDefID
            Dim Old_Blog_ArchiveModuleDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("NewBlog").DesktopModuleID, "Blog Archive").ModuleDefID


            Dim _NewBlogUgradeController As New NewBlogUgradeController
            Dim _BlogController As New BlogController
            Dim _EntryController As New EntryController
            Dim _CommentController As New CommentController

            Dim _moduleInfo As ModuleInfo
            Dim _moduleController As New ModuleController

            Dim BlogId As Integer
            Dim EntryId As Integer
            Dim oldBlogId As Integer
            Dim oldEntryId As Integer
            Dim oldCommentId As Integer

            ' replace all NewBlog controls with the new blog controls
            Dim _allModules As ArrayList = _moduleController.GetAllModules()
            For Each mi As ModuleInfo In _allModules
                _moduleInfo = _moduleController.GetModule(mi.ModuleID, mi.TabID)

                If _moduleInfo.ModuleDefID = Old_View_BlogModuleDefId Then
                    _moduleInfo.ModuleDefID = New_View_BlogModuleDefId
                    _NewBlogUgradeController.UpgradeModuleDefId(_moduleInfo.ModuleDefID, _moduleInfo.ModuleID)
                End If

                If _moduleInfo.ModuleDefID = Old_Blog_ListModuleDefId Then
                    _moduleInfo.ModuleDefID = New_Blog_ListModuleDefId
                    _NewBlogUgradeController.UpgradeModuleDefId(_moduleInfo.ModuleDefID, _moduleInfo.ModuleID)
                End If

                If _moduleInfo.ModuleDefID = Old_New_BlogModuleDefId Then
                    _moduleInfo.ModuleDefID = New_New_BlogModuleDefId
                    _NewBlogUgradeController.UpgradeModuleDefId(_moduleInfo.ModuleDefID, _moduleInfo.ModuleID)
                End If

                If _moduleInfo.ModuleDefID = Old_Search_BlogModuleDefId Then
                    _moduleInfo.ModuleDefID = New_Search_BlogModuleDefId
                    _NewBlogUgradeController.UpgradeModuleDefId(_moduleInfo.ModuleDefID, _moduleInfo.ModuleID)
                End If

                If _moduleInfo.ModuleDefID = Old_Blog_ArchiveModuleDefId Then
                    _moduleInfo.ModuleDefID = New_Blog_ArchiveModuleDefId
                    _NewBlogUgradeController.UpgradeModuleDefId(_moduleInfo.ModuleDefID, _moduleInfo.ModuleID)
                End If
            Next

            'transfer blog settings
            Dim dr As IDataReader = _NewBlogUgradeController.Upgrade_GetBlogModuleSettings()
            While dr.Read()
                Utility.UpdateBlogModuleSetting(CType(dr("PortalID"), Integer), 0, CType(dr("Key"), String), CType(dr("Value"), String))
            End While
            dr.Close()

            ' transfer data from newblog tables to new blog tables
            Try
                Dim _blogs As ArrayList = _NewBlogUgradeController.Upgrade_ListBlogs(Nothing, Nothing, True)
                Dim _blogInfo As New BlogInfo
                For Each bi As BlogInfo In _blogs
                    oldBlogId = bi.BlogID

                    _blogInfo = _BlogController.GetUsersParentBlogByName(bi.PortalID, bi.UserName)
                    If Not IsNothing(_blogInfo) Then           ' blog for this user already exsits
                        bi.ParentBlogID = _blogInfo.BlogID
                        bi.AllowTrackbacks = True
                        BlogId = _BlogController.AddBlog(bi)
                    Else                                        ' first blog for this user
                        BlogId = _BlogController.AddBlog(bi)
                    End If


                    Dim _entries As ArrayList = _NewBlogUgradeController.Upgrade_ListEntriesByBlog(bi.BlogID, Nothing, True)
                    For Each ei As EntryInfo In _entries
                        oldEntryId = ei.EntryID
                        ei.BlogID = BlogId
                        EntryId = _EntryController.AddEntry(ei, ei.TabID).EntryID
                        Dim _comments As ArrayList = _NewBlogUgradeController.Upgrade_ListComments(ei.EntryID)
                        For Each ci As CommentInfo In _comments
                            oldCommentId = ci.CommentID
                            ci.EntryID = EntryId
                            ci.Approved = True
                            _CommentController.AddComment(ci)
                            _NewBlogUgradeController.Upgrade_DeleteComment(oldCommentId)
                        Next
                        _NewBlogUgradeController.Upgrade_DeleteEntry(oldEntryId)
                    Next
                    _NewBlogUgradeController.Upgrade_DeleteBlog(oldBlogId)
                Next

            Catch ex As Exception
                Exceptions.LogException(ex)
            End Try

            ' It was only a try, but I think it is better not to move existing files because all references in text must also changed
            'Try
            '    Dim _portalFolder, _sourceFolder, _targetFolder As String
            '    Dim _portalController As PortalController = New PortalController
            '    Dim _portals As ArrayList = _portalController.GetPortals()
            '    For Each pi As PortalInfo In _portals

            '        _portalFolder = pi.HomeDirectoryMapPath()
            '        _sourceFolder = _portalFolder & "NewBlog\"
            '        _targetFolder = _portalFolder & "Blog\"
            '        MoveFiles(_sourceFolder, _targetFolder)
            '    Next
            'Catch ex As Exception
            '    Exceptions.LogException(ex)
            'End Try



        Catch ex As Exception
            Exceptions.LogException(ex)
        End Try
        Return "Upgrade NewBlog finished"
    End Function

    Private Sub MoveFiles(ByVal sourceFolder As String, ByVal targetFolder As String)
        If Directory.Exists(sourceFolder) Then
            If Not Directory.Exists(targetFolder) Then
                Directory.CreateDirectory(targetFolder)
            End If
            Dim _files As String() = Directory.GetFiles(sourceFolder)
            For Each _file As String In _files
                IO.File.Move(_file, targetFolder & _file.Substring(_file.LastIndexOf("\") + 1))
            Next
            Dim _dirs As String() = Directory.GetDirectories(sourceFolder)
            For Each dir As String In _dirs
                Me.MoveFiles(dir, targetFolder & dir.Substring(dir.LastIndexOf("\") + 1) & "\")
            Next
        End If
    End Sub

#End Region

#Region "Category/Tag Migration"

    Public Function MigrateTaxonomyFolksonomy() As String
        Try
            Dim message As String = ""
            Dim countContentItems As Integer = 0
            Dim countCategories As Integer = 0
            Dim countTags As Integer = 0

            Dim cntEntry As New EntryController
            Dim colEntries As New List(Of EntryInfo)
            colEntries = cntEntry.RetrieveTaxonomyRelatedPosts()

            If colEntries.Count > 0 Then
                ' loop through tags (old system), create in core (vocab = 1). KEEP collection of new tags available to query by name later
                Dim colOldTags As List(Of MigrateTagInfo)
                colOldTags = TagController.GetAllTagsForUpgrade()

                Dim colNewTerms As New List(Of Term)

                If colOldTags IsNot Nothing Then
                    For Each objTag As MigrateTagInfo In colOldTags
                        Dim objTerm As New Term
                        objTerm = Terms.CreateAndReturnTerm(objTag.Tag, 1)
                        colNewTerms.Add(objTerm)

                        ' update
                        objTag.NewTermId = objTerm.TermId

                        countTags += 1
                    Next
                End If
                message = "Migrated " + countTags.ToString() + " tags. " & vbCrLf & vbCrLf

                ' loop through categories (old system), from table ordered by portal id, create under new vocabulary. KEEP both collections available
                Dim colOldCategories As List(Of MigrateCategoryInfo)
                colOldCategories = CategoryController.GetAllCategoriesForUpgrade()

                Dim cntVocabulary As New VocabularyController
                Dim colVocabs As IQueryable(Of Vocabulary) = cntVocabulary.GetVocabularies()

                Dim currentPortalId As Integer = -1
                Dim currentVocabId As Integer = -1
                Dim currentParentId As Integer = -1

                If colOldCategories IsNot Nothing Then
                    For Each objCategory As MigrateCategoryInfo In colOldCategories
                        If Not (objCategory.PortalId = currentPortalId) Then
                            currentPortalId = objCategory.PortalId
                            '' let's first see if there is an existing blog vocabulary for this portal
                            'Dim objTempVocab As Vocabulary = colVocabs.Where(Function(s) s.ScopeId = currentPortalId And s.Name = "Blog").SingleOrDefault()

                            'If objTempVocab IsNot Nothing Then
                            '    currentVocabId = objTempVocab.VocabularyId
                            'Else
                            Dim cntScope As New ScopeTypeController
                            Dim objScope As ScopeType = cntScope.GetScopeTypes().Where(Function(s) s.ScopeType = "Portal").SingleOrDefault()
                            Dim objVocab As New Vocabulary

                            objVocab.Name = "Blog Topics"
                            objVocab.IsSystem = False
                            objVocab.Weight = 0
                            objVocab.Description = "Automatically generated for blog module."
                            objVocab.ScopeId = currentPortalId
                            objVocab.ScopeTypeId = objScope.ScopeTypeId
                            'NOTE: CP - THis should be hierarchy, having a problem getting it to work.
                            objVocab.Type = VocabularyType.Simple
                            objVocab.VocabularyId = cntVocabulary.AddVocabulary(objVocab)

                            currentVocabId = objVocab.VocabularyId

                            DotNetNuke.Modules.Blog.Components.Business.Utility.UpdateBlogModuleSetting(currentPortalId, -1, "VocabularyId", currentVocabId.ToString)
                            'End If
                        End If

                        If objCategory.ParentId > 0 Then
                            If Not (objCategory.ParentId = currentParentId) Then
                                Dim tempParentId As Integer = objCategory.ParentId
                                Dim objParentCategory As MigrateCategoryInfo = colOldCategories.Where(Function(t) t.CatId = tempParentId).FirstOrDefault
                                Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
                                Dim objParentTerm As Term = termController.GetTermsByVocabulary(currentVocabId).Where(Function(t) t.Name.ToLower = objParentCategory.Category.ToLower).FirstOrDefault

                                If objParentTerm IsNot Nothing Then
                                    currentParentId = objParentTerm.TermId
                                End If
                            End If
                        End If

                        'Dim termController As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
                        'Dim existantTerm As Term
                        'existantTerm = termController.GetTermsByVocabulary(currentVocabId).Where(Function(t) t.TermId = id).FirstOrDefault()



                        Dim objTerm As New Term
                        objTerm = Terms.CreateAndReturnTerm(objCategory.Category, currentVocabId, currentParentId)
                        colNewTerms.Add(objTerm)

                        ' update 
                        objCategory.NewTermId = objTerm.TermId

                        countCategories += 1
                    Next
                End If
                message += "Migrated " + countCategories.ToString() + " categories. " & vbCrLf & vbCrLf

                ' At this point we have all categories and tags created in the core and also have the old categories/tags in collections associated w/ the new corresponding term id's

                ' loop through all entries that have a category or tag associated with them
                For Each objEntry As EntryInfo In colEntries
                    Dim portalId As Integer = -1

                    If objEntry.ContentItemId < 1 Then
                        Dim cntTaxonomy As New Content()
                        Dim objContentItem As ContentItem = cntTaxonomy.CreateContentItem(objEntry, objEntry.TabID)
                        objEntry.ContentItemId = objContentItem.ContentItemId

                        countContentItems += 1
                    End If

                    Dim entryTerms As New List(Of Term)

                    ' handle tags
                    Dim colTags As List(Of TagInfo) = CBO.FillCollection(Of TagInfo)(DataProvider.Instance().ListTagsByEntry(objEntry.EntryID))
                    For Each objOldTag As TagInfo In colTags
                        Dim tagid As Integer = objOldTag.TagId
                        Dim objMatchedTag As MigrateTagInfo = colOldTags.Where(Function(t) t.TagId = tagid).FirstOrDefault

                        If objMatchedTag IsNot Nothing Then
                            Dim objMatchedTerm As Term
                            objMatchedTerm = Terms.GetTermById(objMatchedTag.NewTermId, 1)

                            If objMatchedTerm IsNot Nothing Then
                                entryTerms.Add(objMatchedTerm)
                            End If

                            portalId = objMatchedTag.PortalId
                        End If
                    Next

                    ' handle categories
                    Dim colCats As List(Of CategoryInfo) = CategoryController.ListCatsByEntry(objEntry.EntryID)
                    For Each objOldCat As CategoryInfo In colCats
                        Dim catid As Integer = objOldCat.CatId
                        Dim objMatchedCategory As MigrateCategoryInfo = colOldCategories.Where(Function(t) t.CatId = catid).FirstOrDefault

                        If objMatchedCategory IsNot Nothing Then
                            Dim objMatchedTerm As Term
                            objMatchedTerm = Terms.GetTermById(objMatchedCategory.NewTermId, 1)

                            If objMatchedTerm IsNot Nothing Then
                                entryTerms.Add(objMatchedTerm)
                            End If

                            portalId = objMatchedCategory.PortalId
                        End If
                    Next

                    If portalId > -1 Then
                        ' update content item
                        objEntry.Terms.Clear()
                        objEntry.Terms.AddRange(entryTerms)
                        cntEntry.UpdateEntry(objEntry, objEntry.TabID, portalId)
                    End If
                Next
                message += "Migrated " + countContentItems.ToString() + " content items. " & vbCrLf & vbCrLf
            End If

            Return message
        Catch ex As Exception
            LogException(ex)
            Return ex.Message.ToString()
        End Try
    End Function

#End Region

End Class