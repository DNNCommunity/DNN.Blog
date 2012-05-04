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

Imports System
Imports System.Xml
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Users

Namespace Business

    Public Class InterfaceController
        Implements Entities.Modules.IUpgradeable
        Implements Entities.Modules.IPortable
        Implements Entities.Modules.ISearchable

#Region "IUpgradeable"

        Public Function UpgradeModule(ByVal Version As String) As String Implements Entities.Modules.IUpgradeable.UpgradeModule
            Dim message As String = ""

            Select Case Version
                Case "03.01.20"         ' first version as DotNetNuke sub project
                    message = message & " In Custom Upgrade Version " & Version & " - "
                    Dim _CustomUpgrade As New CustomUpgrade
                    message += _CustomUpgrade.UpgradeNewBlog()
                    message += " - " & _CustomUpgrade.UpgradeForumBlog()
                Case "04.05.00"
                    message = message & " Migrating taxonomy/folksonomy in " & Version & " - "
                    Dim _CustomUpgrade As New CustomUpgrade
                    'message += _CustomUpgrade.MigrateTaxonomyFolksonomy()
            End Select
            Return message

        End Function

#End Region

#Region "IPortable"

        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
            Dim strXML As String = "<blogs>"
            Dim m_PortalID As Integer
            Dim m_Blogs As ArrayList
            Dim m_Entries As List(Of EntryInfo)
            Dim m_Comments As ArrayList
            Dim m_ModuleController As New ModuleController
            Dim m_ModuleInfo As New ModuleInfo
            Try

                m_ModuleInfo = m_ModuleController.GetModule(ModuleID, Null.NullInteger)
                m_PortalID = m_ModuleInfo.PortalID
                Dim m_BlogController As New BlogController
                Dim m_EntryController As New EntryController
                Dim m_CommentController As New CommentController
                m_Blogs = m_BlogController.ListBlogsByPortal(m_PortalID, True)
                For Each blog As BlogInfo In m_Blogs
                    strXML += "<blog>"
                    strXML += "<allowAnonymous>" & XmlUtils.XMLEncode(blog.AllowAnonymous.ToString) & "</allowAnonymous>"
                    strXML += "<allowComments>" & XmlUtils.XMLEncode(blog.AllowComments.ToString) & "</allowComments>"
                    strXML += "<blogID>" & XmlUtils.XMLEncode(blog.BlogID.ToString) & "</blogID>"
                    strXML += "<childBlogCount>" & XmlUtils.XMLEncode(blog.ChildBlogCount.ToString) & "</childBlogCount>"
                    strXML += "<created>" & XmlUtils.XMLEncode(blog.Created.ToString) & "</created>"
                    strXML += "<culture>" & XmlUtils.XMLEncode(blog.Culture) & "</culture>"
                    strXML += "<dateFormat>" & XmlUtils.XMLEncode(blog.DateFormat) & "</dateFormat>"
                    strXML += "<description>" & XmlUtils.XMLEncode(blog.Description) & "</description>"
                    strXML += "<emailNotification>" & XmlUtils.XMLEncode(blog.EmailNotification.ToString) & "</emailNotification>"
                    strXML += "<lastEntry>" & XmlUtils.XMLEncode(blog.LastEntry.ToString) & "</lastEntry>"
                    strXML += "<parentBlogID>" & XmlUtils.XMLEncode(blog.ParentBlogID.ToString) & "</parentBlogID>"
                    strXML += "<portalID>" & XmlUtils.XMLEncode(blog.PortalID.ToString) & "</portalID>"
                    strXML += "<public>" & XmlUtils.XMLEncode(blog.Public.ToString) & "</public>"
                    strXML += "<showFullName>" & XmlUtils.XMLEncode(blog.ShowFullName.ToString) & "</showFullName>"
                    strXML += "<syndicated>" & XmlUtils.XMLEncode(blog.Syndicated.ToString) & "</syndicated>"
                    strXML += "<syndicateIndependant>" & XmlUtils.XMLEncode(blog.SyndicateIndependant.ToString) & "</syndicateIndependant>"
                    strXML += "<syndicationEmail>" & XmlUtils.XMLEncode(blog.SyndicationEmail) & "</syndicationEmail>"
                    strXML += "<syndicationURL>" & XmlUtils.XMLEncode(blog.SyndicationURL) & "</syndicationURL>"
     strXML += "<timeZone>" & XmlUtils.XMLEncode(blog.TimeZone.Id.ToString) & "</timeZone>"
                    strXML += "<title>" & XmlUtils.XMLEncode(blog.Title) & "</title>"
                    strXML += "<userFullName>" & XmlUtils.XMLEncode(blog.UserFullName) & "</userFullName>"
                    strXML += "<userID>" & XmlUtils.XMLEncode(blog.UserID.ToString) & "</userID>"
                    strXML += "<userName>" & XmlUtils.XMLEncode(blog.UserName) & "</userName>"
                    m_Entries = m_EntryController.ListAllEntriesByBlog(blog.BlogID)
                    If m_Entries.Count > 0 Then
                        strXML += "<entries>"
                        For Each entry As EntryInfo In m_Entries
                            strXML += "<blogEntry>"
                            strXML += "<addedDate>" & XmlUtils.XMLEncode(entry.AddedDate.ToString) & "</addedDate>"
                            strXML += "<allowComments>" & XmlUtils.XMLEncode(entry.AllowComments.ToString) & "</allowComments>"
                            strXML += "<blogID>" & XmlUtils.XMLEncode(entry.BlogID.ToString) & "</blogID>"
                            strXML += "<commentCount>" & XmlUtils.XMLEncode(entry.CommentCount.ToString) & "</commentCount>"
                            strXML += "<copyright>" & XmlUtils.XMLEncode(entry.Copyright) & "</copyright>"
                            strXML += "<description>" & XmlUtils.XMLEncode(entry.Description) & "</description>"
                            strXML += "<displayCopyright>" & XmlUtils.XMLEncode(entry.DisplayCopyright.ToString) & "</displayCopyright>"
                            strXML += "<entry>" & XmlUtils.XMLEncode(entry.Entry) & "</entry>"
                            strXML += "<entryID>" & XmlUtils.XMLEncode(entry.EntryID.ToString) & "</entryID>"
                            strXML += "<permaLink>" & XmlUtils.XMLEncode(entry.PermaLink) & "</permaLink>"
                            strXML += "<published>" & XmlUtils.XMLEncode(entry.Published.ToString) & "</published>"
                            strXML += "<title>" & XmlUtils.XMLEncode(entry.Title) & "</title>"
                            strXML += "<userFullName>" & XmlUtils.XMLEncode(entry.UserFullName) & "</userFullName>"
                            strXML += "<userID>" & XmlUtils.XMLEncode(entry.UserID.ToString) & "</userID>"
                            strXML += "<userName>" & XmlUtils.XMLEncode(entry.UserName) & "</userName>"
                            m_Comments = m_CommentController.ListComments(entry.EntryID, True)
                            If m_Comments.Count > 0 Then
                                strXML += "<comments>"
                                For Each comment As CommentInfo In m_Comments
                                    strXML += "<blogComment>"
                                    strXML += "<addedDate>" & XmlUtils.XMLEncode(comment.AddedDate.ToString) & "</addedDate>"
                                    strXML += "<comment>" & XmlUtils.XMLEncode(comment.Comment) & "</comment>"
                                    strXML += "<commentID>" & XmlUtils.XMLEncode(comment.CommentID.ToString) & "</commentID>"
                                    strXML += "<email>" & XmlUtils.XMLEncode(comment.Email) & "</email>"
                                    strXML += "<entryID>" & XmlUtils.XMLEncode(comment.EntryID.ToString) & "</entryID>"
                                    strXML += "<title>" & XmlUtils.XMLEncode(comment.Title) & "</title>"
                                    strXML += "<userFullName>" & XmlUtils.XMLEncode(comment.UserFullName) & "</userFullName>"
                                    strXML += "<userID>" & XmlUtils.XMLEncode(comment.UserID.ToString) & "</userID>"
                                    strXML += "<userName>" & XmlUtils.XMLEncode(comment.UserName) & "</userName>"
                                    strXML += "<website>" & XmlUtils.XMLEncode(comment.Website) & "</website>"
                                    strXML += "</blogComment>"
                                Next
                                strXML += "</comments>"
                            End If
                            strXML += "</blogEntry>"
                        Next
                        strXML += "</entries>"
                    End If
                    strXML += "</blog>"
                Next
                strXML += "</blogs>"
            Catch ex As Exception
                LogException(ex)
            End Try

            Return strXML

        End Function

        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements Entities.Modules.IPortable.ImportModule
            Try
                Dim BlogID As Integer = -1
                Dim EntryID As Integer = -1
                Dim xmlblog As XmlNode
                Dim xmlBlogs As XmlNode = GetContent(Content, "blogs")

                ' DW - 06/30/2008 - BLG-7837 Added to ensure that the ImportModule procedure
                '                   is only called from the Blog_List sub module.
                Dim ModuleController As New ModuleController
                Dim ModuleDefController As New ModuleDefinitionController
                Dim ModuleDefInfo As ModuleDefinitionInfo
                Dim ModuleInfo As ModuleInfo
                ModuleInfo = ModuleController.GetModule(ModuleID, Null.NullInteger)
                ModuleDefInfo = ModuleDefController.GetModuleDefinition(ModuleInfo.ModuleDefID)
                If ModuleDefInfo.FriendlyName = "Blog_List" Then

                    If Not IsNothing(xmlBlogs) Then
                        For Each xmlblog In xmlBlogs
                            Dim m_Blog As New BlogInfo
                            m_Blog.AllowAnonymous = Boolean.Parse(xmlblog.Item("allowAnonymous").InnerText)
                            m_Blog.AllowComments = Boolean.Parse(xmlblog.Item("allowComments").InnerText)
                            m_Blog.BlogID = Integer.Parse(xmlblog.Item("blogID").InnerText)
                            m_Blog.ChildBlogCount = Integer.Parse(xmlblog.Item("childBlogCount").InnerText)
                            m_Blog.Created = Date.Parse(xmlblog.Item("created").InnerText)
                            m_Blog.Culture = xmlblog.Item("culture").InnerText
                            m_Blog.DateFormat = xmlblog.Item("dateFormat").InnerText
                            m_Blog.Description = xmlblog.Item("description").InnerText
                            m_Blog.EmailNotification = Boolean.Parse(xmlblog.Item("emailNotification").InnerText)
                            m_Blog.LastEntry = Date.Parse(xmlblog.Item("lastEntry").InnerText)
                            m_Blog.ParentBlogID = Integer.Parse(xmlblog.Item("parentBlogID").InnerText)
                            m_Blog.PortalID = Integer.Parse(xmlblog.Item("portalID").InnerText)
                            m_Blog.Public = Boolean.Parse(xmlblog.Item("public").InnerText)
                            m_Blog.ShowFullName = Boolean.Parse(xmlblog.Item("showFullName").InnerText)
                            m_Blog.Syndicated = Boolean.Parse(xmlblog.Item("syndicated").InnerText)
                            m_Blog.SyndicateIndependant = Boolean.Parse(xmlblog.Item("syndicateIndependant").InnerText)
                            m_Blog.SyndicationEmail = xmlblog.Item("syndicationEmail").InnerText
                            m_Blog.SyndicationURL = xmlblog.Item("syndicationURL").InnerText
       Try
        m_Blog.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(xmlblog.Item("timeZone").InnerText)
       Catch ex As Exception
       End Try
                            m_Blog.Title = xmlblog.Item("title").InnerText
                            m_Blog.UserFullName = xmlblog.Item("userFullName").InnerText
                            m_Blog.UserID = Integer.Parse(xmlblog.Item("userID").InnerText)
                            m_Blog.UserName = xmlblog.Item("userName").InnerText
                            BlogID = Me.addBlog(m_Blog)
                            m_Blog.BlogID = BlogID
                            Dim xmlEntry As XmlNode
                            Dim xmlEntries As XmlNode = GetContent(xmlblog.LastChild.OuterXml.ToString, "entries")
                            If Not IsNothing(xmlEntries) Then
                                For Each xmlEntry In xmlEntries
                                    Dim m_Entry As New EntryInfo
                                    m_Entry.AddedDate = Date.Parse(xmlEntry.Item("addedDate").InnerText)
                                    m_Entry.AllowComments = Boolean.Parse(xmlEntry.Item("allowComments").InnerText)
                                    m_Entry.BlogID = Integer.Parse(xmlEntry.Item("blogID").InnerText)
                                    m_Entry.CommentCount = Integer.Parse(xmlEntry.Item("commentCount").InnerText)
                                    m_Entry.Copyright = xmlEntry.Item("copyright").InnerText
                                    m_Entry.Description = xmlEntry.Item("description").InnerText
                                    m_Entry.DisplayCopyright = Boolean.Parse(xmlEntry.Item("displayCopyright").InnerText)
                                    m_Entry.Entry = xmlEntry.Item("entry").InnerText
                                    m_Entry.EntryID = Integer.Parse(xmlEntry.Item("entryID").InnerText)
                                    ' DW - 06/21/2008 - Removed since old PermaLink is no longer useful in this context.
                                    '   Also needed this to be NULL since the Blog_AddEntry sproc tries to update this 
                                    '   value with the EntryID if a value exists.  By leaving this NULL, we allow the 
                                    '   cmdImport_Click procedure in the BlogImport.ascx page to update the value.
                                    'm_Entry.PermaLink = xmlEntry.Item("permaLink").InnerText
                                    m_Entry.Published = Boolean.Parse(xmlEntry.Item("published").InnerText)
                                    m_Entry.Title = xmlEntry.Item("title").InnerText
                                    m_Entry.UserFullName = xmlEntry.Item("userFullName").InnerText
                                    m_Entry.UserID = Integer.Parse(xmlEntry.Item("userID").InnerText)
                                    m_Entry.UserName = xmlEntry.Item("userName").InnerText
                                    m_Entry = Me.addEntry(m_Blog, m_Entry, ModuleID)
                                    Dim xmlComment As XmlNode
                                    Dim xmlComments As XmlNode = GetContent(xmlEntry.LastChild.OuterXml.ToString, "comments")
                                    If Not IsNothing(xmlComments) Then
                                        For Each xmlComment In xmlComments
                                            Dim m_Comment As New CommentInfo
                                            m_Comment.AddedDate = Date.Parse(xmlComment.Item("addedDate").InnerText)
                                            m_Comment.Comment = xmlComment.Item("comment").InnerText
                                            m_Comment.CommentID = Integer.Parse(xmlComment.Item("commentID").InnerText)
                                            m_Comment.Email = xmlComment.Item("email").InnerText
                                            m_Comment.EntryID = Integer.Parse(xmlComment.Item("entryID").InnerText)
                                            m_Comment.Title = xmlComment.Item("title").InnerText
                                            m_Comment.UserFullName = xmlComment.Item("userFullName").InnerText
                                            m_Comment.UserID = Integer.Parse(xmlComment.Item("userID").InnerText)
                                            m_Comment.UserName = xmlComment.Item("userName").InnerText
                                            m_Comment.Website = xmlComment.Item("website").InnerText
                                            Me.addComments(m_Entry, m_Comment)
                                        Next
                                    End If
                                Next
                            End If
                            ' DW - 06/22/2008 - Moved here to accomodate changes to CreateAllEntryLinks.
                            '                   It now takes BlogID as a parameter
                            ' Update all PermaLinks
                            Utility.CreateAllEntryLinks(m_Blog.PortalID, m_Blog.BlogID)
                        Next
                    End If
                End If

            Catch ex As Exception
                Exceptions.LogException(ex)
            End Try

        End Sub

