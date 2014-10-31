<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManagementPanel.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.ManagementPanel" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="height:40px" id="pnlManagement" runat="server">
 <a href="#" id="cmdCopyModule" runat="server" class="dnnSecondaryAction" visible="false"><%=LocalizeString("cmdCopyModule")%></a>
 <asp:LinkButton runat="server" ID="cmdManageBlogs" resourcekey="cmdManageBlogs" Visible="false" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdAdmin" resourcekey="cmdAdmin" Visible="false" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdEditPost" resourcekey="cmdEditPost" Visible="false" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdBlog" resourcekey="cmdBlog" Visible="false" CssClass="dnnPrimaryAction" />&nbsp;
 <div style="float:right">
  <a href="#" id="doclink" runat="server" visible="false"><i class="fa fa-book fa-fw icon16"></i>&nbsp;</a>
  <a href="<%=DotNetNuke.Common.Globals.NavigateUrl()%>" id="homelink" title="<%=LocalizeString("Home") %>"><i class="fa fa-home fa-fw icon16"></i>&nbsp;</a>
  <a href="#" id="wlwlink" runat="server"><i class="fa fa-pencil fa-fw icon16"></i>&nbsp;</a>
  <a href="<%=RssLink%>" id="rsslink<%=ModuleId %>" title="<%=LocalizeString("RSS") %>" target="_blank"><i class="fa fa-rss fa-fw icon16"></i>&nbsp;</a>
  <a href="#" id="searchlink<%=ModuleId %>" title="<%=LocalizeString("Search") %>"><i class="fa fa-search fa-fw icon16"></i>&nbsp;</a>
 </div>
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
   <a href="#" id="cmdAdd<%=ModuleId %>" class="dnnPrimaryAction"><%=LocalizeString("cmdAdd") %></a>
  </div>
 </div>
