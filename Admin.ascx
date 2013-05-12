<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Admin.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Admin" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm" id="tabs">
 <ul class="dnnAdminTabNav">
  <li><a href="#SettingsTab"><%= LocalizeString("Settings")%></a></li>
  <li><a href="#Categories"><%= LocalizeString("Categories")%></a></li>
 </ul>
 <div id="SettingsTab">
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
		<asp:DropDownList ID="ddVocabularyId" runat="server" DataValueField="VocabularyID" DataTextField="Name" AutoPostBack="true" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblWLWRecentPostsMax" runat="server" controlname="txtWLWRecentPostsMax" suffix=":" />
		<asp:TextBox runat="server" ID="txtWLWRecentPostsMax" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblModifyPageDetails" runat="server" controlname="chkModifyPageDetails" suffix=":" />
		<asp:CheckBox ID="chkModifyPageDetails" runat="server" />
	</div>
</fieldset>
<h2 id="H2" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("Localization")%></a></h2>
<fieldset>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowAllLocales" runat="server" controlname="chkAllowAllLocales" suffix=":" />
		<asp:CheckBox ID="chkAllowAllLocales" runat="server" />
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
 <div id="Categories" class="dnnClear">
  <% If Settings.VocabularyId > -1 %>
   <div id="categoryTree">
   </div><br />
   <button class="dnnSecondaryAction" id="btnAddCategories"><%= LocalizeString("Add")%></button>
   <button class="dnnSecondaryAction" id="btnDeleteCategory"><%= LocalizeString("DeleteSelected")%></button>
   <asp:LinkButton runat="server" ID="cmdEditCategoriesML" resourcekey="cmdEditCategoriesML" CssClass="dnnSecondaryAction" />
  <% Else%>
   <div class="dnnFormMessage dnnFormWarning"><%= LocalizeString("NoVocabulary")%></div>
   <asp:LinkButton runat="server" ID="cmdCreateVocabulary" resourcekey="cmdCreateVocabulary" CssClass="dnnPrimaryAction" />
  <% End If %>
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
 var $dialogAddCategories;
 $('#btnDeleteCategory').click(function () {
   $("#categoryTree").dynatree("getActiveNode").remove();
   $('#<%= treeState.ClientID %>').val(JSON.stringify($("#categoryTree").dynatree("getRoot").toDict(true).children));
  return false;
 });
 <% If Settings.VocabularyId > -1 %>
  $dialogAddCategories = $('<div class="dnnDialog"></div>')
		.html('<div class="dnnFormMessage dnnFormWarning"><%=LocalizeJSString("AddCategories.Help") %></div><div><textarea rows="5" id="txtNewCategories" style="width:95%"></textarea></div>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeJSString("AddCategories.Title") %>',
		 width: 500,
		 open: function (e) {
		  $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Add") %>")').addClass('dnnPrimaryAction');
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
     text: '<%=LocalizeJSString("Add") %>',
     click: function () {
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
      $(this).dialog("close");
    }}
    ]
		});
  <% End If %>
  $('#btnAddCategories').click(function () {
   $dialogAddCategories.dialog('open');
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
