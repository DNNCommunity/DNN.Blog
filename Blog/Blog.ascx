<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<asp:Panel ID="pnlBlog" runat="server" CssClass="dnnBlog dnnClear">
    <asp:Label ID="lblLogin" runat="server" ResourceKey="lblLogin" Visible="False" />
    <asp:LinkButton ID="lnkBlog" runat="server" ResourceKey="lnkBlog" Visible="False" CssClass="dnnPrimaryAction" />
</asp:Panel>
<asp:Panel ID="pnlExistingBlog" runat="server" CssClass="dnnBlog dnnClear">
    <ul class="dnnBlogAuthorActions" id="dnnBlogAuthorActions">
        <li><asp:LinkButton ID="lnkAddEntry" runat="server" ResourceKey="lnkAddEntry" CssClass="dnnPrimaryAction" /></li>
        <li><asp:LinkButton ID="lnkEditBlog" runat="server" ResourceKey="lnkEditBlog" CssClass="dnnSecondaryAction" /></li>
        <li><asp:LinkButton ID="lnkViewBlog" runat="server" ResourceKey="lnkViewBlog" CssClass="dnnSecondaryAction" /></li>
    </ul>
    <div class="dnnRight">
        <a href="#" id="bmShow"><%= Localization.GetString("Show", LocalResourceFile)%></a>
        <a href="#" id="bmHide"><%= Localization.GetString("Hide", LocalResourceFile)%></a>
    </div>
</asp:Panel>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupBloggerMenu() {
            $("#dnnBlogAuthorActions").hide();
            $("#bmHide").hide();

            $("#bmShow").click(function () {
                $("#dnnBlogAuthorActions").show('highlight', '', 200, '');
                $("#bmHide").show();
                $("#bmShow").hide();
                return false;
            });
            $("#bmHide").click(function () {
                $("#bmShow").show();
                $("#dnnBlogAuthorActions").hide('highlight', '', 200, '');
                $("#bmHide").hide();
                return false;
            });
        }

        $(document).ready(function () {
            setupBloggerMenu();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupBloggerMenu();
            });
        });

    } (jQuery, window.Sys));
</script>  