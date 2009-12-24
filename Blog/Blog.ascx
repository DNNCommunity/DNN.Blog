<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<asp:Panel ID="pnlBlog" runat="server">
 <asp:Label ID="lblLogin" runat="server" ResourceKey="lblLogin" Visible="False" CssClass="NormalRed" Text="You must be logged in and have permission to create or edit a blog." />
 <asp:LinkButton ID="lnkBlog" runat="server" ResourceKey="lnkBlog" Visible="False" CssClass="CommandButton" Text="Create My Blog" />
</asp:Panel>
<asp:Panel ID="pnlExistingBlog" runat="server">
 <asp:LinkButton ID="lnkEditBlog" runat="server" ResourceKey="lnkEditBlog" CssClass="CommandButton" Text="Blog Settings" />
 <br />
 <asp:LinkButton ID="lnkViewBlog" runat="server" ResourceKey="lnkViewBlog" CssClass="CommandButton" Text="View My Blog" />
 <br />
 <asp:LinkButton ID="lnkAddEntry" runat="server" ResourceKey="lnkAddEntry" CssClass="CommandButton" Text="Add Blog Entry" />
 <br />
</asp:Panel>
