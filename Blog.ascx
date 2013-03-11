<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Blog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Blog" %>
<%@ Register TagPrefix="blog" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog.Templating" %>
<%@ Register TagPrefix="blog" TagName="comments" Src="controls/Comments.ascx" %>

<div style="height:40px">
<asp:LinkButton runat="server" ID="cmdManageBlogs" resourcekey="cmdManageBlogs" Visible="false" CssClass="dnnSecondaryAction" />
<asp:LinkButton runat="server" ID="cmdEditPost" resourcekey="cmdEditPost" Visible="false" CssClass="dnnSecondaryAction" />
<asp:LinkButton runat="server" ID="cmdBlog" resourcekey="cmdBlog" Visible="false" CssClass="dnnPrimaryAction" />&nbsp;
 <a href="#" id="wlwlink" title="<%=LocalizeString("WLW") %>" class="icon16 entypoButton" style="display:<%=IIF(Security.CanAddEntry, "inline", "none")%>;margin-top:-24px;float:right">&#59290;</a>
 <a href="#" id="rsslink" title="<%=LocalizeString("RSS") %>" class="icon16 entypoButton" style="display:inline;margin-top:-24px;float:right">&#59194;</a>
 <a href="#" id="searchlink" title="<%=LocalizeString("Search") %>" class="icon16 entypoButton" style="display:inline;margin-top:-24px;float:right">&#128269;</a>
</div>
<blog:ViewTemplate runat="server" id="vtContents" />

<script>
 $(document).ready(function () {
  var $dialogWLW = $('<div class="dnnDialog"></div>')
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
   $dialogWLW.dialog('open');
   $('#txtWLWLink').val('http://<%= Request.Url.Host & ControlPath & String.Format("blogpost.ashx?portalid={0}&tabid={1}&moduleid={2}", PortalId, TabId, ModuleId) %>').select();
   return false;
  });
  var $dialogSearch = $('<div class="dnnDialog"></div>')
		.html('<input type="text" id="txtSearch" style="width:95%"></input><br/><%=LocalizeString("SearchIn") %>&nbsp;<input type="checkbox" id="scopeTitle" value="1" checked="1" /><%=LocalizeString("Title") %><input type="checkbox" id="scopeContents" value="1" /><%=LocalizeString("Contents") %>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeString("Search") %>',
		 height: 260,
		 width: 500,
		 open: function (e) {
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeString("Search") %>")').addClass('dnnPrimaryAction');
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeString("Cancel") %>")').addClass('dnnSecondaryAction');
		 },
		 buttons: [
    {
     text: '<%=LocalizeString("Cancel") %>',
     click: function () {
      $(this).dialog("close");
     }
    },
    {
     text: '<%=LocalizeString("Search") %>',
     click: function () {
      $(this).dialog("close");
      var url = '<%=BaseUrlPlusEnding %>';
      url += 'search=' + $('#txtSearch').val();
      if ($('#scopeTitle').is(':checked')) {
       url += '&t=1'
      }
      if ($('#scopeContents').is(':checked')) {
       url += '&c=1'
      }
      window.location.href = encodeURI(url);
     }
    }
    ]
		});
  $('#searchlink').click(function () {
   $dialogSearch.dialog('open');
   return false;
  });
 });
</script>

<blog:comments runat="server" id="ctlComments" />
