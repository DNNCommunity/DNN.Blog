<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ViewBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewBlog" %>
<div>
	<asp:panel id="pnlBlogInfo" Visible="False" Runat="server">
		<TABLE class="BlogInfo" cellSpacing="1" cellPadding="1" width="100%" border="0">
			<TR>
				<TD class="blog_Description_Heavy" noWrap align="right" width="20">
					<asp:Label id="lblAuthorHeader" Runat="server" ResourceKey="lblAuthorHeader">Author:</asp:Label></TD>
				<TD noWrap>
					<asp:Label id="lblAuthor" Runat="server" cssclass="blog_Description"></asp:Label></TD>
				<TD class="blog_Description_Heavy" noWrap align="right" width="20">
					<asp:Label id="lblCreatedHeader" Runat="server" ResourceKey="lblCreatedHeader">Created:</asp:Label></TD>
				<TD noWrap>
					<asp:Label id="lblCreated" Runat="server" cssclass="blog_Description"></asp:Label></TD>
				<TD align="right">
					<asp:HyperLink id="lnkRSS" Runat="server" Visible="False" target="_blank" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif"></asp:HyperLink></TD>
			</TR>
			<TR>
				<TD colSpan="5">
					<asp:label id="lblBlogDescription" cssclass="blog_Description" runat="server"></asp:label></TD>
			</TR>
		</TABLE>
	</asp:panel>
	<asp:Panel id="pnlBlogRss" Runat="server" Visible="False">
		<TABLE class="BlogInfo" cellSpacing="1" cellPadding="1" width="100%" border="0">
			<TR>
				<TD align="right">
					<asp:HyperLink id="lnkRecentRss" Runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif"
						Target="_blank"></asp:HyperLink></TD>
			</TR>
		</TABLE>
	</asp:Panel>
	<asp:label id="InfoEntry" ResourceKey="lblInfoEntry" cssclass="NormalBold" runat="server" text="No Entries were posted for this blog."></asp:label>
	<asp:datalist id="lstBlogView" runat="server" width="100%">
		<ItemTemplate>
			<div class="blog_body">
				<!-- Begin Blog Entry Title -->
				<div class="blog_head">
					<h2 class="blog_title">
						<asp:HyperLink id="lnkEntry" runat="server">
							<%# DataBinder.Eval(Container.DataItem, "Title") %>
						</asp:HyperLink>
					</h2>
				</div>
				<asp:label id="lblUserName" runat="server" visible="false" CssClass="blog_dateline"></asp:label>
				<asp:Label ID="lblPublishDate" Runat="server" CssClass="blog_dateline"></asp:Label>
				<p>
					<asp:Label ID="lblPublished" Runat="server" Visible="False" CssClass="NormalRed" ResourceKey="lblPublished">
						<p>This entry has not been published.</p>
					</asp:Label>
					<asp:Label Runat="server" ID="lblDescription"></asp:Label>
					<asp:HyperLink id="lnkReadMore" runat="server" ResourceKey="lnkReadMore" CssClass="blog_more_link">Read More...</asp:HyperLink>
				</p>
				<div class="blog_footer">
					<div class="blog_footer_right">
						<asp:LinkButton ID="lnkComments" Runat="server" commandname="Comments" commandargument='<%# DataBinder.Eval(Container.DataItem, "EntryID") %>' CssClass="blog_comments_normal"><%= getLnkComment() %> (<%# DataBinder.Eval(Container.DataItem, "CommentCount") %>)</asp:LinkButton>
						<asp:hyperlink id="lnkEditEntry" ResourceKey="msgEditEntry" cssclass="blog_edit_link" runat="server">Edit Entry</asp:hyperlink>
					</div>
					<div class="blog_footer_left">
						<span class="blog_topics">
							<asp:HyperLink ID="lnkParentBlog" Runat="server"></asp:HyperLink>
							<asp:image id="imgBlogParentSeparator" Runat="server" ImageUrl="~/DesktopModules/Blog/images/folder_closed.gif"></asp:image>
							<asp:HyperLink ID="lnkChildBlog" Runat="server"></asp:HyperLink>
						</span>
					</div>
				</div>
			</div>
		</ItemTemplate>
	</asp:datalist>
	<asp:datalist id="lstSearchResults" Visible="False" Runat="server" width="100%">
		<itemtemplate>
			<table border="0" cellpadding="1" cellspacing="0" width="100%">
				<tr>
					<td class="SubHead" nowrap="nowrap">
						<asp:HyperLink cssclass="SubHead" id="lnkEntryTitle" runat="server">
							<%# Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "EntryTitle")) %>
						</asp:HyperLink>
					</td>
					<td align="right">
						<asp:Label Runat="server" ID="lblEntryUserName" CssClass="Normal"></asp:Label>
						<asp:Label Runat="server" ID="lblEntryDate" Cssclass="Normal"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="Normal">
						<asp:HyperLink ID="lnkParentBlogSearch" Runat="server" CssClass="CommandButton"></asp:HyperLink>
						<asp:image id="imgBlogParentSeparatorSearch" Runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif"
							Visible="False"></asp:image>
						<asp:HyperLink ID="lnkChildBlogSearch" Runat="server" CssClass="CommandButton" Visible="False"></asp:HyperLink>
					</td>
					<td class="SubHead" align="right">Hits:
						<%# DataBinder.Eval(Container.DataItem, "Rank") %>
					</td>
				</tr>
				<tr>
					<td colspan="2" class="Normal">
						<asp:Label ID="lblItemSummary" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td class="Normal" align="right" colspan="2">
						<asp:linkbutton id="lnkMoreSearch" runat=server commandname="Entry" commandargument='<%# DataBinder.Eval(Container.DataItem, "EntryID") %>' CssClass="CommandButton" ResourceKey="lnkMoreSearch"> More... </asp:linkbutton>
					</td>
				</tr>
			</table>
			<hr />
		</itemtemplate>
	</asp:datalist>
</div>
