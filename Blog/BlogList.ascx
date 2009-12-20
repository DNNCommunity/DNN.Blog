<%@ Control Language="vb" AutoEventWireup="false" Codebehind="BlogList.ascx.vb" Inherits="DotNetNuke.Modules.Blog.BlogList" %>
<asp:datalist id="lstBlogs" ShowFooter="False" ExtractTemplateRows="True" Width="100%" runat="server">
	<HeaderTemplate>
		<asp:Table id="tblHeader" CellPadding="0" CellSpacing="0" BorderWidth="0" Runat="server">
			<asp:TableRow Runat="server" ID="trHeader">
				<asp:TableCell Runat="server" ID="tdHeaderIcon" Width="10">
					<asp:HyperLink Runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_item.gif" ID="lnkBlogIcon"
						Font-Bold="True"></asp:HyperLink>
				</asp:TableCell>
				<asp:TableCell Runat="server" ID="tdHeaderLink" ColumnSpan="2">
					<asp:HyperLink cssclass="CommandButton" id="lnkBlog" runat="server" ResourceKey="lnkBlog">
					 View All Recent Entries
					</asp:HyperLink>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</HeaderTemplate>
	<FooterTemplate>
		<asp:Table id="Table1" CellPadding="0" CellSpacing="0" BorderWidth="0" Runat="server">
			<asp:TableRow Runat="server" ID="Tablerow1">
				<asp:TableCell Runat="server" ID="tdFooter" ColumnSpan="2" cssclass="Normal">
				<asp:Label ID="lblFooter" Runat="server" CssClass="Normal"></asp:Label>					
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</FooterTemplate>
	<ItemTemplate>
		<asp:Table id="tblBlogList" Runat="server" BorderWidth="0" CellSpacing="0" CellPadding="0">
			<asp:TableRow Runat="server" ID="trBlogList">
				<asp:TableCell Runat="server" ID="tdIcon" Width="10">
					<asp:HyperLink Runat="server" ImageUrl="~/desktopmodules/Blog/Images/tree_item.gif" ID="lnkBlogIcon"></asp:HyperLink>
				</asp:TableCell>
				<asp:TableCell Runat="server" ID="tdBlogName">
					<asp:HyperLink cssclass="CommandButton" id="lnkBlog" runat="server" Font-Bold="True" />
				</asp:TableCell>
				<asp:TableCell Runat="server" ID="tdBlogRSS" HorizontalAlign="Right">
					<asp:HyperLink ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" Runat="server" ID="lnkBlogRSS"
						Visible="False" target="_blank"></asp:HyperLink>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow Runat="server" ID="trBlogChildren" Visible="False">
				<asp:TableCell Runat="server" ID="Tablecell2" Width="10">
					<asp:Image ImageUrl="~/images/spacer.gif" Runat="server" Width="8" Height="8" ID="Image1"></asp:Image>
				</asp:TableCell>
				<asp:TableCell Runat="server" ID="tdBlogChildren" ColumnSpan="2">
					<asp:datalist id="lstBlogChildren" runat="server" Width="100%" ExtractTemplateRows="True">
						<ItemTemplate>
							<asp:Table ID="tblChildBlogs" Runat="server" BorderWidth="0" CellSpacing="0" CellPadding="0">
								<asp:TableRow Runat="server" ID="trChildBlogs">
									<asp:TableCell Runat="server" ID="tdChildIcon" Width="10">
										<asp:HyperLink Runat="server" ImageUrl="~/desktopmodules/Blog/Images/tree_item.gif" ID="lnkChildIcon"></asp:HyperLink>
									</asp:TableCell>
									<asp:TableCell Runat="server" ID="tdChildBlog">
										<asp:HyperLink cssclass="CommandButton" id="lnkChildBlog" runat="server" />
									</asp:TableCell>
									<asp:TableCell Runat="server" ID="tdChildBlogRSS" HorizontalAlign="Right">
										<asp:HyperLink ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" Runat="server" ID="lnkChildBlogRSS"
											Visible="False" Target="_blank"></asp:HyperLink>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</ItemTemplate>
					</asp:datalist>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</ItemTemplate>
</asp:datalist>
