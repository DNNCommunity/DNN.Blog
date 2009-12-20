'
' DotNetNuke -  http://www.dotnetnuke.com
' Copyright (c) 2002-2005
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

Imports System
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.Blog.Business

Namespace ForumBlog

 Public Class Utils

#Region "public shared methods"
  Public Shared Function isForumBlogInstalled(ByVal PortalID As Integer, ByVal TabID As Integer, ByVal checkNew As Boolean) As Boolean

   Dim m_BlogSettings As BlogSettings = BlogSettings.GetBlogSettings(PortalID, TabID)
   If Not checkNew Then
    If m_BlogSettings.ForumBlogInstalled = "Installed" Then
     Return True
    ElseIf m_BlogSettings.ForumBlogInstalled = "NotInstalled" Then
     Return False
    End If
   End If
   Dim m_desktopModuleController As New DesktopModuleController
   Dim DNN_BlogModuleID As Integer

   ' if module is not installed on the portal, return false
   If IsNothing(m_desktopModuleController.GetDesktopModuleByModuleName("DNN_Blog")) Then
    m_BlogSettings.ForumBlogInstalled = "NotInstalled"
    m_BlogSettings.UpdateSettings()
    'Utility.UpdateBlogModuleSetting(PortalID, TabID, "ForumBlogInstalled", "NotInstalled")
    Return False
   Else
    DNN_BlogModuleID = m_desktopModuleController.GetDesktopModuleByModuleName("DNN_Blog").DesktopModuleID
   End If

   Dim list As New ArrayList
   Dim m_Forum_GroupsController As New Forum_GroupsController
   list = m_Forum_GroupsController.List(PortalID)
   If list.Count > 0 Then              ' there are instances installed
    m_BlogSettings.ForumBlogInstalled = "Installed"
    m_BlogSettings.UpdateSettings()
    'Utility.UpdateBlogModuleSetting(PortalID, TabID, "ForumBlogInstalled", "Installed")
    Return True
   Else
    m_BlogSettings.ForumBlogInstalled = "NotInstalled"
    m_BlogSettings.UpdateSettings()
    'Utility.UpdateBlogModuleSetting(PortalID, TabID, "ForumBlogInstalled", "NotInstalled")
    Return False
   End If
  End Function
#End Region

 End Class

End Namespace