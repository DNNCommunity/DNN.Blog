<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewBlog" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<div class="dnnClear">
 <asp:Panel ID="pnlBlogInfo" Visible="False" runat="server">
  <table class="BlogInfo" cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
	<td class="BlogDescriptionHeavy" style="white-space: nowrap; text-align: right; width: 20px;">
	 <asp:Label ID="lblAuthorHeader" runat="server" ResourceKey="lblAuthorHeader" />
	</td>
	<td style="white-space: nowrap;">
	 <asp:Label ID="lblAuthor" runat="server" CssClass="BlogDescription" />
	</td>
	<td class="BlogDescriptionHeavy" style="white-space: nowrap; text-align: right; width: 20px;">
	 <asp:Label ID="lblCreatedHeader" runat="server" ResourceKey="lblCreatedHeader" />
	</td>
	<td style="white-space: nowrap;">
	 <asp:Label ID="lblCreated" runat="server" CssClass="BlogDescription" />
	</td>
	<td align="right">
	 <asp:HyperLink ID="lnkRSS" runat="server" Visible="False" Target="_blank">
	  <asp:Image ID="Image1" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" AlternateText="RssIcon" />
	 </asp:HyperLink>
	</td>
   </tr>
   <tr>
	<td colspan="5">
	 <div class="BlogDescription">
	  <asp:Literal ID="litBlogDescription" runat="server" />
	 </div>
	</td>
   </tr>
  </table>
 </asp:Panel>
 <asp:Panel ID="pnlBlogRss" runat="server" Visible="False">
  <table class="BlogInfo" cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
	<td align="right">
	 <asp:HyperLink ID="lnkRecentRss" runat="server" Target="_blank">
	  <asp:Image ID="lnkRecentRssIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" AlternateText="RssIcon" />
	 </asp:HyperLink>
	</td>
   </tr>
  </table>
 </asp:Panel>
 <asp:Label ID="InfoEntry" ResourceKey="lblInfoEntry" runat="server" />
 <asp:DataList ID="lstBlogView" runat="server" Width="100%">
  <ItemTemplate>
   <div class="BlogBody">
	<!-- Begin Blog Entry Title -->
	<div class="BlogHead">
	 <h2>
	  <asp:HyperLink ID="lnkEntry" runat="server">
			<%# DataBinder.Eval(Container.DataItem, "Title") %>
	  </asp:HyperLink>
	 </h2>
	</div>
	<div class="BlogDateline">
		<asp:Literal ID="litAuthor" runat="server" />
		<asp:Label ID="lblPublishDate" runat="server" />
	</div>
	<div style="padding-top: 1em">
	 <asp:Label ID="lblPublished" runat="server" Visible="False" CssClass="NormalRed" ResourceKey="lblPublished" />
	 <asp:Literal ID="litDescription" runat="server" />
	 <div class="BlogReadMore" runat="server" id="divBlogReadMore">
	  <asp:HyperLink ID="hlPermaLink" runat="server" />
	  <asp:HyperLink ID="hlMore" runat="server" CssClass="BlogMoreLink" />
	 </div>
	</div>
	<div class="BlogFooter">
	 <div class="BlogFooterRight">
	  <asp:HyperLink ID="lnkComments" runat="server" CssClass="BlogCommentsNormal" />
	  <asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="BlogEditLink" runat="server" />
	 </div>
	 <div class="BlogFooterLeft">
		<div class="BlogCategories">
		<asp:Label ID="lblCategories" runat="server" ResourceKey="lblCategories" />
		<asp:HyperLink ID="lnkParentBlog" runat="server" />
		<asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/DesktopModules/Blog/images/folder_closed.gif" AlternateText="Parent Separator" />
		<asp:HyperLink ID="lnkChildBlog" runat="server" />
	  </div>
		<div class="tags dnnClear BlogTopics">
		   <div class="dnnLeft">
				<div class="tags"><dba:Tags ID="dbaTag" runat="server" EnableViewState="false" /></div>
		   </div>
	   </div>
	 </div>
	</div>
   </div>
  </ItemTemplate>
 </asp:DataList>
 <asp:DataList ID="lstSearchResults" Visible="False" runat="server" Width="100%">
  <ItemTemplate>
   <table border="0" cellpadding="1" cellspacing="0" width="100%">
	<tr>
	 <td class="SubHead" style="white-space: nowrap;">
	  <asp:HyperLink CssClass="SubHead" ID="lnkEntryTitle" runat="server">
			<%# Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "EntryTitle")) %>
	  </asp:HyperLink>
	 </td>
	 <td align="right">
	  <asp:Label runat="server" ID="lblEntryUserName" CssClass="Normal" />
	  <asp:Label runat="server" ID="lblEntryDate" CssClass="Normal" />
	 </td>
	</tr>
	<tr>
	 <td class="Normal">
	  <asp:HyperLink ID="lnkParentBlogSearch" runat="server" CssClass="CommandButton" />
	  <asp:Image ID="imgBlogParentSeparatorSearch" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" AlternateText="Parent Separator" />
	  <asp:HyperLink ID="lnkChildBlogSearch" runat="server" CssClass="CommandButton" Visible="False" />
	 </td>
	 <td class="SubHead" align="right">
	  <asp:Label runat="server" ID="lblHits" resourcekey="lblHits" />
	  <%# DataBinder.Eval(Container.DataItem, "Rank") %>
	 </td>
	</tr>
	<tr>
	 <td colspan="2" class="Normal">
	  <asp:Label ID="lblItemSummary" runat="server" />
	 </td>
	</tr>
	<tr>
	 <td class="Normal" align="right" colspan="2">
	  <asp:HyperLink runat="server" ID="hlPermaLinkSearch" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "PermaLink") %>' ResourceKey="lnkPermaLink" CssClass="CommandButton" Visible='<%# CBool(CStr(DataBinder.Eval(Container.DataItem, "PermaLink")) <> DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabID, PortalId, DataBinder.Eval(Container.DataItem, "EntryID"), DataBinder.Eval(Container.DataItem, "EntryTitle"), BlogSettings.ShowSeoFriendlyUrl)) %>' />
	  <asp:HyperLink runat="server" ID="hlMoreSearch" NavigateUrl='<%# DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabID, PortalId, DataBinder.Eval(Container.DataItem, "EntryID"), DataBinder.Eval(Container.DataItem, "EntryTitle"), BlogSettings.ShowSeoFriendlyUrl) %>' ResourceKey="lnkReadMore" CssClass="CommandButton" />
	 </td>
	</tr>
   </table>
   <hr />
  </ItemTemplate>
 </asp:DataList>
</div>