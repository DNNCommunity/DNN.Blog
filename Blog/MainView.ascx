<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MainView.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MainView" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlAddModuleDefs" Visible="false" CssClass="dnnForm dnnClear blogMainView">
    <div id="divbmForm">
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
    </div>
    <div class="dnnRight">
        <a href="#" id="bmvShow"><%= Localization.GetString("Show", LocalResourceFile)%></a>
        <a href="#" id="bmvHide"><%= Localization.GetString("Hide", LocalResourceFile)%></a>
    </div>
</asp:Panel>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupMainView() {
            $("#divbmForm").hide();
            $("#bmvHide").hide();

            $("#bmvShow").click(function () {
                $("#divbmForm").show('highlight', '', 500, '');
                $("#bmvHide").show();
                $("#bmvShow").hide();              
                return false;
            });
            $("#bmvHide").click(function () {
                $("#bmvShow").show();
                $("#divbmForm").hide('highlight', '', 200, '');
                $("#bmvHide").hide();
                return false;
            });
        }

        $(document).ready(function () {
            setupMainView();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupMainView();
            });
        });

    } (jQuery, window.Sys));
</script>  