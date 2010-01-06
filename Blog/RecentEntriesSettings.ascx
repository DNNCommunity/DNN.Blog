<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Blog.RecentEntriesSettings" Codebehind="RecentEntriesSettings.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" border="0" summary="Recent Blog Settings Design Table">
    <tr>
        <td class="SubHead" width="180"><dnn:label id="lblTemplate" runat="server" controlname="txtTemplate" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtTemplate" cssclass="NormalTextBox" width="300" columns="30" textmode="MultiLine" rows="10" maxlength="2000" runat="server" />
        </td>
    </tr>
       <tr>
        <td class="SubHead" width="180"><dnn:label id="lblMaxCount" runat="server" controlname="lblMaxCount" suffix=":"></dnn:label></td>
        <td valign="bottom" >
            <asp:textbox id="txtMaxCount" cssclass="NormalTextBox" width="50" runat="server" />
        </td>
    </tr>
</table>