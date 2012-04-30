<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewBlog" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<div class="dnnClear dnnForm dnnViewBlog">
	<asp:Panel ID="pnlBlogInfo" Visible="False" runat="server">
		<div class="vbAuthor dnnClear">
			<div class="dnnLeft">
				<asp:HyperLink id="imgAuthorLink" runat="server"><dnnweb:DnnBinaryImage ID="dbiUser" runat="server" Width="40" /></asp:HyperLink>
			</div>
			<div class="dnnLeft">
				<asp:HyperLink ID="hlAuthor" runat="server" />
				<div class="dnnLeft">
					<asp:Literal ID="litBlogDescription" runat="server" />
				</div>
			</div>
			<div class="dnnRight">
				<asp:HyperLink ID="lnkRSS" runat="server" Visible="False" Target="_blank">
					<asp:Image ID="Image1" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" AlternateText="RssIcon" />
				</asp:HyperLink>
			</div>
		</div>
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
			<div class="vbEntry">
				<h2><asp:HyperLink ID="lnkEntry" runat="server"><%# DataBinder.Eval(Container.DataItem, "Title") %></asp:HyperLink></h2>
				<div class="vbHeader">
					<asp:Literal ID="litAuthor" runat="server" />
					<asp:Label ID="lblPublishDate" runat="server" />
				</div>
				<div class="vbBody dnnClear">
					<asp:Label ID="lblPublished" runat="server" Visible="False" CssClass="NormalRed" ResourceKey="lblPublished" />
					<asp:Literal ID="litDescription" runat="server" />
					<div class="BlogReadMore" runat="server" id="divBlogReadMore">
						<asp:HyperLink ID="hlPermaLink" runat="server" CssClass="dnnSecondaryAction" />
						<asp:HyperLink ID="hlMore" runat="server" CssClass="dnnSecondaryAction" />
					</div>
				</div>
				<div class="dnnClear">
					<div class="dnnLeft">
						<div class="BlogCategories">
							<label><%= Localization.GetString("lblCategories", LocalResourceFile)%></label>
							<asp:HyperLink ID="lnkParentBlog" runat="server" />
							<asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/DesktopModules/Blog/images/folder_closed.gif" AlternateText="Parent Separator" />
							<asp:HyperLink ID="lnkChildBlog" runat="server" />
						</div>
						<div class="BlogCategories">
							<asp:Literal ID="litCategories" runat="server" />
						</div>
						<div class="tags BlogTopics">
							<div class="tags"><dba:Tags ID="dbaTag" runat="server" EnableViewState="false" /></div>
						</div>
					</div>
					<div class="dnnRight">
						<asp:HyperLink ID="lnkComments" runat="server" CssClass="dnnSecondaryAction" />
						<asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="dnnSecondaryAction" runat="server" />
					</div>
				</div>
			</div>
		</ItemTemplate>
		<SeparatorTemplate>
			<div class="blogSeparator"></div>
		</SeparatorTemplate>
	</asp:DataList>
	<asp:DataList ID="lstSearchResults" Visible="False" runat="server" Width="100%">
		<ItemTemplate>
			<table>
				<tr>
					<td>
						<asp:HyperLink ID="lnkEntryTitle" runat="server"><%# Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "EntryTitle")) %></asp:HyperLink>
					</td>
					<td>
						<asp:Label runat="server" ID="lblEntryUserName" />
						<asp:Label runat="server" ID="lblEntryDate" />
					</td>
				</tr>
				<tr>
					<td>
						<asp:HyperLink ID="lnkParentBlogSearch" runat="server" CssClass="dnnSecondaryAction" />
						<asp:Image ID="imgBlogParentSeparatorSearch" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" AlternateText="Parent Separator" />
						<asp:HyperLink ID="lnkChildBlogSearch" runat="server" CssClass="dnnSecondaryAction" Visible="False" />
					</td>
					<td>
						<asp:Label runat="server" ID="lblHits" resourcekey="lblHits" />
						<%# DataBinder.Eval(Container.DataItem, "Rank") %>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<asp:Label ID="lblItemSummary" runat="server" />
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<asp:HyperLink runat="server" ID="hlPermaLinkSearch" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "PermaLink") %>' ResourceKey="lnkPermaLink" CssClass="dnnSecondaryAction" Visible='<%# CBool(CStr(DataBinder.Eval(Container.DataItem, "PermaLink")) <> DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabID, PortalId, DataBinder.Eval(Container.DataItem, "EntryID"), DataBinder.Eval(Container.DataItem, "EntryTitle"), BlogSettings.ShowSeoFriendlyUrl)) %>' />
						<asp:HyperLink runat="server" ID="hlMoreSearch" NavigateUrl='<%# DotNetNuke.Modules.Blog.Business.Utility.BlogNavigateURL(TabID, PortalId, DataBinder.Eval(Container.DataItem, "EntryID"), DataBinder.Eval(Container.DataItem, "EntryTitle"), BlogSettings.ShowSeoFriendlyUrl) %>' ResourceKey="lnkReadMore" CssClass="dnnSecondaryAction" />
					</td>
				</tr>
			</table>
		</ItemTemplate>
	</asp:DataList>
</div>