<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<%@ Register TagPrefix="blog" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog.Templating" %>
<%@ Register TagPrefix="blog" TagName="comments" Src="controls/Comments.ascx" %>

<asp:LinkButton runat="server" ID="cmdManageBlogs" resourcekey="cmdManageBlogs" Visible="false" CssClass="dnnSecondaryAction" />
<asp:LinkButton runat="server" ID="cmdBlog" resourcekey="cmdBlog" Visible="false" CssClass="dnnPrimaryAction" />
<a href="#" id="wlwlink" title="<%=LocalizeString("WLW.Link") %>" style="float:right"><img src="<%=ResolveUrl("~/DesktopModules/Blog/images/Windows_Live_Writer_logo.png")%>" width="16" height="16" border="0" /></a>

<blog:ViewTemplate runat="server" id="vtContents" />

<script>
 $(document).ready(function () {
  var $dialog = $('<div class="dnnDialog"></div>')
		.html('<input type="text" id="txtWLWLink" style="width:95%"></input><br/><span><%=LocalizeString("WLW.Help") %></span>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeString("WLW") %>',
		 height: 160,
		 width: 500
		});
  $('#wlwlink').click(function () {
   $dialog.dialog('open');
   $('#txtWLWLink').val('http://<%= Request.Url.Host & ControlPath & String.Format("blogpost.ashx?portalid={0}&tabid={1}&moduleid={2}", PortalId, TabId, ModuleId) %>').select();
   return false;
  });
 });
</script>

<blog:comments runat="server" id="ctlComments" />
