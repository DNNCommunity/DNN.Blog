<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Comments.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.Comments" %>
<%@ Register TagPrefix="blog" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog.Templating" %>

<div id="blog_allcomments<%=ModuleId %>">
<blog:ViewTemplate runat="server" id="vtContents" />
</div>


<script>
var commentParentId = -1;
var $dialogComment;
(function ($, Sys) {
 $(document).ready(function () {
 <% If Security.CanAddComment %>
  $dialogComment = $('<div class="dnnDialog"></div>')
		.html('<div><%=LocalizeString("Comment") %></div><div><textarea rows="5" id="txtComment" style="width:95%"></textarea></div>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeString("CommentTitle") %>',
		 width: 500,
		 open: function (e) {
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeString("Submit") %>")').addClass('dnnPrimaryAction');
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
     text: '<%=LocalizeString("Submit") %>',
     click: function () {
      $(this).dialog("close");
      blogService.addComment(<%=BlogId %>, <%=ContentItemId %>, commentParentId, $('#txtComment').val(), '', '', '', function () {
       blogService.getCommentsHtml(<%=BlogId %>, <%=ContentItemId %>, function(data) {
        $('#blog_allcomments<%=ModuleId %>').html(data.Result);
       });
      });
     }
    }
    ]
		});
  <% End If %>
 });
} (jQuery, window.Sys));
</script>
