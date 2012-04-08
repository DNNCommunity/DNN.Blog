<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewTagsSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewTagsSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnTagSettings dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblDisplayMode" runat="server" controlname="rblTagDisplayMode" suffix=":" />
        <asp:RadioButtonList ID="rblTagDisplayMode" runat="server" CssClass="dnnFormRadioButtons">
            <asp:ListItem Value="Cloud" resourcekey="optTagCloud" />
            <asp:ListItem Value="List" resourcekey="optTagList" />
        </asp:RadioButtonList>
    </div>
</div>