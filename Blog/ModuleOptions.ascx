<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ModuleOptions.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ModuleOptions" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnBlogOptions dnnClear" id="dnnBlogOptions">
	<ul class="dnnAdminTabNav dnnClear">
		<li><a href="#boBasicSettings"><%=LocalizeString("BasicSettings")%></a></li>
		<li><a href="#boAdvancedSettings"><%=LocalizeString("tblAdvancedSettings")%></a></li>
	</ul>
	<div class="boBasicSettings dnnClear" id="boBasicSettings">
		<div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
		<div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>    
		<div class="bobsContent dnnClear">
			<h2 id="dnnSitePanel-AdvancedSettings" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secMisc")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblPageBlogs" runat="server" controlname="cmbPageBlogs" suffix=":" />
					<asp:DropDownList ID="cmbPageBlogs" DataValueField="BlogID" DataTextField="Title" runat="server" />
				</div>			
				<div class="dnnFormItem">
					<dnn:label id="lblRecentEntriesMax" runat="server" controlname="txtRecentEntriesMax" suffix=":" />
					<dnnweb:DnnNumericTextBox ID="dntxtbxRecentEntriesMax" runat="server" MinValue="1" MaxValue="100" Width="100"  NumberFormat-DecimalDigits="0" ShowSpinButtons="true" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblCatVocabRoot" runat="server" controlname="ddlCatVocabRoot" suffix=":" />
					<asp:DropDownList ID="ddlCatVocabRoot" runat="server" DataValueField="VocabularyID" DataTextField="Name" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblAllowMultipleCategories" runat="server" controlname="chkAllowMultipleCategories" suffix=":" />
					<asp:CheckBox ID="chkAllowMultipleCategories" runat="server" />
				</div>
				<asp:Panel class="dnnFormItem" id="pnlAllowChildBlogs" runat="server">
					<dnn:label id="lblAllowChildBlogs" runat="server" controlname="chkAllowChildBlogs" suffix=":" />
					<asp:CheckBox ID="chkAllowChildBlogs" runat="server" />
				</asp:Panel>
				<asp:Panel class="dnnFormItem" id="pnlMigrateChildBlogs" runat="server">
					<dnn:label id="lblMigrateChildblogs" runat="server" suffix=":" controlname="cmdMigrateChildblogs" />
					<asp:Label runat="server" ID="lblChildBlogsStatus" />
					<div class="dnnRight">
						<asp:LinkButton ID="cmdMigrateChildblogs" runat="server" CausesValidation="False" resourceKey="cmdMigrateChildblogs" CssClass="dnnSecondaryAction" />
					</div>
				</asp:Panel>
			</fieldset>
			<h2 id="dnnSitePanel-CommentSettings" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secCommentSettings")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblCommentMode" runat="server" controlname="lblShowGravatars" suffix=":" />
					<asp:DropDownList ID="ddlCommentMode" runat="server">
						<asp:ListItem Value="-1" resourcekey="None" />
						<asp:ListItem Value="0" resourcekey="Default" />
					</asp:DropDownList>
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblSocialSharingMode" runat="server" suffix=":" controlname="ddlSocialSharingMode" />
					<asp:DropDownList ID="ddlSocialSharingMode" runat="server">
						<asp:ListItem Value="-1" resourcekey="None" />
						<asp:ListItem Value="0" resourcekey="Default" />
					</asp:DropDownList>
				</div>
				<div class="dnnFormItem" id="divAddThisId" style="display:none;">
					<dnn:label id="lblAddThisId" runat="server" suffix=":" controlname="txtAddThisId" />
					<asp:TextBox ID="txtAddThisId" runat="server" CssClass="dnnSmall"  />
				</div>
				<div class="dnnFormItem" id="divFacebookApp" style="display:none;">
					<dnn:label id="lblFacebookAppId" runat="server" suffix=":" controlname="FacebookAppId" />
					<asp:TextBox ID="txtFacebookAppId" runat="server" CssClass="dnnSmall" />
				</div>
				<div class="dnnFormItem" id="divPlusOne" style="display:none;">
					<dnn:label id="lblEnablePlusOne" runat="server" suffix=":" controlname="chkEnablePlusOne" />
					<asp:CheckBox ID="chkEnablePlusOne" runat="server" />
				</div>
				<div class="dnnFormItem" id="divTwitter" style="display:none;">
					<dnn:label id="lblEnableTwitter" runat="server" suffix=":" controlname="chkEnableTwitter" />
					<asp:CheckBox ID="chkEnableTwitter" runat="server" />
				</div>
				<div class="dnnFormItem" id="divLinkedIn" style="display:none;">
					<dnn:label id="lblEnableLinkedIN" runat="server" suffix=":" controlname="chkEnableLinkedIN" />
					<asp:CheckBox ID="chkEnableLinkedIN" runat="server" />
				</div>
			</fieldset>	
			<h2 id="dnnSitePanel-RSSSettings" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secRSSSettings")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblIncludeBody" runat="server" controlname="chkIncludeBody" suffix=":" />
					<asp:CheckBox ID="chkIncludeBody" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblIncludeCategoriesInDescription" runat="server" controlname="chkIncludeCategoriesInDescription" suffix=":" />
					<asp:CheckBox ID="chkIncludeCategoriesInDescription" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblIncludeTagsInDescription" runat="server" controlname="chkIncludeTagsInDescription" suffix=":" />
					<asp:CheckBox ID="chkIncludeTagsInDescription" runat="server" />
				</div>
				<div class="dnnFormItem" style="display:none;">
					<dnn:label id="lblFeedCacheTime" runat="server" controlname="txtFeedCacheTime" suffix=":" />
					<asp:TextBox runat="server" ID="txtFeedCacheTime" Width="100" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblRecentRssEntriesMax" runat="server" controlname="txtRecentRssEntriesMax" suffix=":" />
					<asp:TextBox ID="txtRecentRssEntriesMax" runat="server" Text="10" />
				</div>     
			</fieldset>
			<h2 id="dnnSitePanel-WLWSettings" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secWLWSettings")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblAllowWLW" runat="server" controlname="chkAllowWLW" suffix=":" />
					<asp:CheckBox ID="chkAllowWLW" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblUseWLWExcerpt" runat="server" controlname="chkUseWLWExcerpt" suffix=":" />
					<asp:CheckBox ID="chkUseWLWExcerpt" runat="server" />
				</div>
			</fieldset>
		</div>
	</div>
	<div class="boAdvancedSettings" id="boAdvancedSettings">
		<div class="boasContent dnnClear">
			<h2 id="dnnSitePanel-GeneralSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("EntrySummary")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblMandatory" runat="server" controlname="chkForceDescription" suffix=":" />
					<asp:CheckBox ID="chkForceDescription" runat="server" />
				</div>
				<div class="dnnFormItem" id="divSummary">
					<dnn:label id="lblSummary" runat="server" controlname="txtSummaryLimit" suffix=":" />
					<asp:TextBox ID="txtSummaryLimit" runat="server" Text="1024" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblEnforceSummaryTruncation" runat="server" suffix=":" controlname="lblEnforceSummaryTruncation" />
					<asp:CheckBox ID="chkEnforceSummaryTruncation" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblShowSummary" runat="server" controlname="chkShowSummary" suffix=":" />
					<asp:CheckBox ID="chkShowSummary" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblAllowSummaryHtml" runat="server" controlname="chkAllowSummaryHtml" suffix=":" />
					<asp:CheckBox ID="chkAllowSummaryHtml" runat="server" />
				</div>		
			</fieldset>
			<h2 id="dnnSitePanel-SEOSettings" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("tblSEOSettings")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblShowSeoFriendlyUrl" runat="server" controlname="chkShowSeoFriendlyUrl" suffix=":" />
					<asp:CheckBox ID="chkShowSeoFriendlyUrl" runat="server" />
				</div>
				<div class="dnnFormItem">
					 <dnn:label id="lblRegenerateLinks" runat="server" suffix=":" controlname="cmdGenerateLinks" />
					 <div class="dnnLeft">
						<asp:LinkButton ID="cmdGenerateLinks" runat="server" CausesValidation="False" resourceKey="cmdGenerateLinks" CssClass="dnnSecondaryAction" />
					 </div>
				</div>
			</fieldset>
			<h2 id="dnnSitePanel-LoadFiles" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("FileManagement")%></a></h2>
			<fieldset>
				<div class="dnnFormItem">
					<dnn:label id="lblUploadOption" runat="server" suffix=":" controlname="chkUploadOption" />
					<asp:CheckBox ID="chkUploadOption" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblMaxImageWidth" runat="server" controlname="txtMaxImageWidth" suffix=":" />
					<asp:TextBox ID="txtMaxImageWidth" runat="server" Text="400" />
				</div>
				<div class="dnnFormItem">
					<dnn:label id="lblHostFiles" runat="server" controlname="cblHostFiles" suffix=":" />
					<asp:CheckBoxList runat="server" ID="cblHostFiles" RepeatColumns="1" />
				</div>
				<asp:Panel class="dnnFormItem" id="pnlPortalFiles" runat="server">
					<dnn:label id="lblPortalFiles" runat="server" controlname="cblPortalFiles" suffix=":" />
					<asp:CheckBoxList runat="server" ID="cblPortalFiles" RepeatColumns="1" CssClass="dnnBlogCheckbox" />
				</asp:Panel>
			</fieldset>
		</div>
	</div>
	<ul class="dnnActions">
		<li><asp:LinkButton ID="cmdUpdateOptions" runat="server" CausesValidation="False" resourceKey="cmdUpdate" CssClass="dnnPrimaryAction" /></li>
		<li><asp:HyperLink ID="hlCancelOptions" runat="server" resourceKey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
		<li><asp:LinkButton ID="cmdUpdate" runat="server" CausesValidation="False" CssClass="dnnSecondaryAction">Upgrade</asp:LinkButton></li>
	</ul>
