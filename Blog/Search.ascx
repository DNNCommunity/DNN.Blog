<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Search.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Search" %>

<table id="Table1" cellspacing="1" cellpadding="1" width="100%" border="0">
  <tr>
    <td><asp:DropDownList id="cboBlogSelect" runat="server" Width="100%" CssClass="NormalTextBox"></asp:DropDownList>    </td>
  </tr>
  <tr>
    <td><asp:textbox id="txtSearch" runat="server" cssclass="NormalTextBox" Width="100%" EnableViewState="False"></asp:textbox></td>
  </tr>
  <tr>
    <td><asp:radiobuttonlist id="optSearchType" runat="server" cssclass="SearchOptions" repeatdirection="Horizontal" autopostback="False">
        <asp:ListItem Value="Keyword" Selected="True" ResourceKey="optSearchType_Keyword">Keywords</asp:ListItem>
        <asp:ListItem Value="Phrase" ResourceKey="optSearchType_Phrase">Phrase</asp:ListItem>
      </asp:radiobuttonlist></td>
  </tr>
  <tr>
    <td align="right"><asp:Button ID="btnSearch" runat="server" CssClass="Normal" Text="Search" resourcekey="btnSearch"></asp:Button></td>
  </tr>
</table>
