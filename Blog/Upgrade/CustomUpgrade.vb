'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
'-------------------------------------------------------------------------
Imports System.IO
Imports DotNetNuke.Modules.Blog.Business
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.Exceptions

Public Class CustomUpgrade

#Region "Forum/Blog Import"

    Public Function ImportForumBlog(ByVal GroupID As Integer, ByVal BlogID As Integer) As Boolean
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
                    Dim EntryID As Integer = m_EntryController.AddEntry(m_EntryInfo)
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

                    _blogInfo = _BlogController.GetBlogByUserName(bi.PortalID, bi.UserName)
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
                        EntryId = _EntryController.AddEntry(ei)
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
        Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)


        ' We need to get all categories associated w/ the portal (in the blog), ' We need to get all tags associated w/ the portal (in the blog), ' We need to create content items

        '' OR

        '' Get all blog entries for the portal. Go through each one, add a content item, then migrate any categories/tags
        Dim cntBlogs As New BlogController
        Dim colAllPortalBlogEntries As List(Of EntryInfo) = cntBlogs.GetAllPublishedPortalBlogEntries(_portalSettings.PortalId)

        If colAllPortalBlogEntries IsNot Nothing Then
            ' we have some published blog entries for this portal

            ' we need to go through each entry, create a content item, then migrate any categories/tags


        End If

        'Dim _desktopModuleController As New DesktopModuleController

        Return ""
    End Function

#End Region

End Class