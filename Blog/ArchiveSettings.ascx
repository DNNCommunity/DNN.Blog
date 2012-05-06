<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ArchiveSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ArchiveSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnTagSettings dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblLoadCss" runat="server" Suffix=":" ControlName="rblLoadCss" />
        <asp:RadioButtonList ID="rblLoadCss" runat="server" CssClass="dnnFormRadioButtons">
            <asp:ListItem Value="Yes" resourcekey="Yes" />
            <asp:ListItem Value="No" resourcekey="No" />
        </asp:RadioButtonList>
    </div>
</div>