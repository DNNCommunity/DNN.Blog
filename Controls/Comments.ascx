<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Comments.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.Comments" %>
<%@ Register TagPrefix="blog" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog.Templating" %>

<blog:ViewTemplate runat="server" id="vtContents" />

<div id="blogServiceErrorBox">
</div>

<script language="javascript" type="text/javascript">
 jQuery(function ($) {
  $("abbr.commenttimeago").timeago();
 } (jQuery, window.Sys));
</script>
