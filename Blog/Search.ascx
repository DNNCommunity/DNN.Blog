<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Search.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Search" %>
<table id="Table1" cellspacing="1" cellpadding="1" width="100%" border="0">
 <tr>
  <td>
   <asp:DropDownList ID="cboBlogSelect" runat="server" Width="100%" CssClass="NormalTextBox" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" Width="100%" EnableViewState="False" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:RadioButtonList ID="optSearchType" runat="server" CssClass="SearchOptions" RepeatDirection="Horizontal" AutoPostBack="False">
    <asp:ListItem Value="Keyword" Selected="True" ResourceKey="optSearchType_Keyword" />
    <asp:ListItem Value="Phrase" ResourceKey="optSearchType_Phrase" />
   </asp:RadioButtonList>
  </td>
 </tr>
 <tr>
  <td align="right">
   <asp:Button ID="btnSearch" runat="server" CssClass="Normal" resourcekey="btnSearch" Text=" " />
  </td>
 </tr>
</table>
