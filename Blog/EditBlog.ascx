<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditBlog" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnEditBlog dnnClear" id="dnnEditBlog">
    <div id="ebContent">
        <h2 id="dnnSitePanel-Options" class="dnnFormSectionHead"><%=LocalizeString("lblOptions")%></h2>
        <fieldset>
            <div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblTitle" runat="server" controlname="txtTitle" suffix=":" />
                <asp:TextBox ID="txtTitle" runat="server" ResourceKey="txtTitle" CssClass="dnnFormRequired" />
                <asp:RequiredFieldValidator ID="valTitle" runat="server" Display="Dynamic" ControlToValidate="txtTitle" resourcekey="valTitle.ErrorMessage" CssClass="dnnFormError" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblDescription" runat="server" controlname="txtDescription" suffix=":" />
                <asp:TextBox ID="txtDescription" runat="server" Rows="5" TextMode="MultiLine" CssClass="dnnFormRequired" />
                <asp:RequiredFieldValidator ID="valTitleDescription" runat="server" Display="Dynamic" resourcekey="valTitleDescription.ErrorMessage" ControlToValidate="txtDescription" CssClass="dnnFormError" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblPublic" runat="server" controlname="chkPublic" suffix=":" />
                <asp:CheckBox ID="chkPublic" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblUserIdentity" runat="server" controlname="rdoUserName" suffix=":" />
                <asp:RadioButtonList ID="rdoUserName" CssClass="dnnFormRadioButtons" runat="server">
                    <asp:ListItem Value="False" Selected="True" ResourceKey="rdoUserName_UserName" />
                    <asp:ListItem Value="True" ResourceKey="rdoUserName_FullName" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblAuthorMode" runat="server" controlname="ddlAuthorMode" suffix=":" />
                <asp:DropDownList ID="ddlAuthorMode" runat="server">
                    <asp:ListItem Value="0" ResourceKey="PersonalMode" />
                    <asp:ListItem Value="1" ResourceKey="GhostMode" />
                    <asp:ListItem Value="2" ResourceKey="BloggerMode" />
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblMetaWebBlog" runat="server" controlname="" suffix=":" />
                <div>
                    <asp:Label ID="lblMetaWeblogNotAvailable" runat="server" ResourceKey="lblMetaWeblogNotAvailable" Visible="false" />
                    <asp:Label ID="lblMetaWeblogUrl" runat="server" Text="http://www.yourdomain.com/desktopmodules/blog/blogpost.ashx" />
                </div>
            </div>
        </fieldset>
        <h2 id="dnnSitePanel-CommentOptions" class="dnnFormSectionHead"><%=LocalizeString("lblCommentOptions")%></h2>
        <fieldset>
            <asp:Panel ID="pnlComments" runat="server" CssClass="dnnFormItem">
                <dnn:Label ID="lblAllowComments" runat="server" controlname="chkAllowComments" suffix=":" />
                <asp:CheckBox ID="chkAllowComments" runat="server" />        
            </asp:Panel>
            <div class="dnnFormItem">
                <dnn:Label ID="lblSyndicate" runat="server" controlname="chkSyndicate" suffix=":" />
                <asp:CheckBox ID="chkSyndicate" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblSyndicationEmail" runat="server" controlname="txtSyndicationEmail" suffix=":" />
                <asp:TextBox ID="txtSyndicationEmail" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblRegenerate" runat="server" controlname="cmdGenerateLinks" suffix=":" />
                <asp:LinkButton ID="cmdGenerateLinks" runat="server" CausesValidation="False" resourceKey="cmdGenerateLinks" CssClass="dnnSecondaryAction" />
            </div>
        </fieldset>
        <h2 id="dnnSitePanelChildBlogs" class="dnnFormSectionHead" runat="server"><%=LocalizeString("lblChildBlogs")%></h2>
        <fieldset runat="server" id="fsChildBlogs">
            <div>
                <p><%=LocalizeString("ChildBlogsDesc")%></p>
            </div>
            <div class="dnnFormItem">
                <asp:ListBox ID="lstChildBlogs" runat="server" Rows="5" Width="300" />
            </div>
            <ul class="dnnActions">
                <li><asp:LinkButton ID="cmdAddChildBlog" CssClass="dnnSecondaryAction" runat="server" Enabled="False" resourceKey="cmdAdd" /></li>
                <li><asp:LinkButton ID="cmdEditChildBlog" CssClass="dnnSecondaryAction" runat="server" Enabled="False" resourceKey="cmdEdit" /></li>
                <li><asp:LinkButton ID="cmdDeleteChildBlog" CssClass="dnnSecondaryAction dnnChildDelete" runat="server" Enabled="False" resourceKey="cmdDelete" /></li>
            </ul>
        </fieldset>
    </div>
    <ul class="dnnActions">
        <li><asp:LinkButton ID="cmdUpdate" CssClass="dnnPrimaryAction" runat="server" resourceKey="cmdUpdate" /></li>
        <li><asp:HyperLink ID="hlCancel" runat="server" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
        <li><asp:LinkButton ID="cmdDelete" CssClass="dnnSecondaryAction dnnBlogDelete" runat="server" CausesValidation="False" Visible="False" resourceKey="cmdDelete" /></li>
    </ul>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupDnnBlogSettings() {
            $('.dnnBlogDelete').dnnConfirm({
                text: '<%= LocalizeString("msgDeleteBlog") %>',
                yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
                noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
                title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
            });
            $('.dnnChildDelete').dnnConfirm({
                text: '<%= LocalizeString("msgDeleteChildBlog") %>',
                yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
                noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
                title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
            });
        };

        $(document).ready(function () {
            setupDnnBlogSettings();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupDnnBlogSettings();
            });
        });

    } (jQuery, window.Sys));
</script>   