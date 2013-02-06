<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewCategoriesSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewCategoriesSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<h2>View Categories Settings Control</h2>
<div class="dnnForm dnnTagSettings dnnClear">
	<div class="dnnFormItem">
		<dnn:label id="lblDisplayMode" runat="server" controlname="ddlDisplayMode" suffix=":" />
		<asp:DropDownList ID="ddlDisplayMode" runat="server">
			<asp:ListItem Value="Tree" resourcekey="Tree" />
			<asp:ListItem Value="List" resourcekey="List" Enabled="false" />
		</asp:DropDownList>
	</div>
	<div class="dnnFormItem" id="divTelerikSkin">
		<dnn:label id="lblTelerikSkin" runat="server" controlname="ddlTelerikSkin" suffix=":" />
		<asp:DropDownList ID="ddlTelerikSkin" runat="server" />
	</div>
</div>
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setupCategoryViewSettings() {
			switchDisplayMode();

			$('#<%= ddlDisplayMode.ClientID  %>').change(function () {
				switchDisplayMode();
				return true;
			});

			function switchDisplayMode() {
				var displayMode = $('#<%= ddlDisplayMode.ClientID  %>').val();

				if (displayMode == "List") {
					$("#divTelerikSkin").hide();
				}
				else {
					$("#divTelerikSkin").show('highlight', '', 200, '');
				}
			}

		};

		$(document).ready(function () {
			setupCategoryViewSettings();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setupCategoryViewSettings();
			});
		});

	} (jQuery, window.Sys));
</script>   