<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Manage.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Manage" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm" id="tabs">
 <ul class="dnnAdminTabNav">
  <li runat="server" id="SettingsHeader"><a href="#<%=SettingsTab.ClientId%>">Settings</a></li>
  <li><a href="#Blogs">Blogs</a></li>
  <li><a href="#Posts">Posts</a></li>
  <li><a href="#Categories">Categories</a></li>
 </ul>
 <div id="SettingsTab" class="dnnClear" runat="server">
<h2 id="H1" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("General")%></a></h2>
<fieldset>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowMultipleCategories" runat="server" controlname="chkAllowMultipleCategories" suffix=":" />
		<asp:CheckBox ID="chkAllowMultipleCategories" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblSummaryModel" runat="server" controlname="ddSummaryModel" suffix=":" />
  <asp:DropDownList ID="ddSummaryModel" runat="server">
   <asp:ListItem Value="0" ResourceKey="PlainTextIndependent.Opt" />
   <asp:ListItem Value="1" ResourceKey="HtmlIndependent.Opt" />
   <asp:ListItem Value="2" ResourceKey="HtmlPrecedesPost.Opt" />
  </asp:DropDownList>
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowAttachments" runat="server" controlname="chkAllowAttachments" suffix=":" />
		<asp:CheckBox ID="chkAllowAttachments" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowWLW" runat="server" controlname="chkAllowWLW" suffix=":" />
		<asp:CheckBox ID="chkAllowWLW" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblVocabularyId" runat="server" controlname="ddVocabularyId" suffix=":" />
		<asp:DropDownList ID="ddVocabularyId" runat="server" DataValueField="VocabularyID" DataTextField="Name" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblBloggersCanEditCategories" runat="server" controlname="chkBloggersCanEditCategories" suffix=":" />
		<asp:CheckBox ID="chkBloggersCanEditCategories" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblWLWRecentPostsMax" runat="server" controlname="txtWLWRecentPostsMax" suffix=":" />
		<asp:TextBox runat="server" ID="txtWLWRecentPostsMax" />
	</div>
</fieldset>
<h2 id="H2" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("Localization")%></a></h2>
<fieldset>
	<div class="dnnFormItem">
		<dnn:label id="lblLocalizationModel" runat="server" controlname="ddLocalizationModel" suffix=":" />
  <asp:DropDownList ID="ddLocalizationModel" runat="server">
   <asp:ListItem Value="0" ResourceKey="None.Opt" />
   <asp:ListItem Value="1" ResourceKey="Loose.Opt" />
   <asp:ListItem Value="2" ResourceKey="Strict.Opt" />
  </asp:DropDownList>
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowAllLocales" runat="server" controlname="chkAllowAllLocales" suffix=":" />
		<asp:CheckBox ID="chkAllowAllLocales" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblShowAllLocales" runat="server" controlname="chkShowAllLocales" suffix=":" />
		<asp:CheckBox ID="chkShowAllLocales" runat="server" />
	</div>
	<div class="dnnFormItem">
  <asp:LinkButton runat="server" ID="cmdEditTagsML" resourcekey="cmdEditTagsML" CssClass="dnnSecondaryAction" />
	</div>
</fieldset>
<h2 id="H3" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("RSS")%></a></h2>
<fieldset>
	<div class="dnnFormItem">
		<dnn:label id="lblRssDefaultNrItems" runat="server" controlname="txtRssDefaultNrItems" suffix=":" />
		<asp:TextBox runat="server" ID="txtRssDefaultNrItems" Width="50" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssMaxNrItems" runat="server" controlname="txtRssMaxNrItems" suffix=":" />
		<asp:TextBox runat="server" ID="txtRssMaxNrItems" Width="50" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssTtl" runat="server" controlname="txtRssTtl" suffix=":" />
		<asp:TextBox runat="server" ID="txtRssTtl" Width="50" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblEmail" runat="server" controlname="txtEmail" suffix=":" />
		<asp:TextBox runat="server" ID="txtEmail" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssDefaultCopyright" runat="server" controlname="txtRssDefaultCopyright" suffix=":" />
		<asp:TextBox runat="server" ID="txtRssDefaultCopyright" Width="50" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssAllowContentInFeed" runat="server" controlname="chkRssAllowContentInFeed" suffix=":" />
		<asp:CheckBox ID="chkRssAllowContentInFeed" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssImageWidth" runat="server" controlname="txtRssImageWidth" suffix=":" />
		<asp:TextBox runat="server" ID="txtRssImageWidth" Width="50" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssImageHeight" runat="server" controlname="txtRssImageHeight" suffix=":" />
		<asp:TextBox runat="server" ID="txtRssImageHeight" Width="50" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblRssImageSizeAllowOverride" runat="server" controlname="chkRssImageSizeAllowOverride" suffix=":" />
		<asp:CheckBox ID="chkRssImageSizeAllowOverride" runat="server" />
	</div>
