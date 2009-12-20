<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BlogImport.ascx.vb" Inherits="DotNetNuke.Modules.Blog.BlogImport" %>
<asp:datalist id="lstBlogs" runat="server" Width="100%" ExtractTemplateRows="True" ShowFooter="False">
	<HeaderTemplate>
		<asp:table ID="tblHeader" CellPadding="0" CellSpacing="0" BorderWidth="0" Runat="server" Visible="True">
			<asp:TableRow ID="trHeader" Runat="server">
				<asp:TableCell ID="tdCategory" Runat="server" Width="150">
					<asp:Label ID="lblHeaderCategory" Runat="server" CssClass="SubHead" ResourceKey="lblHeaderCategory"
						Visible="True"></asp:Label>
				</asp:TableCell>
				<asp:TableCell>
					<asp:Label ID="lblMappeTo" Runat="server" CssClass="SubHead" ResourceKey="lblMappeTo" Visible="True"></asp:Label>
				</asp:TableCell>
				<asp:TableCell HorizontalAlign="Right">
					<asp:Label ID="lblImport" Runat="server" CssClass="SubHead" ResourceKey="lblImport"></asp:Label>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell ColumnSpan="3">
					<hr>
				</asp:TableCell>
			</asp:TableRow>
		</asp:table>
	</HeaderTemplate>
	<ItemTemplate>
		<asp:Table id="tblBlogList" CellPadding="0" CellSpacing="0" BorderWidth="0" Runat="server">
			<asp:TableRow Runat="server" ID="trBlogList">
				<asp:TableCell Runat="server" ID="tdIcon" Width="250">
					<asp:Label id="lblCategoryID" Runat="server" Visible="False" CssClass="Normal"></asp:Label>
					<asp:Label id="lblCategory" Runat="server" Visible="True" CssClass="Normal">Category:</asp:Label>
				</asp:TableCell>
				<asp:TableCell>
					<asp:DropDownList ID="ddlBlogs" Runat="server"></asp:DropDownList>
				</asp:TableCell>
				<asp:TableCell HorizontalAlign="Right">
					<asp:CheckBox ID="chkImport" Runat="server" CssClass="Normal" ResourceKey="chkImport"></asp:CheckBox>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow Runat="server" ID="trForumList">
				<asp:TableCell ID="tdForumList" Runat="server" ColumnSpan="3">
					<asp:DataList ID="lstForum" Runat="server" Width="100%" ExtractTemplateRows="True">
						<HeaderTemplate>
							<asp:Table ID="Table1" CellPadding="0" CellSpacing="0" BorderWidth="0" Runat="server">
								<asp:TableRow ID="Tablerow1" Runat="server">
									<asp:TableCell ID="Tablecell1" Runat="server">
										<asp:Label ID="lblHeaderForum" Runat="server" Visible="True" CssClass="SubHead" ResourceKey="lblHeaderForum"></asp:Label>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</HeaderTemplate>
						<FooterTemplate>
							<asp:Table ID="tblFooter" Runat="server" Width="100%">
								<asp:TableRow>
									<asp:TableCell ColumnSpan="2">
										<hr>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</FooterTemplate>
						<ItemTemplate>
							<asp:Table ID="tblForumsList" CellPadding="0" CellSpacing="0" BorderWidth="0" Runat="server"
								Width="100%">
								<asp:TableRow ID="trForums" Runat="server">
									<asp:TableCell ID="tdForums" Runat="server">
										<asp:Label ID="lblForum" Runat="server" Visible="True" CssClass="Normal"></asp:Label>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</ItemTemplate>
					</asp:DataList>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</ItemTemplate>
</asp:datalist>
<asp:linkbutton id="cmdImport" ResourceKey="cmdImport" runat="server" 
    cssclass="CommandButton" borderstyle="None" onclick="cmdImport_Click" 
    style="height: 19px">Import</asp:linkbutton>&nbsp;
<asp:linkbutton id="cmdCancel" ResourceKey="cmdCancel" runat="server" cssclass="CommandButton" borderstyle="None"
	causesvalidation="False">Cancel</asp:linkbutton>&nbsp;
