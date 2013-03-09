<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Settings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Settings" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<h2 class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("secMisc")%></a></h2>
<fieldset>
	<div class="dnnFormItem">
  <dnn:label id="lblTemplate" runat="server" controlname="ddTemplate" suffix=":" />
  <asp:DropDownList runat="server" ID="ddTemplate" />
	</div>			
	<div class="dnnFormItem">
		<dnn:label id="lblAllowMultipleCategories" runat="server" controlname="chkAllowMultipleCategories" suffix=":" />
		<asp:CheckBox ID="chkAllowMultipleCategories" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowHtmlSummary" runat="server" controlname="chkAllowHtmlSummary" suffix=":" />
		<asp:CheckBox ID="chkAllowHtmlSummary" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowAttachments" runat="server" controlname="chkAllowAttachments" suffix=":" />
		<asp:CheckBox ID="chkAllowAttachments" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowWLW" runat="server" controlname="chkAllowWLW" suffix=":" />
		<asp:CheckBox ID="chkAllowWLW" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblAllowAllLocales" runat="server" controlname="chkAllowAllLocales" suffix=":" />
		<asp:CheckBox ID="chkAllowAllLocales" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblEmail" runat="server" controlname="txtEmail" suffix=":" />
		<asp:TextBox runat="server" ID="txtEmail" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblVocabularyId" runat="server" controlname="ddVocabularyId" suffix=":" />
		<asp:DropDownList ID="ddVocabularyId" runat="server" DataValueField="VocabularyID" DataTextField="Name" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblWLWRecentEntriesMax" runat="server" controlname="txtWLWRecentEntriesMax" suffix=":" />
		<asp:TextBox runat="server" ID="txtWLWRecentEntriesMax" />
	</div>
</fieldset>