</fieldset>
 </div>
 <div id="Blogs" class="dnnClear">
  <asp:DataList runat="server" ID="dlBlogs">
   <HeaderTemplate>
    <table class="dnnGrid">
     <tr class="dnnGridHeader">
      <td></td>
      <td></td>
      <td></td>
      <td>Blog</td>
      <td>Owner</td>
     </tr>
   </HeaderTemplate>
   <ItemTemplate>
    <tr class="dnnGridItem">
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogEdit") %>" class="icon16 entypoButton" title="Edit">&#9998;</a></td>
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogImport") %>" class="icon16 entypoButton" title="Import">&#59200;</a></td>
     <td><a href="#" class="icon16 entypoButton exportlink" title="Export" data-blogid="<%# Eval("BlogId") %>">&#59201;</a></td>
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
          class="icon16 entypoButton" 
          title="Edit"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanEdit, "inline", "none") %>">&#9998;</a>
       <a href="#" 
          onclick="if (confirm('<%= LocalizeString("DeletePost") %>')) {blogService.deletePost(<%# Eval("BlogID") %>, <%# Eval("ContentItemID") %>, function() {$('#cmdDeletePost<%# Eval("ContentItemID") %>').parent().parent().hide()})};return false;"
          id="cmdDeletePost<%# Eval("ContentItemID") %>"
          class="icon16 entypoButton" 
          title="Delete"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanEdit, "inline", "none") %>">&#59177;</a>
       <a href="#" 
          onclick="if (confirm('<%= LocalizeString("ApprovePost") %>')) {blogService.approvePost(<%# Eval("BlogID") %>, <%# Eval("ContentItemID") %>, function() {$('#cmdApprovePost<%# Eval("ContentItemID") %>').hide();$('#approveTick<%# Eval("ContentItemID") %>').text('&#10003;')})};return false;" 
          id="cmdApprovePost<%# Eval("ContentItemID") %>"
          class="icon16 entypoButton" 
          title="Approve"
          style="display:<%# IIF(CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Blog.CanApprove AND NOT CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Posts.PostInfo).Published, "inline", "none") %>">&#128077;</a>
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
       <span class="entypoIcon icon16" id="approveTick<%# Eval("ContentItemID") %>"><%# IIf(Eval("Published"), "&#10003;", "&#10060;")%></span>
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
 <div id="Categories" class="dnnClear">
  <div class="dnnLeft">
   <textarea id="txtNewCategories" rows="10" cols="60"></textarea><br />
   <button class="dnnSecondaryAction" id="btnAddCategories">Add</button>
   <button class="dnnSecondaryAction" id="btnDeleteCategory">Delete Selected</button>
  </div>
  <div id="categoryTree" class="dnnLeft">
  </div>
  <asp:HiddenField runat="server" ID="treeState" />
 </div>
</div>
<p class="updatecancelbar">
 <asp:LinkButton runat="server" ID="cmdCancel" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdUpdate" resourcekey="cmdUpdate" CssClass="dnnPrimaryAction" />
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
 $('#btnDeleteCategory').click(function () {
   $("#categoryTree").dynatree("getActiveNode").remove();
   $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
  return false;
 });
 $('#btnAddCategories').click(function () {
  var rootNode = $("#categoryTree").dynatree("getRoot");
  var lines = $('#txtNewCategories').val().split('\n');
  $.each(lines, function (n, elem) {
   if ($.trim(elem) != '') {
    var childNode = rootNode.addChild({
     title: $.trim(elem),
     key: "-1",
     icon: false,
     isFolder: true
    });
   };
  });
  $('#txtNewCategories').val('');
  $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
  return false;
 });
 function editNode(node) {
  var prevTitle = node.data.title,
  tree = node.tree;
  tree.$widget.unbind();
  $(".dynatree-title", node.span).html("<input id='editNode' value='" + prevTitle + "'>");
  $("input#editNode")
    .focus()
    .keydown(function (event) {
     switch (event.which) {
      case 27: // [esc]
       $("input#editNode").val(prevTitle);
       $(this).blur();
       break;
      case 13: // [enter]
       $(this).blur();
       break;
     }
    }).blur(function (event) {
     var title = $("input#editNode").val();
     node.setTitle(title);
     tree.$widget.bind();
     node.focus();
     $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
    });
 }
 $(document).ready(function () {
  $('#categoryTree').dynatree({
   checkbox: false,
   children: $.parseJSON($('#<%= treeState.ClientID %>').val()),
   dnd: {
    onDragStart: function (node) {
     return true;
    },
    onDragStop: function (node) {
    },
    autoExpandMS: 1000,
    preventVoidMoves: true,
    onDragEnter: function (node, sourceNode) {
     return true;
    },
    onDragOver: function (node, sourceNode, hitMode) {
     if (node.isDescendantOf(sourceNode)) {
      return false;
     }
     if (!node.data.isFolder && hitMode === "over") {
      return "after";
     }
    },
    onDrop: function (node, sourceNode, hitMode, ui, draggable) {
     sourceNode.move(node, hitMode);
     $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
    },
    onDragLeave: function (node, sourceNode) {
    }
   },
   onClick: function (node, event) {
    if (event.shiftKey) {
     editNode(node);
     $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
     return false;
    }
   },
   onDblClick: function (node, event) {
    editNode(node);
    $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
    return false;
   }
  })
 });
} (jQuery, window.Sys));
</script>
