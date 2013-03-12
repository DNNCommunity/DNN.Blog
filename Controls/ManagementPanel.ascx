<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManagementPanel.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.ManagementPanel" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="height:40px" id="pnlManagement" runat="server">
 <a href="#" id="cmdCopyModule" runat="server" class="dnnSecondaryAction" visible="false"><%=LocalizeString("cmdCopyModule")%></a>
 <asp:LinkButton runat="server" ID="cmdManageBlogs" resourcekey="cmdManageBlogs" Visible="false" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdEditPost" resourcekey="cmdEditPost" Visible="false" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdBlog" resourcekey="cmdBlog" Visible="false" CssClass="dnnPrimaryAction" />&nbsp;
 <a href="#" id="wlwlink" runat="server" class="icon16 entypoButton" style="margin-top:-24px;float:right">&#59290;</a>
 <a href="#" id="rsslink<%=ModuleId %>" title="<%=LocalizeString("RSS") %>" class="icon16 entypoButton" style="display:inline;margin-top:-24px;float:right">&#59194;</a>
 <a href="#" id="searchlink<%=ModuleId %>" title="<%=LocalizeString("Search") %>" class="icon16 entypoButton" style="display:inline;margin-top:-24px;float:right">&#128269;</a>
</div>
<div class="dnnDialog" id="pnlCopyModule" runat="server">
 <div class="dnnClear">
  <div class="dnnFormItem">
   <dnn:label id="lblTemplate" runat="server" controlname="ddTemplate" suffix=":" />
   <asp:DropDownList runat="server" ID="ddTemplate" />
  </div>			
  <div class="dnnFormItem">
   <dnn:Label ID="lblTitle" runat="server" Suffix=":" />
   <asp:TextBox runat="server" ID="txtTitle" />
  </div>
  <div class="dnnFormItem">
   <dnn:Label ID="lblPane" runat="server" Suffix=":" />
   <asp:DropDownList runat="server" ID="ddPane" />
  </div>
  <div class="dnnFormItem">
   <dnn:Label ID="lblInsert" runat="server" Suffix=":" />
   <asp:DropDownList runat="server" ID="ddPosition" />    
  </div>
  <div class="dnnRight">
   <a href="#" class="dnnSecondaryAction" onclick="$('#<%=pnlCopyModule.ClientId %>').dialog('close')"><%=LocalizeString("cmdCancel") %></a>
   <a href="<%= NavigateUrl() %>" id="cmdAdd<%=ModuleId %>" class="dnnPrimaryAction"><%=LocalizeString("cmdAdd") %></a>
  </div>
 </div>
</div>
<script>
(function ($, Sys) {
 $(document).ready(function () {
<% If Security.CanAddEntry %>
  var $dialogWLW = $('<div class="dnnDialog"></div>')
		.html('<input type="text" id="txtWLWLink<%=ModuleId %>" style="width:95%"></input><br/><span><%=LocalizeString("WLW.Help") %></span>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeString("WLW") %>',
		 height: 160,
		 width: 500
		});
		$('#<%=wlwlink.ClientId %>').click(function () {
   $dialogWLW.dialog('open');
   $('#txtWLWLink<%=ModuleId %>').val('http://<%= Request.Url.Host & ControlPath & String.Format("blogpost.ashx?portalid={0}&tabid={1}&moduleid={2}", PortalId, TabId, ModuleId) %>').select();
   return false;
  });
<% End If %>
<% If Security.IsEditor %>
 $('#<%=pnlCopyModule.ClientId %>')
		.dialog({
		 autoOpen: false,
		 resizable: false,
		 dialogClass: 'dnnFormPopup dnnClear',
		 title: '<%=LocalizeString("cmdCopyModule") %>',
		 width: 800
		});
		$('#<%=cmdCopyModule.ClientId %>').click(function () {
   $('#<%=pnlCopyModule.ClientId %>').dialog('open');
   return false;
  });
 $('#cmdAdd<%=ModuleId %>').click(function () {
  blogService.addModule($('#<%=ddPane.ClientId %>').val(), $('#<%=ddPosition.ClientId %>').val(), $('#<%=txtTitle.ClientId %>').val(), $('#<%=ddTemplate.ClientId %>').val());
 });
<% End If %>
  var $dialogSearch = $('<div class="dnnDialog"></div>')
		.html('<input type="text" id="txtSearch" style="width:95%"></input><br/><%=LocalizeString("SearchIn") %>&nbsp;<input type="checkbox" id="scopeTitle<%=ModuleId %>" value="1" checked="1" /><%=LocalizeString("Title") %><input type="checkbox" id="scopeContents<%=ModuleId %>" value="1" /><%=LocalizeString("Contents") %>')
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
      var url = '<%=ModuleUrls.GetUrl(True, True, True, True) %>';
      url += 'search=' + $('#txtSearch').val();
      if ($('#scopeTitle<%=ModuleId %>').is(':checked')) {
       url += '&t=1'
      }
      if ($('#scopeContents<%=ModuleId %>').is(':checked')) {
       url += '&c=1'
      }
      window.location.href = encodeURI(url);
     }
    }
    ]
		});
  $('#searchlink<%=ModuleId %>').click(function () {
   $dialogSearch.dialog('open');
   return false;
  });
 });
} (jQuery, window.Sys));
</script>
