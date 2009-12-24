<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MainView.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MainView" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlAddModuleDefs" Visible="false">
 <table cellspacing="2" cellpadding="2" width="100%" border="0">
  <tr>
   <td class="SubHead" valign="top" width="165">
    <dnn:label id="lblAddModuleDef" runat="server" controlname="ddModuleDef" suffix="" />
   </td>
   <td>
    <asp:DropDownList runat="server" ID="ddModuleDef" DataTextField="FriendlyName" DataValueField="ModuleDefID" />&nbsp;
    <asp:DropDownList runat="server" ID="ddPane" />&nbsp;
    <asp:DropDownList runat="server" ID="ddPosition" />&nbsp;
    <asp:TextBox runat="server" ID="txtTitle" Width="150" />&nbsp;
    <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" />
   </td>
  </tr>
 </table>
 <hr />
</asp:Panel>
