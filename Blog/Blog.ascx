<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<asp:Panel ID="pnlBlog" runat="server" CssClass="dnnForm">
 <asp:Label ID="lblLogin" runat="server" ResourceKey="lblLogin" Visible="False" CssClass="dnnFormMessage dnnFormWarning" />
 <asp:LinkButton ID="lnkBlog" runat="server" ResourceKey="lnkBlog" Visible="False" CssClass="dnnPrimaryAction" />
</asp:Panel>
<asp:Panel ID="pnlExistingBlog" runat="server" CssClass="dnnForm">
    <ul class="dnnBlogAuthorActions">
        <li><asp:LinkButton ID="lnkAddEntry" runat="server" ResourceKey="lnkAddEntry" CssClass="dnnPrimaryAction" /></li>
        <li><asp:LinkButton ID="lnkEditBlog" runat="server" ResourceKey="lnkEditBlog" CssClass="dnnSecondaryAction" /></li>
        <li><asp:LinkButton ID="lnkViewBlog" runat="server" ResourceKey="lnkViewBlog" CssClass="dnnSecondaryAction" /></li>
    </ul>
</asp:Panel>