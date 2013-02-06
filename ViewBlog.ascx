<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewBlog" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<h2>View Blog Control</h2>
<div class="dnnClear dnnForm dnnViewBlog">
	<asp:Panel ID="pnlBlogInfo" Visible="false" runat="server">
		<div class="vbAuthor dnnClear">
			<div class="vbaImage dnnLeft">
				<div><asp:HyperLink id="imgAuthorLink" runat="server"><dnnweb:DnnBinaryImage ID="dbiUser" runat="server" Width="50" /></asp:HyperLink></div>
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
	<asp:DataList ID="lstBlogView" runat="server" Width="100%">
		<ItemTemplate>
			<div class="vbEntry">
				<h2><asp:HyperLink ID="lnkEntry" runat="server"><%# DataBinder.Eval(Container.DataItem, "Title") %></asp:HyperLink></h2>
				<div class="vbHeader dnnClear">
					<asp:Literal ID="litAuthor" runat="server" />
					<asp:Label ID="lblPublishDate" runat="server" />
					<div class="dnnRight"></div>
				</div>
				<div class="vbBody dnnClear">
					<asp:Panel id="pnNotPublished" runat="server" Visible="false" CssClass="dnnFormMessage dnnFormWarning">
						<%= LocalizeString("lblPublished")%>
					</asp:Panel>
					<asp:Literal ID="litDescription" runat="server" />
					<div class="BlogReadMore" runat="server" id="divBlogReadMore">
						<asp:HyperLink ID="hlPermaLink" runat="server" CssClass="dnnSecondaryAction" />
						<asp:HyperLink ID="hlMore" runat="server" CssClass="dnnSecondaryAction" />
					</div>
				</div>
				<div class="dnnClear">
					<div class="dnnLeft">
						<div class="BlogCategories">
							<asp:Literal ID="litCategories" runat="server" />
						</div>
						<div class="tags BlogTopics">
							<div class="tags"><dba:Tags ID="dbaTag" runat="server" EnableViewState="false" /></div>
						</div>
					</div>
					<div class="dnnRight">
						<asp:HyperLink ID="lnkComments" runat="server" CssClass="BlogComments" />
						<asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="BlogEditLink" runat="server" />
					</div>
				</div>
			</div>
		</ItemTemplate>
		<SeparatorTemplate>
			<div class="blogSeparator"></div>
		</SeparatorTemplate>
	</asp:DataList>
	<div class="dnnRight">
		<asp:HyperLink ID="hlPagerPrev" resourcekey="hlPagerPrev" runat="server" CssClass="dnnPrimaryAction" Visible="false" />
		<asp:HyperLink ID="hlPagerNext" resourcekey="hlPagerNext" runat="server" CssClass="dnnPrimaryAction" Visible="false" />
	</div>
	<asp:Panel ID="pnlNoRecords" runat="server" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
		<asp:Literal ID="litNoRecords" runat="server" />
	</asp:Panel>
</div>