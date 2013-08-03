<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.ViewSettings" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<fieldset>
	<div class="dnnFormItem">
  <dnn:label id="lblBlogModuleId" runat="server" controlname="ddBlogModuleId" suffix=":" />
  <asp:DropDownList runat="server" ID="ddBlogModuleId" DataTextField="ModuleTitle" DataValueField="ModuleId" AutoPostBack="true" />
	</div>			
	<div class="dnnFormItem">
  <dnn:label id="lblBlogId" runat="server" controlname="ddBlogId" suffix=":" />
  <asp:DropDownList runat="server" ID="ddBlogId" DataTextField="LocalizedTitle" DataValueField="BlogId" />
	</div>			
	<div class="dnnFormItem">
  <dnn:label id="lblTermId" runat="server" controlname="ddTermId" suffix=":" />
  <asp:DropDownList runat="server" ID="ddTermId" DataTextField="LocalizedName" DataValueField="TermId" />
	</div>			
	<div class="dnnFormItem">
  <dnn:label id="lblAuthorId" runat="server" controlname="ddAuthorId" suffix=":" />
  <asp:DropDownList runat="server" ID="ddAuthorId" DataTextField="DisplayName" DataValueField="UserId" />
	</div>			
	<div class="dnnFormItem">
  <dnn:label id="lblTemplate" runat="server" controlname="ddTemplate" suffix=":" />
  <asp:DropDownList runat="server" ID="ddTemplate" />
	</div>			
	<div class="dnnFormItem">
		<dnn:label id="lblShowAllLocales" runat="server" controlname="chkShowAllLocales" suffix=":" />
		<asp:CheckBox ID="chkShowAllLocales" runat="server" />
	</div>
</fieldset>
