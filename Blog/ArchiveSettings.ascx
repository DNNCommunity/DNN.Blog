<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ArchiveSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ArchiveSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnTagSettings dnnClear">
	<div class="dnnFormItem">
		<dnn:label id="lblDisplayMode" runat="server" controlname="chkEnableArchiveDropDown" suffix=":" />
		<asp:DropDownList ID="ddlDisplayMode" runat="server">
			<asp:ListItem Value="List" resourcekey="List" />
			<asp:ListItem Value="Calendar" resourcekey="Calendar" />
		</asp:DropDownList>
	</div>
	<div class="dnnFormItem" id="divListMode" style="display:none;">
		<dnn:label id="lblListMode" runat="server" controlname="chkEnableArchiveDropDown" suffix=":" />
		<asp:DropDownList ID="ddlListMode" runat="server">
			<asp:ListItem Value="DropDown" resourcekey="DropDown" />
			<asp:ListItem Value="UL" resourcekey="UL" />
		</asp:DropDownList>
	</div>
	<div class="dnnFormItem" id="divLoadCss" style="display:none;">
		<dnn:Label ID="lblLoadCss" runat="server" Suffix=":" ControlName="rblLoadCss" />
		<asp:RadioButtonList ID="rblLoadCss" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal">
			<asp:ListItem Value="True" resourcekey="Yes" />
			<asp:ListItem Value="False" resourcekey="No" />
		</asp:RadioButtonList>
	</div>
</div>
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setupArchiveSettings() {
			switchDisplayMode();

			$('#<%= ddlDisplayMode.ClientID  %>').change(function () {
			    switchDisplayMode();
			    return true;
			});

			function switchDisplayMode() {
			    var displayMode = $('#<%= ddlDisplayMode.ClientID  %>').val();

			    if (displayMode == "List") {
					$("#divListMode").show('highlight', '', 200, '');
					$("#divLoadCss").hide();
				}
	            else {
	                $("#divLoadCss").show('highlight', '', 200, '');
	                $("#divListMode").hide();
				}
			}

		};

		$(document).ready(function () {
			setupArchiveSettings();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setupArchiveSettings();
			});
		});

	} (jQuery, window.Sys));
</script>   