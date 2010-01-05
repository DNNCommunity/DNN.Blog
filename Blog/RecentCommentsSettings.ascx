<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecentCommentsSettings.ascx.vb"
 Inherits="DotNetNuke.Modules.Blog.RecentCommentsSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0" summary="Recent Blog Settings Design Table">
 <tr>
  <td class="SubHead" width="180">
   <dnn:label id="lblTemplate" runat="server" controlname="txtTemplate" suffix=":" />
  </td>
  <td valign="bottom">
   <asp:TextBox ID="txtTemplate" CssClass="NormalTextBox" Width="300" Columns="30" TextMode="MultiLine" Rows="10" MaxLength="2000" runat="server" />
  </td>
 </tr>
 <tr>
  <td class="SubHead" width="180">
   <dnn:label id="lblMaxCount" runat="server" controlname="lblMaxCount" suffix=":" />
  </td>
  <td valign="bottom">
   <asp:TextBox ID="txtMaxCount" CssClass="NormalTextBox" Width="50" runat="server" />
  </td>
 </tr>
</table>
