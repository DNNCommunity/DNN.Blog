<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MainView.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MainView" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlAddModuleDefs" Visible="false" CssClass="dnnForm">
    <div class="dnnFormItem">
        <dnn:label id="lblAddModuleDef" runat="server" controlname="ddModuleDef" suffix=":" />
        <div class="dnnLeft">
            <asp:DropDownList runat="server" ID="ddModuleDef" DataTextField="FriendlyName" DataValueField="ModuleDefID" />&nbsp;
            <asp:DropDownList runat="server" ID="ddPane" />&nbsp;
            <asp:DropDownList runat="server" ID="ddPosition" />         
        </div>
    </div>
    <div class="dnnFormItem">
        <label></label>
        <div class="dnnLeft">
            <asp:TextBox runat="server" ID="txtTitle" />
            <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CssClass="dnnSecondaryAction" />
        </div>
    </div>
    <hr />
</asp:Panel>