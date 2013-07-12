<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PostEdit.ascx.vb" Inherits="DotNetNuke.Modules.Blog.PostEdit" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="blog" Namespace="DotNetNuke.Modules.Blog.Controls" Assembly="DotNetNuke.Modules.Blog" %>
<div class="dnnForm dnnBlogEditPost dnnClear" id="dnnBlogEditPost">
 <ul class="dnnAdminTabNav">
  <li><a href="#dnnBlogEditContent"><%= LocalizeString("Content")%></a></li>
  <li><a href="#dnnBlogEditMetadata"><%= LocalizeString("Metadata")%></a></li>
  <li><a href="#dnnBlogEditPublishing"><%= LocalizeString("Publishing")%></a></li>
 </ul>
 <div class="dnnClear" id="dnnBlogEditContent">
	<fieldset>
		<dnn:Label ID="lblTitle" runat="server" controlname="treeCategories" suffix=":" CssClass="dnnLeft" /><br />
  <blog:ShortTextEdit id="txtTitle" runat="server" Required="True" CssClass="blog_rte_full" RequiredResourceKey="Title.Required" CssPrefix="blog_rte_" />
  <asp:Label runat="server" ID="lblSummaryPrecedingWarning" resourcekey="lblSummaryPrecedingWarning" Visible="false" CssClass="dnnFormMessage dnnFormWarning" />
  <blog:LongTextEdit id="teBlogPost" runat="server" Width="100%" TextBoxHeight="500" TextBoxWidth="100%" ShowRichTextBox="True" CssClass="blog_rte" CssPrefix="blog_rte_" />
	</fieldset>
 </div>
 <div class="dnnClear" id="dnnBlogEditMetadata">
	<fieldset>
		<dnn:Label ID="lblSummary" runat="server" controlname="txtDescription" suffix=":" CssClass="dnnLeft" />
  <div style="display:block;float:clear">&nbsp;</div>
  <blog:LongTextEdit id="txtDescription" runat="server" Width="100%" TextBoxWidth="100%" TextBoxHeight="300" CssPrefix="blog_rte_" />
  <div class="dnnFormItem" id="rowLocale" runat="server">
   <dnn:Label ID="lblLocale" runat="server" controlname="ddLocale" suffix=":" />
   <asp:DropDownList ID="ddLocale" runat="server" DataTextField="NativeName" />
  </div>
  <div class="dnnFormItem">
   <dnn:Label ID="lblImage" runat="server" controlname="fileImage" suffix=":" />
   <div style="float:left;">
   <asp:Image runat="server" ID="imgPostImage" /><br /><br />
   <asp:FileUpload runat="server" ID="fileImage" Width="300" />&nbsp;
   <asp:LinkButton runat="server" ID="cmdImageRemove" resourcekey="cmdImageRemove" cssclass="dnnSecondaryAction" />
   </div>
  </div>
		<asp:Panel ID="pnlCategories" runat="server" class="dnnFormItem">
			<dnn:Label ID="lblCategories" ResourceKey="lblCategories" runat="server" controlname="dtCategories" suffix=":" />
			<div class="dnnLeft">
    <blog:CategorySelect ID="ctlCategories" runat="server" />
			</div>    
		</asp:Panel>
  <div class="dnnClear"></div>
		<div style="display:block">
			<dnn:Label ID="lblTags" runat="server" controlname="txtTags" suffix=":" /><div></div>
			<blog:TagEdit ID="ctlTags" runat="server" width="500px" AllowSpaces="True" />	
		</div>
	</fieldset>
 </div>
 <div class="dnnClear" id="dnnBlogEditPublishing">
	<fieldset>
		<asp:Panel ID="pnlPublished" runat="server" class="dnnFormItem">
			<dnn:Label ID="lblPublished" runat="server" controlname="chkPublished" suffix=":" />
			<asp:CheckBox ID="chkPublished" runat="server" />
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
			<dnn:Label ID="lblPublishDate" runat="server" ControlName="dpPostDate" Suffix=":" />
			<div class="dnnLeft">
				<dnnweb:DnnDatePicker ID="dpPostDate" runat="server" CssClass="dateFix" />
				<dnnweb:DnnTimePicker ID="tpPostTime" runat="server" TimeView-Columns="4" ShowPopupOnFocus="true" CssClass="dateFix" />
			</div>
		</div>
		<div class="dnnClear" style="padding:10px 0px;">
   <em><%= LocalizeString("PublishTimeZoneNote")%>&nbsp;<asp:Literal runat="server" ID="litTimezone" /></em>
		</div>
	</fieldset>
 </div>
	<ul class="dnnActions">
		<li><asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdSave" /></li>
		<li><asp:HyperLink ID="hlCancel" ResourceKey="cmdCancel" runat="server" CssClass="dnnSecondaryAction" /></li>
		<li><asp:LinkButton ID="cmdDelete" ResourceKey="cmdDelete" runat="server" CssClass="dnnSecondaryAction dnnPostDelete" CausesValidation="False" Visible="False" /></li>
	</ul>
	<div class="dnnFormItem">
		<asp:ValidationSummary ID="valSummary" CssClass="dnnFormMessage dnnFormValidationSummary" EnableClientScript="False" runat="server" DisplayMode="BulletList" />
	</div>
</div>
<asp:CustomValidator ID="valPost" EnableClientScript="False" runat="server" ResourceKey="valPost.ErrorMessage" Display="None" />
<asp:CustomValidator ID="valUpload" EnableClientScript="False" runat="server" Display="None" />
<script language="javascript" type="text/javascript">
 (function ($, Sys) {

  $(document).ready(function () {

   $('#dnnBlogEditPost').dnnTabs();

   $('.dnnPostDelete').dnnConfirm({
    text: '<%= LocalizeJSString("DeleteItem") %>',
    yesText: '<%= LocalizeJSString("Yes.Text", Localization.SharedResourceFile) %>',
    noText: '<%= LocalizeJSString("No.Text", Localization.SharedResourceFile) %>',
    title: '<%= LocalizeJSString("Confirm.Text", Localization.SharedResourceFile) %>'
   });

  });

 } (jQuery, window.Sys));
</script>  
