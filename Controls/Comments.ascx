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
		.html('<div><%=LocalizeString("Comment") %></div><div><textarea rows="5" id="txtComment<%=ModuleId %>" style="width:95%"></textarea></div><% If Not Security.LoggedIn
   %><%=LocalizeString("Name") %><br/><input type="text" id="txtName<%=ModuleId %>" style="width:95%"></input><%=LocalizeString("Email") %><br/><input type="text" id="txtEmail<%=ModuleId %>" style="width:95%"></input><%=LocalizeString("Website") %><br/><input type="text" id="txtWebsite<%=ModuleId %>" style="width:95%"></input><% 
   End If %><div id="commentError<%=ModuleId %>" class="dnnFormMessage dnnFormError"></div>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeString("CommentTitle") %>',
		 width: 500,
		 open: function (e) {
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeString("Submit") %>")').addClass('dnnPrimaryAction');
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeString("Cancel") %>")').addClass('dnnSecondaryAction');
    $('#commentError<%=ModuleId %>').hide();
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
      if ($('#txtComment<%=ModuleId %>').val().trim()=='') {$('#commentError<%=ModuleId %>').html('<%=LocalizeString("RemarksRequired.Error") %>').show();return;};
      <% If Not Security.LoggedIn %>
      if ($('#txtName<%=ModuleId %>').val().trim()=='') {$('#commentError<%=ModuleId %>').html('<%=LocalizeString("NameRequired.Error") %>').show();return;};
      if ($('#txtEmail<%=ModuleId %>').val().trim()=='') {$('#commentError<%=ModuleId %>').html('<%=LocalizeString("EmailRequired.Error") %>').show();return;};
      var emailRegex = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
      if (!$('#txtEmail<%=ModuleId %>').val().match(emailRegex)) {$('#commentError<%=ModuleId %>').html('<%=LocalizeString("EmailRequired.Error") %>').show();return;};
      <% End If %>
      $(this).dialog("close");
      <% If Not Security.LoggedIn %>
       blogService.addComment(<%=BlogId %>, <%=ContentItemId %>, commentParentId, $('#txtComment<%=ModuleId %>').val(), $('#txtName<%=ModuleId %>').val(), $('#txtWebsite<%=ModuleId %>').val(), $('#txtEmail<%=ModuleId %>').val(), function (data) {
      <% ELse %>
       blogService.addComment(<%=BlogId %>, <%=ContentItemId %>, commentParentId, $('#txtComment<%=ModuleId %>').val(), '', '', '', function (data) {
      <% End If %>
       if (data.Result=='successnotapproved') {
        alert('<%=LocalizeString("NotApproved") %>');
       } else {
        blogService.getCommentsHtml(<%=BlogId %>, <%=ContentItemId %>, function(data) {
         $('#blog_allcomments<%=ModuleId %>').html(data.Result);
        });
       }
      });
     }
    }
    ]
		});
  <% End If %>
 });
} (jQuery, window.Sys));
</script>
