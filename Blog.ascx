<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<%@ Register TagPrefix="blog" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog.Templating" %>
<%@ Register TagPrefix="blog" TagName="comments" Src="controls/Comments.ascx" %>
<%@ Register TagPrefix="blog" TagName="management" Src="controls/ManagementPanel.ascx" %>

<asp:Literal runat="server" ID="litTrackback" />

<blog:management runat="server" id="ctlManagement" />

<div id="blogServiceErrorBox<%=ModuleId %>"></div>

<blog:ViewTemplate runat="server" id="vtContents" />

<blog:comments runat="server" id="ctlComments" />
