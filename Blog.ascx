<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<%@ Register TagPrefix="blog" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog.Templating" %>
<%@ Register TagPrefix="blog" TagName="comments" Src="controls/Comments.ascx" %>
<%@ Register TagPrefix="blog" TagName="management" Src="controls/ManagementPanel.ascx" %>

<blog:management runat="server" id="ctlManagement" />

<blog:ViewTemplate runat="server" id="vtContents" />

<blog:comments runat="server" id="ctlComments" />
