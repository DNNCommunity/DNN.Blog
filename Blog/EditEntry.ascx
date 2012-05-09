<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditEntry.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditEntry" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnBlogEditEntry dnnClear" id="dnnBlogEditEntry">
	<h2 id="dnnSitePanel-BlogContent" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("BlogContent")%></a></h2>
	<fieldset>
		<div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>
		<div class="dnnFormItem">
			<dnn:Label ID="lblTitle" runat="server" controlname="treeCategories" suffix=":" />
			<asp:TextBox ID="txtTitle" runat="server" CssClass="dnnFormRequired" />
			<asp:RequiredFieldValidator ID="valTitle" runat="server" ResourceKey="valTitle.ErrorMessage" Display="Dynamic" ControlToValidate="txtTitle" CssClass="dnnFormError" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="lblPublishDate" runat="server" ControlName="dpEntryDate" Suffix=":" />
			<dnnweb:DnnDatePicker ID="dpEntryDate" runat="server" />
			<dnnweb:DnnTimePicker ID="tpEntryTime" runat="server" TimeView-Columns="4" ShowPopupOnFocus="true" />
		</div>
		<div class="dnnFormItem">
			<label></label>
			<div class="dnnLeft">
				<em><%= Localization.GetString("PublishTimeZoneNote", LocalResourceFile)%><br /><asp:Literal runat="server" ID="litTimezone" /></em>
			</div>
		</div>
		<asp:Panel class="dnnFormItem" id="pnlChildBlogs" runat="server">
			<dnn:Label ID="lblChildBlog" runat="server" controlname="cboChildBlogs" suffix=":" />
			<asp:DropDownList ID="cboChildBlogs" runat="server" ResourceKey="cboChildBlogs.DataTextField" DataValueField="BlogID" DataTextField="Title" /> 
		</asp:Panel>
		<div class="dnnFormItem">
			<dnn:Label ID="lblSummary" runat="server" controlname="txtDescription" suffix=":" />
			<div class="dnnLeft">
				<dnn:texteditor id="txtDescription" runat="server" width="450" height="300" />
				<asp:TextBox runat="server" ID="txtDescriptionText" Width="450" Height="200" TextMode="MultiLine" Visible="false" />
				<asp:RequiredFieldValidator ID="valDescription" runat="server" ResourceKey="valDescription.ErrorMessage" Display="Dynamic" ControlToValidate="txtDescription" CssClass="dnnFormError" />  
			</div>
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="lblDescription" ResourceKey="lblDescription" runat="server" controlname="teBlogEntry" suffix=":" />
			<div class="dnnLeft">
				<dnn:texteditor id="teBlogEntry" runat="server" width="450" height="400" />
			</div>
		</div>
		<asp:Panel ID="pnlCategories" runat="server" class="dnnFormItem">
			<dnn:Label ID="lblCategories" ResourceKey="lblCategories" runat="server" controlname="dtCategories" suffix=":" />
			<div class="dnnLeft">
				<dnnweb:DnnTreeView id="dtCategories" runat="server" CheckBoxes="true" DataFieldID="TermID" DataFieldParentID="ParentTermID" DataTextField="Name" DataValueField="TermID" />
			</div>    
		</asp:Panel>
		<div class="dnnFormItem">
			<dnn:Label ID="lblTags" runat="server" controlname="txtTags" suffix=":" />
			<div class="dnnClear">
				<asp:TextBox ID="txtTags" runat="server" />	
			</div>
		</div>
	</fieldset>			
	<asp:Panel ID="pnlUploads" runat="server" Visible="true">
		<h2 id="dnnSitePanel-UploadOptions" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secUploadOption")%></a></h2>
		<fieldset>
			<div class="dnnFormItem">
				<dnn:Label ID="lblAddPicture" runat="server" controlname="btnUploadPicture" />
				<input id="picFilename" type="file" size="40" name="picFilename" runat="server" />
				<asp:Button ID="btnUploadPicture" runat="server" ResourceKey="btnUploadPicture" />
			</div> 
			<div class="dnnFormItem">
				<dnn:Label ID="lblAltText" runat="server" controlname="txtAltText" />
				<asp:TextBox ID="txtAltText" runat="server" />
			</div> 
			<div class="dnnFormItem">
				<dnn:Label ID="lblAddAttachment" runat="server" controlname="attFilename" />
				<input id="attFilename" type="file" size="40" name="attFilename" runat="server" />
				<asp:Button ID="btnUploadAttachment" runat="server" ResourceKey="btnUploadAttachment" />
			</div> 
			<div class="dnnFormItem">
				<dnn:Label ID="lblAttachmentDescription" runat="server" controlname="txtAttachmentDescription" />
				<asp:TextBox ID="txtAttachmentDescription" runat="server" />
			</div> 
		</fieldset>
		<h2 id="dnnSitePanel-LinkedFiles" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secLinkedFiles")%></a></h2>
		<fieldset>
			<asp:DataGrid ID="dgLinkedFiles" runat="server" Width="100%" AutoGenerateColumns="False" BorderStyle="Solid" ShowFooter="False" GridLines="Horizontal">
				<Columns>
					<asp:TemplateColumn HeaderText="Filename" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="SubHead">
						<ItemTemplate>
							<asp:Label ID="lblFileName" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-HorizontalAlign="Right">
						<ItemTemplate>
							<asp:ImageButton ID="lnkDeleteFile" runat="server" ResourceKey="lnkDeleteFile.AlternateText" OnCommand="lnkDeleteFile_Command" CommandName='<%# DataBinder.Eval(Container, "DataItem") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</fieldset>
	</asp:Panel>
	<h2 id="dnnSitePanel-EntryOptions" class="dnnFormSectionHead"><a href="" class=""><%= LocalizeString("secEntryOptions")%></a></h2>
	<fieldset>
		<div class="dnnFormItem">
			<dnn:Label ID="lblAllowComments" runat="server" controlname="chkAllowComments" suffix=":" />
			<asp:CheckBox ID="chkAllowComments" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="lblDisplayCopyright" runat="server" controlname="chkDisplayCopyright" suffix=":" />
			<asp:CheckBox ID="chkDisplayCopyright" runat="server" AutoPostBack="True" />
		</div>
		<asp:Panel ID="pnlCopyright" runat="server" Visible="False" class="dnnFormItem">
			<dnn:Label ID="lblCopyright" runat="server" controlname="txtCopyright" suffix=":" />
			<asp:TextBox ID="txtCopyright" runat="server" />
		</asp:Panel>
		<div class="dnnFormItem">
			<dnn:Label ID="lblTrackbackUrl" runat="server" controlname="txtTrackBackUrl" suffix=":" />
			<asp:TextBox ID="txtTrackBackUrl" runat="server" />
		</div>
	</fieldset>
	<ul class="dnnActions">
		<li><asp:LinkButton ID="cmdPublish" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdPublish" /></li>
		<li><asp:LinkButton ID="cmdDraft" runat="server" CssClass="dnnSecondaryAction dnnSaveOffline" resourcekey="cmdDraft" /></li>
		<li><asp:HyperLink ID="hlCancel" ResourceKey="cmdCancel" runat="server" CssClass="dnnSecondaryAction" /></li>
		<li><asp:LinkButton ID="cmdDelete" ResourceKey="cmdDelete" runat="server" CssClass="dnnSecondaryAction dnnEntryDelete" CausesValidation="False" Visible="False" /></li>
	</ul>
	<div class="dnnFormItem">
		<asp:ValidationSummary ID="valSummary" CssClass="dnnFormMessage dnnFormValidationSummary" EnableClientScript="False" runat="server" DisplayMode="BulletList" />
	</div>