#Region "private methods"
        Private Function addBlog(ByVal blog As BlogInfo) As Integer
            Dim m_PortalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim m_UserController As New UserController
            Dim m_UserInfo As New UserInfo
            Dim retVal As Integer
            blog.PortalID = m_PortalSettings.PortalId
            ' 11/19/2008 replaced m_UserController with UserController
            m_UserInfo = UserController.GetUserByName(m_PortalSettings.PortalId, blog.UserName)
            If Not IsNothing(m_UserInfo) Then       ' User Exists
                blog.UserID = m_UserInfo.UserID
                blog.UserFullName = m_UserInfo.DisplayName
            Else                                    ' User doesn't exist
                m_UserInfo = m_UserController.GetUser(m_PortalSettings.PortalId, m_PortalSettings.AdministratorId)
                blog.UserFullName = m_UserInfo.DisplayName
                blog.UserID = m_UserInfo.UserID
                blog.UserName = m_UserInfo.Username
            End If
            Dim m_BlogController As New BlogController
            Dim m_BlogInfo As New BlogInfo
            m_BlogInfo = m_BlogController.GetBlogByUserName(m_PortalSettings.PortalId, blog.UserName)
            If Not IsNothing(m_BlogInfo) Then           ' blog for this user already exsits
                blog.ParentBlogID = m_BlogInfo.BlogID
                retVal = m_BlogController.AddBlog(blog)
            Else                                        ' first blog for this user
                retVal = m_BlogController.AddBlog(blog)
            End If
            Return retVal
        End Function

        Private Function addEntry(ByVal blog As BlogInfo, ByVal entry As EntryInfo, ByVal moduleId As Integer) As EntryInfo
            Dim m_EntryController As New EntryController
            entry.BlogID = blog.BlogID
            entry.UserFullName = blog.UserFullName
            entry.UserID = blog.UserID
            entry.UserName = blog.UserName
            entry.ModuleID = moduleId
            Return m_EntryController.AddEntry(entry, entry.TabID)
        End Function

        Private Function addComments(ByVal entry As EntryInfo, ByVal comment As CommentInfo) As Integer
            Dim m_PortalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim m_UserController As New UserController
            Dim m_UserInfo As New UserInfo
            Dim m_CommentController As New CommentController
            If comment.UserID <> -1 Then            ' if not anonymous
                ' 11/19/2008 Rip Rowan replaced m_UserController with UserController
                m_UserInfo = UserController.GetUserByName(m_PortalSettings.PortalId, comment.UserName)
                If IsNothing(m_UserInfo) Then       ' User doens't exist in this portal
                    comment.UserID = -1
                    comment.UserName = ""
                Else
                    comment.UserID = m_UserInfo.UserID
                    comment.UserFullName = m_UserInfo.DisplayName
                End If
            End If
            comment.EntryID = entry.EntryID
            Return m_CommentController.AddComment(comment)
        End Function
