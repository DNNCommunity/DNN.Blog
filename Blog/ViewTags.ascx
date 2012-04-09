<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewTags.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewTags" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<asp:Panel ID="pnlTagList" runat="server" class="dnnForm blogViewTags dnnClear">
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
<asp:Panel ID="pnlTagCloud" runat="server" class="dnnBlogTagCloud dnnClear">
	<dnnweb:DnnTagCloud ID="rtcTags" runat="server" Width="176" RenderItemWeight="true" DataValueField="TermID" DataTextField="Name" DataWeightField="TotalTermUsage" OnItemDataBound="RtcCloudItemDataBound" />
</asp:Panel>
<asp:PlaceHolder ID="phTags" runat="server" />
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setupDnnBlogViewTags() {
			$('.qaTooltip').qaTooltip();
		}

		$(document).ready(function () {
			setupDnnBlogViewTags();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setupDnnBlogViewTags();
			});
		});

	} (jQuery, window.Sys));
</script>  