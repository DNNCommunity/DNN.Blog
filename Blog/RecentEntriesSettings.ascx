<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Blog.RecentEntriesSettings" Codebehind="RecentEntriesSettings.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnRecentSettings dnnClear">
    <div class="dnnFormItem">
        <dnn:label id="lblTemplate" runat="server" controlname="txtTemplate" suffix=":" />
        <asp:textbox id="txtTemplate" columns="30" textmode="MultiLine" rows="10" maxlength="2000" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblMaxCount" runat="server" controlname="lblMaxCount" suffix=":" />
        <asp:textbox id="txtMaxCount" width="50" runat="server" />
    </div>
</div>