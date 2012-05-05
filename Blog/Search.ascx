<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Search.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Search" %>
<div class="dnnblogSearch dnnForm dnnClear">
    <div class="dnnFormItem">
        <asp:DropDownList ID="cboBlogSelect" runat="server" />
    </div>
    <div class="dnnFormItem">
        <asp:TextBox ID="txtSearch" runat="server" EnableViewState="False" />
    </div>
    <div class="dnnFormItem">
        <asp:RadioButtonList ID="optSearchType" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" AutoPostBack="False">
            <asp:ListItem Value="Keyword" Selected="True" ResourceKey="optSearchType_Keyword" />
            <asp:ListItem Value="Phrase" ResourceKey="optSearchType_Phrase" />
        </asp:RadioButtonList>
        <asp:Button ID="btnSearch" runat="server" resourcekey="btnSearch" Text=" " />
    </div>
</div>