<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<asp:panel id="pnlBlog" runat="server">
	<asp:label id="lblLogin" runat="server" ResourceKey="lblLogin" visible="False" cssclass="NormalRed">You must be logged in and have permission to create or edit a blog.</asp:label>
	<asp:linkbutton id="lnkBlog" runat="server" ResourceKey="lnkBlog" visible="False" cssclass="CommandButton">Create My Blog</asp:linkbutton>
</asp:panel>
<asp:panel id="pnlExistingBlog" runat="server">
	<asp:linkbutton id="lnkEditBlog" runat="server" ResourceKey="lnkEditBlog" cssclass="CommandButton">Blog Settings</asp:linkbutton>
	<BR>
	<asp:linkbutton id="lnkViewBlog" runat="server" ResourceKey="lnkViewBlog" cssclass="CommandButton">View My Blog</asp:linkbutton>
	<BR>
	<asp:linkbutton id="lnkAddEntry" runat="server" ResourceKey="lnkAddEntry" cssclass="CommandButton">Add Blog Entry</asp:linkbutton>
	<BR>
</asp:panel>
