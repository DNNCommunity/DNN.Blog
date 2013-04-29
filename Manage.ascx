<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Manage.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Manage" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm" id="tabs">
 <ul class="dnnAdminTabNav">
  <li><a href="#Blogs"><%= LocalizeString("Blogs")%></a></li>
  <li><a href="#Posts"><%= LocalizeString("Posts")%></a></li>
 </ul>
 <div id="Blogs" class="dnnClear">
  <asp:DataList runat="server" ID="dlBlogs">
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
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogEdit") %>" class="blogicon-edit icon16" title="Edit"></a></td>
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogImport") %>" class="blogicon-download-alt icon16" title="Import"></a></td>
     <td><a href="#" class="blogicon-upload-alt icon16 exportlink" title="Export" data-blogid="<%# Eval("BlogId") %>"></a></td>
     <td><%# Eval("Title") %></td>
     <td><%# Eval("DisplayName") %></td>
    </tr>
   </ItemTemplate>
   <FooterTemplate>
    </table>
   </FooterTemplate>
  </asp:DataList>
  <p>
   <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CssClass="dnnSecondaryAction" />
  </p>
 </div>
 <div id="Posts" class="dnnClear">
  <div class="coreMessaging" id="blogPostsError"></div>
  <dnn:DNNGrid id="grdPosts" autogeneratecolumns="false" cssclass="dnnGrid dnnSecurityRolesGrid"
   runat="server" allowpaging="True" allowcustompaging="True" enableviewstate="True"
   onneeddatasource="GetPosts">
   <MasterTableView>
    <Columns>
     <dnn:DnnGridTemplateColumn HeaderText="Actions">
      <ItemStyle Width="90px"></ItemStyle>
      <ItemTemplate>
       <a href="<%# EditUrl("Post", Eval("ContentItemId"), "PostEdit") %>" 
          class="blogicon-edit icon16" 
          title="Edit"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanEdit, "inline", "none") %>"></a>
       <a href="#" 
          onclick="if (confirm('<%= LocalizeString("DeletePost") %>')) {blogService.deletePost(<%# Eval("BlogID") %>, <%# Eval("ContentItemID") %>, function() {$('#cmdDeletePost<%# Eval("ContentItemID") %>').parent().parent().hide()})};return false;"
          id="cmdDeletePost<%# Eval("ContentItemID") %>"
          class="blogicon-remove icon16" 
          title="Delete"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanEdit, "inline", "none") %>"></a>
       <a href="#" 
          onclick="if (confirm('<%= LocalizeString("ApprovePost") %>')) {blogService.approvePost(<%# Eval("BlogID") %>, <%# Eval("ContentItemID") %>, function() {$('#cmdApprovePost<%# Eval("ContentItemID") %>').hide();$('#approveTick<%# Eval("ContentItemID") %>').text('&#10003;')})};return false;" 
          id="cmdApprovePost<%# Eval("ContentItemID") %>"
          class="blogicon-ok icon16" 
          title="Approve"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanApprove AND NOT CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Published, "inline", "none") %>"></a>
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridTemplateColumn HeaderText="Date">
      <ItemTemplate>
       <asp:Label ID="Label2" Runat="server" Text='<%# CDate(Eval("PublishedOnDate")).ToString("d") %>' ToolTip='<%# CDate(Eval("PublishedOnDate")).ToString("U") %>' />
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridBoundColumn datafield="Title" headertext="Title"/>
     <dnn:DnnGridTemplateColumn HeaderText="Published">
      <ItemStyle Width="30px" HorizontalAlign="Center"></ItemStyle>
      <ItemTemplate>
       <span class="blogicon-<%# IIf(Eval("Published"), "ok", "remove")%> icon16" id="approveTick<%# Eval("ContentItemID") %>"></span>
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridTemplateColumn HeaderText="Blog">
      <ItemTemplate>
       <asp:Label ID="Label1" Runat="server" Text='<%# CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.Title %>' />
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
    </Columns>
   </MasterTableView>
  </dnn:DNNGrid>
 </div>
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
		.html('<p><%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Export.Help")) %></p><p><a id="blogMLDownloadLink" style=""><%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Download")) %></a></p>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Export")) %>',
		 height: 250,
		 width: 500,
		 open: function (e) {
		  $('#blogMLDownloadLink').hide();
		  $('.ui-dialog-buttonpane').find('button:contains("<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Export")) %>")').addClass('dnnPrimaryAction');
		  $('.ui-dialog-buttonpane').find('button:contains("<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Cancel")) %>")').addClass('dnnSecondaryAction');
		 },
		 buttons: [
    {
     text: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Cancel")) %>',
     click: function () {
      $(this).dialog("close");
     }
    },
    {
     text: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Export")) %>',
     click: function () {
      $('.ui-dialog-buttonpane').find('button:contains("<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Export")) %>")').attr("disabled", "disabled");
      blogService.exportBlog(selectedBlog, function (returnValue) {
       $('.ui-dialog-buttonpane').find('button:contains("<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Export")) %>")').removeAttr("disabled");
       $('#blogMLDownloadLink').attr('href', returnValue);
       $('#blogMLDownloadLink').show();
      });
     }
    }
    ]
		});
 $('.exportlink').click(function () {
  selectedBlog = $(this).attr('data-blogid');
  $dialogexport.dialog('open');
  return false;
 });
} (jQuery, window.Sys));
</script>
