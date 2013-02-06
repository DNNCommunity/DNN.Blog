<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MainView.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MainView" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>

<h2>Main View Control</h2>
<div class="dnnClear bloggerMenu">
    <div class="dnnRight divBloggerBar" id="divBloggerBar">
        <ul class="buttonGroup">
            <li id="liAddEntry" runat="server" visible="false"><asp:HyperLink ID="hlAddEntry" runat="server" ResourceKey="lnkAddEntry" CssClass="dnnTertiaryAction" /></li>
            <li id="liView" runat="server" visible="false"><asp:HyperLink ID="hlView" runat="server" ResourceKey="lnkViewBlog" CssClass="dnnTertiaryAction" /></li>
            <li id="liEditBlog" runat="server" visible="false"><asp:HyperLink ID="hlEditBlog" runat="server" ResourceKey="lnkEditBlog" CssClass="dnnTertiaryAction" /></li>
            <li id="liCreateBlog" runat="server" visible="false"><asp:HyperLink ID="hlCreateBlog" runat="server" ResourceKey="lnkBlog" CssClass="dnnTertiaryAction" /></li>
            <li id="liAddPart" runat="server" visible="false"><a class="dnnTertiaryAction" id="lnkAddPart" href="#"><%= Localization.GetString("AddPart", LocalResourceFile)%></a></li>
        </ul> 
    </div>
</div>
<div id="divMainViewDialog" class="divMainViewDialog">
    <div class="dnnClear">
        <div class="dnnFormItem" >
            <dnn:Label ID="lblBlogPart" runat="server" Suffix=":" />
            <asp:DropDownList runat="server" ID="ddModuleDef" DataTextField="FriendlyName" DataValueField="ModuleDefID" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTitle" runat="server" Suffix=":" />
            <div class="dnnLeft">
                <asp:TextBox runat="server" ID="txtTitle" />
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPane" runat="server" Suffix=":" />
            <asp:DropDownList runat="server" ID="ddPane" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblInsert" runat="server" Suffix=":" />
            <asp:DropDownList runat="server" ID="ddPosition" />    
        </div>
        <div class="dnnRight">
            <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CssClass="dnnPrimaryAction" />
        </div>
    </div>
</div>
<dnnweb:DnnCodeBlock ID="dcbQuestions" runat="server" >
    <script language="javascript" type="text/javascript">
        /*globals jQuery, window, Sys */
        (function ($, Sys) {
            function setupMainView() {
                $('#divMainViewDialog').dialog({ autoOpen: false, minWidth: 350, title: '<%= Localization.GetString("DialogTitle", LocalResourceFile) %>' });

                $("#lnkAddPart").click(function () {
                    // show dialog for adding module part.
                    $('#divMainViewDialog').dialog('open');

                    return false;
                });

                $("#divMainViewDialog").parent().appendTo($("form:first"));
            }

            $(document).ready(function () {
                setupMainView();
            });

        } (jQuery, window.Sys));
    </script>  
</dnnweb:DnnCodeBlock>