<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewTagsSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewTagsSettings" %>
<asp:RadioButtonList ID="rblTagDisplayMode" runat="server">
 <asp:ListItem Value="Cloud" Text="Tag Cloud" />
 <asp:ListItem Value="List" Text="Tag List" />
</asp:RadioButtonList>
