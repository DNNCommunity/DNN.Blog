<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Manage.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Manage" %>
<%@ Import Namespace="DotNetNuke.Entities.Users" %>
<div class="dnnForm" id="tabs">
 <ul class="dnnAdminTabNav">
  <li id="blogsLink" runat="server"><a href="#<%= Blogs.ClientId %>"><%= LocalizeString("Blogs")%></a></li>
  <li id="postsLink" runat="server"><a href="#<%= Posts.ClientId %>"><%= LocalizeString("Posts")%></a></li>
 </ul>
 <asp:Panel id="Blogs" runat="server" CssClass="dnnClear">
  <asp:Repeater runat="server" ID="dlBlogs" >
   <HeaderTemplate>
    <table class="dnnGrid">
     <tr class="dnnGridHeader">
      <td></td>
      <td></td>
      <td></td>
      <td><%= LocalizeString("Blog")%></td>
      <td><%= LocalizeString("Owner")%></td>
     </tr>
   </HeaderTemplate>
   <ItemTemplate>
    <tr class="dnnGridItem">
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogEdit") %>" title="Edit"><i class="fa fa-pencil-square-o icon16"></i></a></td>
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogImport") %>" title="Import"><i class="fa fa-download icon16"></i></a></td>
     <td><a href="#" title="Export" data-blogid="<%# Eval("BlogId") %>"><i class="fa fa-upload icon16 exportlink"></i></a></td>
     <td><%# Eval("Title") %></td>
     <td><%# Eval("DisplayName") %></td>
    </tr>
   </ItemTemplate>
   <FooterTemplate>
    </table>
   </FooterTemplate>
  </asp:Repeater>
  <p>
   <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CssClass="dnnSecondaryAction" />
  </p>
 </asp:Panel>
 <asp:Panel id="Posts" runat="server" CssClass="dnnClear">
  <div class="coreMessaging" id="blogPostsError"></div>
  <asp:GridView id="grdPosts" autogeneratecolumns="false" cssclass="dnnGrid dnnSecurityRolesGrid"
   runat="server" allowpaging="True" allowcustompaging="True" enableviewstate="True" AllowSorting="true">
    <Columns>
     <asp:TemplateField HeaderText="Actions">
      <ItemStyle Width="90px"></ItemStyle>
      <ItemTemplate>
       <a href="<%# EditUrl("Post", Eval("ContentItemId"), "PostEdit") %>"
          title="Edit"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanEdit OR BlogContext.Security.CanEditThisPost(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo)), "inline", "none") %>"><i class="fa fa-pencil-square-o icon16"></i></a>
       <a href="#" 
          onclick="if (confirm('<%= LocalizeJSString("DeletePost") %>')) {blogService.deletePost(<%# Eval("BlogID") %>, <%# Eval("ContentItemID") %>, function() {$('#cmdDeletePost<%# Eval("ContentItemID") %>').parent().parent().hide()})};return false;"
          id="cmdDeletePost<%# Eval("ContentItemID") %>"
          title="Delete"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanEdit OR BlogContext.Security.CanEditThisPost(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo)), "inline", "none") %>"><i class="fa fa-times icon16"></i></a>
       <a href="#" 
          onclick="if (confirm('<%= LocalizeJSString("ApprovePost") %>')) {blogService.approvePost(<%# Eval("BlogID") %>, <%# Eval("ContentItemID") %>, function() {$('#cmdApprovePost<%# Eval("ContentItemID") %>').hide();$('#approveTick<%# Eval("ContentItemID") %>').removeClass('fa fa-times icon16').addClass('fa fa-check icon16') })};return false;" 
          id="cmdApprovePost<%# Eval("ContentItemID") %>"
          title="Approve"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanApprove AND NOT CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Published, "inline", "none") %>"><i class="fa fa-check icon16"></i></a>
      </ItemTemplate>
     </asp:TemplateField>
     <asp:TemplateField headertext="Date" SortExpression="PublishedOnDate">
      <ItemTemplate>
       <%# DotNetNuke.Modules.Blog.Common.Globals.UtcToLocalTime(Eval("PublishedOnDate"), UserController.GetCurrentUserInfo().Profile.PreferredTimeZone)%>
      </ItemTemplate>
     </asp:TemplateField>
     <asp:BoundField datafield="Title" headertext="Title" SortExpression="Title" />
     <asp:TemplateField HeaderText="Published">
      <ItemStyle Width="30px" HorizontalAlign="Center"></ItemStyle>
      <ItemTemplate>
       <i class="fa fa-<%# IIf(Eval("Published"), "check", "times")%> icon16" id="approveTick<%# Eval("ContentItemID") %>"></i>
      </ItemTemplate>
     </asp:TemplateField>
     <asp:TemplateField HeaderText="Blog">
      <ItemTemplate>
       <asp:Label ID="Label1" Runat="server" Text='<%# CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.Title %>' />
      </ItemTemplate>
     </asp:TemplateField>
    </Columns>
  </asp:GridView>
 </asp:Panel>
</div>
<p class="updatecancelbar">
 <asp:LinkButton runat="server" ID="cmdReturn" resourcekey="cmdReturn" CssClass="dnnPrimaryAction" />
</p>

<div id="blogServiceErrorBox">
</div>

<script type="text/javascript">
(function ($, Sys) {
 $('#tabs').dnnTabs();
 var selectedBlog;
 var $dialogexport = $('<div class="dnnDialog"></div>')
		.html('<p><%=LocalizeJSString("Export.Help") %></p><p><a id="blogMLDownloadLink" style=""><%=LocalizeJSString("Download") %></a></p>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeJSString("Export") %>',
		 height: 250,
		 width: 500,
		 open: function (e) {
		  $('#blogMLDownloadLink').hide();
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Export") %>")').addClass('dnnPrimaryAction');
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Cancel") %>")').addClass('dnnSecondaryAction');
		 },
		 buttons: [
    {
     text: '<%=LocalizeJSString("Cancel") %>',
     click: function () {
      $(this).dialog("close");
     }
    },
    {
     text: '<%=LocalizeJSString("Export") %>',
     click: function () {
      $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Export") %>")').attr("disabled", "disabled");
      blogService.exportBlog(selectedBlog, function (returnValue) {
       $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Export") %>")').removeAttr("disabled");
       $('#blogMLDownloadLink').attr('href', returnValue.Result);
       $('#blogMLDownloadLink').show();
      });
     }
    }
    ]
		});
 $('.exportlink').click(function () {
  selectedBlog = $(this).parent().attr('data-blogid');
  $dialogexport.dialog('open');
  return false;
 });
} (jQuery, window.Sys));
</script>