<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewCategories.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewCategories" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<div class="dnnForm dnnBlogCategories">
    <asp:Panel ID="pnlTreeview" runat="server">
        <dnnweb:DnnTreeView id="dtCategories" runat="server" DataFieldID="TermID" DataFieldParentID="ParentTermID" DataTextField="Name" DataValueField="TermID" />
    </asp:Panel>
    <asp:Panel ID="pnlList" runat="server" Visible="false">
    
    </asp:Panel>
</div>