</div>
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setupDnnBlogSettings() {
			handleSummaryDisplay();
			switchSocialMode();

			$('#dnnBlogOptions').dnnTabs().dnnPanels();

			$('#<%= chkForceDescription.ClientID  %>').click(function () {
				handleSummaryDisplay();
				return true;
			});

			$('#<%= ddlSocialSharingMode.ClientID  %>').change(function () {
				switchSocialMode();
				return true;
			});

			$('#<%= cmdMigrateChildblogs.ClientID  %>').dnnConfirm({
				text: '<%= Localization.GetString("MigrateConfirm.Text", Localization.SharedResourceFile) %>',
				yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
				noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
				title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
			});

			$('#boBasicSettings .dnnFormExpandContent a').dnnExpandAll({ expandText: '<%=Localization.GetSafeJSString("ExpandAll", Localization.SharedResourceFile)%>', collapseText: '<%=Localization.GetSafeJSString("CollapseAll", Localization.SharedResourceFile)%>', targetArea: '#boBasicSettings' });

			function handleSummaryDisplay() {
				if ($('#<%= chkForceDescription.ClientID  %>').prop('checked')) {
					$("#divSummary").hide('highlight', '', 200, '');
					$("#divSearchSummary").hide('highlight', '', 200, '');
				} else {
					$("#divSummary").show('highlight', '', 200, '');
					$("#divSearchSummary").show('highlight', '', 200, '');
				}
			}

			function switchSocialMode() {
				var mode = $('#<%= ddlSocialSharingMode.ClientID  %>').val();

				if (mode == "0") {
					$("#divAddThisId").hide();

					$("#divFacebookApp").show('highlight', '', 200, '');
					$("#divPlusOne").show('highlight', '', 200, '');
					$("#divTwitter").show('highlight', '', 200, '');
					$("#divLinkedIn").show('highlight', '', 200, '');
				}
				else if (mode == "1") {
					$("#divAddThisId").show('highlight', '', 200, '');

					$("#divFacebookApp").hide();
					$("#divPlusOne").hide();
					$("#divTwitter").hide();
					$("#divLinkedIn").hide();
				}
				else {
					$("#divAddThisId").hide();
					$("#divFacebookApp").hide();
					$("#divPlusOne").hide();
					$("#divTwitter").hide();
					$("#divLinkedIn").hide();
				}
			}

		};

		$(document).ready(function () {
			setupDnnBlogSettings();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setupDnnBlogSettings();
			});
		});

	} (jQuery, window.Sys));
</script>   