<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewCategories.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewCategories" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<h2>View Categories Control</h2>
<div class="dnnForm dnnBlogCategories">
	<asp:Panel ID="pnlTreeview" runat="server">
		<dnnweb:DnnTreeView id="dtCategories" runat="server" DataFieldID="TermID" DataFieldParentID="ParentTermID" DataTextField="Name" DataValueField="TermID" OnNodeDataBound="TvNodeItemDataBound" />
	</asp:Panel>
	<asp:Panel ID="pnlList" runat="server" Visible="false">
		<div class="dnnLeft">
		<ul class="qaRecentTags dnnClear">
			<asp:Repeater ID="rptTags" runat="server" OnItemDataBound="RptTagsItemDataBound" EnableViewState="false">
				<ItemTemplate>
					<li class="rtTag"><dba:Tags ID="dbaSingleTag" runat="server" EnableViewState="false" /></li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>	
	</div>
	</asp:Panel>
</div>