</div>
<asp:CustomValidator ID="valEntry" EnableClientScript="False" runat="server" ResourceKey="valEntry.ErrorMessage" Display="None" />
<asp:CustomValidator ID="valUpload" EnableClientScript="False" runat="server" Display="None" />
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setupDnnEditBlogEntry() {
			$('#dnnBlogEditEntry').dnnPanels();


			$("#catAvailable").draggable();
			$("#catSelected").droppable({
				drop: function (event, ui) {
					$(this)
					.addClass("ui-state-highlight");
				}
			});


			$('.dnnEntryDelete').dnnConfirm({
				text: '<%= LocalizeString("DeleteItem") %>',
				yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
				noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
				title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
			});
			$('.dnnSaveOffline').dnnConfirm({
				text: '<%= LocalizeString("SaveAndOffline.Confirm") %>',
				yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
				noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
				title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
			});

			function split(val) {
				return val.split(/,\s*/);
			}

			function extractLast(term) {
				return split(term).pop();
			}

			var myTextArea = $('#<%= txtTags.ClientID  %>').tagify({ delimiters: [9, 13, 44, 59, 188], addTagPrompt: '<%= Localization.GetString("AddTags", LocalResourceFile) %>' }); // tab, return, comma, semicolon
			myTextArea.tagify('inputField').autocomplete({
				source: function (request, response) {
					$.ajax({
						type: "POST",
						url: '<%= ResolveUrl("~/DesktopModules/Blog/BlogTerms.asmx/SearchTags")%>',
						data: "{'searchTerm' : '" + extractLast(request.term) + "'}",
						contentType: "application/json",
						dataType: "json",
						success: function (data) {
							var suggestions = [];
							mydata = data;
							$.each($.parseJSON(data.d), function (i, val) {
								suggestions.push(val);
							});
							response(suggestions);
						}
					});
				},
				minLength: 2,
				close: function (event, ui) { myTextArea.tagify('add'); myTextArea.tagify('serialize'); }
			});
		};

		$(document).ready(function () {
			setupDnnEditBlogEntry();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setupDnnEditBlogEntry();
			});
		});

	} (jQuery, window.Sys));
</script>  
<script language="javascript" type="text/javascript">
	function serializeTags(source, args) {
		$('#<%= txtTags.ClientID %>').tagify('serialize');
		args.IsValid = true;
	};
</script>