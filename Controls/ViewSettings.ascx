<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.ViewSettings" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="blog" Namespace="DotNetNuke.Modules.Blog.Controls" Assembly="DotNetNuke.Modules.Blog" %>

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
        <asp:Panel ID="pnlCategories" runat="server" class="dnnFormItem">
		    <dnn:Label ID="lblCategories" ResourceKey="lblCategories" runat="server" controlname="dtCategories" suffix=":" />
		    <blog:CategorySelect ID="ctlCategories" runat="server" />
	    </asp:Panel>
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
    <div class="dnnFormItem">
        <dnn:label id="lblModifyPageDetails" runat="server" controlname="chkModifyPageDetails" suffix=":" />
        <asp:CheckBox ID="chkModifyPageDetails" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblShowManagementPanel" runat="server" controlname="chkShowManagementPanel" suffix=":" />
        <asp:CheckBox ID="chkShowManagementPanel" runat="server" ResourceKey="chkShowManagementPanel" />
        <asp:CheckBox ID="chkShowManagementPanelViewMode" runat="server" ResourceKey="chkShowManagementPanelViewMode" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblHideUnpublishedBlogs" runat="server" controlname="chkHideUnpublishedBlogsViewMode" suffix=":" />
        <asp:CheckBox ID="chkHideUnpublishedBlogsViewMode" runat="server" ResourceKey="chkHideUnpublishedBlogsViewMode" />
        <asp:CheckBox ID="chkHideUnpublishedBlogsEditMode" runat="server" ResourceKey="chkHideUnpublishedBlogsEditMode" />
    </div>
</fieldset>
