<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>

<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupBloggerMenu() {

        }

        $(document).ready(function () {
            setupBloggerMenu();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupBloggerMenu();
            });
        });

    } (jQuery, window.Sys));
</script>  