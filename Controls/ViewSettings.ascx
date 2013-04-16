<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.ViewSettings" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<fieldset>
	<div class="dnnFormItem">
  <dnn:label id="lblBlogModuleId" runat="server" controlname="ddBlogModuleId" suffix=":" />
  <asp:DropDownList runat="server" ID="ddBlogModuleId" DataTextField="ModuleTitle" DataValueField="ModuleId" />
	</div>			
	<div class="dnnFormItem">
  <dnn:label id="lblTemplate" runat="server" controlname="ddTemplate" suffix=":" />
  <asp:DropDownList runat="server" ID="ddTemplate" />
	</div>			
	<div class="dnnFormItem">
		<dnn:label id="lblShowManagementPanel" runat="server" controlname="chkShowManagementPanel" suffix=":" />
		<asp:CheckBox ID="chkShowManagementPanel" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblShowComments" runat="server" controlname="chkShowComments" suffix=":" />
		<asp:CheckBox ID="chkShowComments" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label id="lblShowAllLocales" runat="server" controlname="chkShowAllLocales" suffix=":" />
		<asp:CheckBox ID="chkShowAllLocales" runat="server" />
	</div>
</fieldset>