</div>
<script>
    (function ($, Sys) {
        $(document).ready(function () {
<% If BlogContext.Security.CanAddPost %>
     var $dialogWLW = $('<div class="dnnDialog"></div>')
           .html('<input type="text" id="txtWLWLink<%=ModuleId %>" style="width:95%"></input><br/><span><%=LocalizeJSString("WLW.Help") %></span>')
		.dialog({
		    autoOpen: false,
		    resizable: false,
		    dialogClass: 'dnnFormPopup dnnClear',
		    title: '<%=LocalizeJSString("WLW") %>',
		 height: 160,
		 width: 500
		});
     $('#<%=wlwlink.ClientId %>').click(function () {
         $dialogWLW.dialog('open');
         $('#txtWLWLink<%=ModuleId %>').val('http://<%= Request.Url.Host & DotNetNuke.Common.Globals.ApplicationPath & String.Format("/DesktopModules/Blog/BlogPost.ashx?portalid={0}&tabid={1}&moduleid={2}", PortalId, TabId, ModuleId) %>').select();
   return false;
		});
     <% End If %>
<% If BlogContext.Security.IsEditor %>
     $('#<%=pnlCopyModule.ClientId %>')
            .dialog({
                autoOpen: false,
                resizable: false,
                dialogClass: 'dnnFormPopup dnnClear',
                title: '<%=LocalizeJSString("cmdCopyModule") %>',
		 width: 800
		});
     $('#<%=cmdCopyModule.ClientId %>').click(function () {
         $('#<%=pnlCopyModule.ClientId %>').dialog('open');
		    return false;
		});
     $('#cmdAdd<%=ModuleId %>').click(function () {
         blogService.addModule($('#<%=ddPane.ClientId %>').val(), $('#<%=ddPosition.ClientId %>').val(), $('#<%=txtTitle.ClientId %>').val(), $('#<%=ddTemplate.ClientId %>').val(),
     function () {
         location.reload()
     });
     $('#<%=pnlCopyModule.ClientId %>').dialog('close');
     return false;
 });
<% End If %>
     var $dialogSearch = $('<div class="dnnDialog"></div>')
           .html('<input type="text" id="txtSearch" style="width:95%"></input><br/><%=LocalizeJSString("SearchIn") %>&nbsp;<input type="checkbox" id="scopeAll<%=ModuleId %>" value="1" checked="1" /><%=LocalizeJSString("SearchAll") %><input type="checkbox" id="scopeTitle<%=ModuleId %>" value="1" checked="1" /><%=LocalizeJSString("Title") %><input type="checkbox" id="scopeContents<%=ModuleId %>" value="1" /><%=LocalizeJSString("Contents") %><% If BlogContext.Security.CanAddPost %><input type="checkbox" id="chkUnpublished<%=ModuleId %>" value="1" /><%=LocalizeJSString("Unpublished") %><% End If %>')
		.dialog({
		    autoOpen: false,
		    resizable: false,
		    dialogClass: 'dnnFormPopup dnnClear',
		    title: '<%=LocalizeJSString("Search") %>',
		 height: 210,
		 width: 500,
		 open: function (e) {
		     $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Search") %>")').addClass('dnnPrimaryAction');
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
        text: '<%=LocalizeJSString("Search") %>',
        click: function () {
            $(this).dialog("close");
            var url
            if ($('#scopeAll<%=ModuleId %>').is(':checked')) {
          url = '<%=BlogContext.ModuleUrls.GetUrl(False, False, False, False, True) %>';
      } else {
          url = '<%=BlogContext.ModuleUrls.GetUrl(True, False, True, True, True) %>';
      }
         url += 'search=' + $('#txtSearch').val();
         if ($('#scopeTitle<%=ModuleId %>').is(':checked')) {
          url += '&t=1'
      }
      if ($('#scopeContents<%=ModuleId %>').is(':checked')) {
             url += '&c=1'
         }
         if ($('#chkUnpublished<%=ModuleId %>').is(':checked')) {
             url += '&u=1'
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
<% If BlogContext.Security.CanAddPost %>
     var $blogChoose = $('<div class="dnnDialog"></div>')
           .html('<%=BlogSelectListHtml %>')
		.dialog({
		    autoOpen: false,
		    resizable: false,
		    dialogClass: 'dnnFormPopup dnnClear',
		    title: '<%=LocalizeJSString("BlogChoose") %>',
		 width: 500,
		 open: function (e) {
		     $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("cmdBlog") %>")').addClass('dnnPrimaryAction');
		     $('.ui-dialog-buttonpane').find('button:contains("<%=LocalizeJSString("Cancel") %>")').addClass('dnnSecondaryAction');
		     $('#ddBlog').width("100%");
		 },
		    buttons: [
       {
           text: '<%=LocalizeJSString("Cancel") %>',
        click: function () {
            $(this).dialog("close");
        }
    },
    {
        text: '<%=LocalizeJSString("cmdBlog") %>',
        click: function () {
            $(this).dialog("close");
            var url = '<%=EditUrl("PostEdit") %>';
      if (url.indexOf("?") == -1) {
          url += '?'
      } else {
          url += '&'
      };
      url += 'Blog=' + $('#ddBlog').val();
      window.location.href = encodeURI(url);
     }
    }
		 ]
		});
  <% If NrBlogs > 1 %>
     $('#<%=cmdBlog.ClientId %>').click(function () {
         $blogChoose.dialog('open');
         return false;
     });
  <% ElseIf NrBlogs = 1 %>
     $('#<%=cmdBlog.ClientId %>').click(function () {
         var url = '<%=EditUrl("PostEdit") %>';
      if (url.indexOf("?") == -1) {
          url += '?'
      } else {
          url += '&'
      };
      url += 'Blog=' + $('#ddBlog').val();
      window.location.href = encodeURI(url);
      return false;
  });
     <% End If %>
<% End If %>
 });
}(jQuery, window.Sys));
</script>
