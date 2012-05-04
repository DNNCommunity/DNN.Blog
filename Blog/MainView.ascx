<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MainView.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MainView" %>
<div class="dnnClear bloggerMenu">
    <div class="dnnRight divBloggerBar" id="divBloggerBar">
        <ul class="buttonGroup">
            <li id="liAddEntry" runat="server" visible="false"><asp:HyperLink ID="hlAddEntry" runat="server" ResourceKey="lnkAddEntry" CssClass="dnnTertiaryAction" /></li>
            <li id="liView" runat="server" visible="false"><asp:HyperLink ID="hlView" runat="server" ResourceKey="lnkViewBlog" CssClass="dnnTertiaryAction" /></li>
            <li id="liEditBlog" runat="server" visible="false"><asp:HyperLink ID="hlEditBlog" runat="server" ResourceKey="lnkEditBlog" CssClass="dnnTertiaryAction" /></li>
            <li id="liCreateBlog" runat="server" visible="false"><asp:HyperLink ID="hlCreateBlog" runat="server" ResourceKey="lnkBlog" CssClass="dnnTertiaryAction" /></li>
            <li id="liAddPart" runat="server" visible="false"><a class="dnnTertiaryAction" id="lnkAddPart" href="#">Add Blog Part</a></li>
        </ul> 
    </div>
</div>
<div id="divMainViewDialog" style="display:none;" class="dnnDialog divMainViewDialog">
    <div class="dnnClear">
        <div class="dnnFormItem" >
            <label>Blog Part:</label>
            <asp:DropDownList runat="server" ID="ddModuleDef" DataTextField="FriendlyName" DataValueField="ModuleDefID" />
        </div>
        <div class="dnnFormItem">
            <label>Title:</label>
            <div class="dnnLeft">
                <asp:TextBox runat="server" ID="txtTitle" />
            </div>
        </div>
        <div class="dnnFormItem">
            <label>Pane:</label>
            <asp:DropDownList runat="server" ID="ddPane" />
        </div>
        <div class="dnnFormItem">
            <label>Insert:</label>
            <asp:DropDownList runat="server" ID="ddPosition" />    
        </div>
        <div class="dnnRight">
            <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CssClass="dnnPrimaryAction" OnClick="cmdAdd_Click" />
        </div>
    </div>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupMainView() {
            $('#divMainViewDialog').dialog({ autoOpen: false, minWidth: 350, title: 'Add Blog Part to Page' });

            $("#lnkAddPart").click(function () {
                // show dialog for adding module part.
                $('#divMainViewDialog').dialog('open');

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