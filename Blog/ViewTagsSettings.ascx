<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewTagsSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewTagsSettings" %>
<asp:RadioButtonList ID="rblTagDisplayMode" runat="server">
    <asp:ListItem Value="Cloud">Tag Cloud</asp:ListItem>
    <asp:ListItem Value="List">Tag List</asp:ListItem>
</asp:RadioButtonList>
