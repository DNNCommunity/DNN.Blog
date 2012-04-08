<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecentCommentsSettings.ascx.vb"
 Inherits="DotNetNuke.Modules.Blog.RecentCommentsSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnRecentSettings dnnClear">
    <div class="dnnFormItem">
        <dnn:label id="lblTemplate" runat="server" controlname="txtTemplate" suffix=":" />
        <asp:TextBox ID="txtTemplate" Columns="30" TextMode="MultiLine" Rows="10" MaxLength="2000" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblMaxCount" runat="server" controlname="lblMaxCount" suffix=":" />
        <asp:TextBox ID="txtMaxCount" Width="50" runat="server" MaxLength="5" />
    </div>
</div>