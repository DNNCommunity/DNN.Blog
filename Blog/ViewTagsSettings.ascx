<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewTagsSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewTagsSettings" %>
<asp:RadioButtonList ID="rblTagDisplayMode" runat="server">
 <asp:ListItem Value="Cloud" resourcekey="optTagCloud" />
 <asp:ListItem Value="List" resourcekey="optTagList" />
</asp:RadioButtonList>