#End Region

#End Region

#Region "ISearchable"
        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
            Dim BlogSettings As Settings.BlogSettings = DotNetNuke.Modules.Blog.Settings.BlogSettings.GetBlogSettings(ModInfo.PortalID, ModInfo.TabID)
            Dim IncludeInSearch As Boolean
            Dim SearchItemCollection As New SearchItemInfoCollection

            IncludeInSearch = BlogSettings.EnableDNNSearch

            If ModInfo.ControlTitle = "View Blog" And IncludeInSearch Then
                Dim m_EntryController As New EntryController
                Dim Entries As List(Of EntryInfo) = m_EntryController.ListAllEntriesByPortal(ModInfo.PortalID, False, False)
                Dim objEntry As Object

                For Each objEntry In Entries
                    Try

                        Dim SearchItem As SearchItemInfo
                        With CType(objEntry, EntryInfo)
                            Dim UserId As Integer = .UserID
                            Dim strContent As String = System.Web.HttpUtility.HtmlDecode(.Title & " " & .Description & " " & .Entry)
                            Dim strDescription As String = HtmlUtils.Shorten(HtmlUtils.Clean(System.Web.HttpUtility.HtmlDecode(.Entry), False), 100, "...")
                            SearchItem = New SearchItemInfo(ModInfo.FriendlyName & " - " & .Title, strDescription, UserId, .AddedDate, ModInfo.ModuleID, .EntryID.ToString, strContent, "EntryId=" & .EntryID.ToString)
                            SearchItemCollection.Add(SearchItem)
                        End With
                    Catch ex As Exception
                        Exceptions.LogException(ex)
                    End Try
                Next
            End If

            Return SearchItemCollection

        End Function
#End Region

    End Class

End Namespace