<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EntryEdit.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EntryEdit" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="blog" Namespace="DotNetNuke.Modules.Blog.Controls" Assembly="DotNetNuke.Modules.Blog" %>
<div class="dnnForm dnnBlogEditEntry dnnClear" id="dnnBlogEditEntry">
	<h2 id="dnnSitePanel-BlogContent" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("Title")%></a></h2>
	<fieldset>
		<dnn:Label ID="lblBlog" runat="server" controlname="ddBlog" suffix=":" CssClass="dnnLeft" /><br />
  <asp:DropDownList runat="server" ID="ddBlog" DataTextField="Title" DataValueField="BlogID" Width="100%" AutoPostBack="true" />

		<dnn:Label ID="lblTitle" runat="server" controlname="treeCategories" suffix=":" CssClass="dnnLeft" /><br />
		<asp:TextBox ID="txtTitle" runat="server" CssClass="dnnFormRequired" Width="98%" />
		<asp:RequiredFieldValidator ID="valTitle" runat="server" ResourceKey="valTitle.ErrorMessage" Display="Dynamic" ControlToValidate="txtTitle" CssClass="dnnFormError" />

		<dnn:Label ID="lblSummary" runat="server" controlname="txtDescription" suffix=":" CssClass="dnnLeft" /><br />
		<dnn:texteditor id="txtDescription" runat="server" width="100%" height="250" />
		<asp:TextBox runat="server" ID="txtDescriptionText" Width="100%" Height="250" TextMode="MultiLine" Visible="false" />
	</fieldset>
	<h2 id="H1" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("Content")%></a></h2>
	<fieldset>
  <asp:Label runat="server" ID="lblSummaryPrecedingWarning" resourcekey="lblSummaryPrecedingWarning" Visible="false" CssClass="dnnFormMessage dnnFormWarning" />
		<dnn:texteditor id="teBlogEntry" runat="server" width="100%" height="400" />
	</fieldset>
	<h2 id="H3" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("Publishing")%></a></h2>
	<fieldset>
		<asp:Panel ID="pnlPublished" runat="server" class="dnnFormItem">
			<dnn:Label ID="lblPublished" runat="server" controlname="chkPublished" suffix=":" />
			<asp:CheckBox ID="chkPublished" runat="server" AutoPostBack="True" />
		</asp:Panel>
		<asp:Panel ID="pnlComments" runat="server" class="dnnFormItem">
			<dnn:Label ID="lblAllowComments" runat="server" controlname="chkAllowComments" suffix=":" />
			<asp:CheckBox ID="chkAllowComments" runat="server" />
		</asp:Panel>
		<div class="dnnFormItem">
			<dnn:Label ID="lblDisplayCopyright" runat="server" controlname="chkDisplayCopyright" suffix=":" />
			<asp:CheckBox ID="chkDisplayCopyright" runat="server" AutoPostBack="True" />
		</div>
		<asp:Panel ID="pnlCopyright" runat="server" Visible="False" class="dnnFormItem">
			<dnn:Label ID="lblCopyright" runat="server" controlname="txtCopyright" suffix=":" />
			<asp:TextBox ID="txtCopyright" runat="server" />
		</asp:Panel>
		<div class="dnnFormItem">
			<dnn:Label ID="lblPublishDate" runat="server" ControlName="dpEntryDate" Suffix=":" />
			<div class="dnnLeft">
				<dnnweb:DnnDatePicker ID="dpEntryDate" runat="server" CssClass="dateFix" />
				<dnnweb:DnnTimePicker ID="tpEntryTime" runat="server" TimeView-Columns="4" ShowPopupOnFocus="true" CssClass="dateFix" />
			</div>
		</div>
		<div class="dnnClear" style="padding:10px 0px;">
   <em><%= LocalizeString("PublishTimeZoneNote")%>&nbsp;<asp:Literal runat="server" ID="litTimezone" /></em>
		</div>
	</fieldset>
	<h2 id="H2" class="dnnFormSectionHead"><a href="" class="dnnFormSectionExpanded"><%= LocalizeString("Metadata")%></a></h2>
	<fieldset>
  <div class="dnnFormItem">
   <dnn:Label ID="lblLocale" runat="server" controlname="ddLocale" suffix=":" />
   <asp:DropDownList ID="ddLocale" runat="server" DataTextField="NativeName" />
  </div>
  <div class="dnnFormItem">
   <dnn:Label ID="lblImage" runat="server" controlname="fileImage" suffix=":" />
   <div style="float:left;">
   <asp:Image runat="server" ID="imgEntryImage" /><br /><br />
   <asp:FileUpload runat="server" ID="fileImage" Width="300" />&nbsp;
   <asp:LinkButton runat="server" ID="cmdImageRemove" resourcekey="cmdImageRemove" cssclass="dnnSecondaryAction" />
   </div>
  </div>
		<asp:Panel ID="pnlCategories" runat="server" class="dnnFormItem">
			<dnn:Label ID="lblCategories" ResourceKey="lblCategories" runat="server" controlname="dtCategories" suffix=":" />
			<div class="dnnLeft">
				<dnnweb:DnnTreeView id="dtCategories" runat="server" CheckBoxes="true" DataFieldID="TermID" DataFieldParentID="ParentTermID" DataTextField="Name" DataValueField="TermID" />
			</div>    
		</asp:Panel>
  <div class="dnnClear"></div>
		<div style="display:block">
			<dnn:Label ID="lblTags" runat="server" controlname="txtTags" suffix=":" /><div></div>
			<blog:TagEdit ID="ctlTags" runat="server" width="500px" AllowSpaces="True" />	
		</div>
	</fieldset>

	<ul class="dnnActions">
		<li><asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdSave" /></li>
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
 (function ($, Sys) {

  $(document).ready(function () {

   $('#dnnBlogEditEntry').dnnPanels();

   $('.dnnEntryDelete').dnnConfirm({
    text: '<%= LocalizeString("DeleteItem") %>',
    yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
    noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
    title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
   });

  });

 } (jQuery, window.Sys));
</script>  
