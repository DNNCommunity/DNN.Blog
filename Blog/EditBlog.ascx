<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditBlog" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnEditBlog dnnClear" id="dnnEditBlog">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
    <div>
        <h2 id="dnnSitePanel-Options" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("lblOptions")%></a></h2>
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
                <dnn:Label id="lblEnableGhostWriting" runat="server" controlname="" suffix=":" />
                <asp:CheckBox ID="chkEnableGhostWriting" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblMetaWebBlog" runat="server" controlname="" suffix=":" />
                <div>
                    <asp:Label ID="lblMetaWeblogNotAvailable" runat="server" ResourceKey="lblMetaWeblogNotAvailable" Visible="false" />
                    <asp:Label ID="lblMetaWeblogUrl" runat="server" Text="http://www.yourdomain.com/desktopmodules/blog/blogpost.ashx" />
                </div>
            </div>
        </fieldset>
        <h2 id="dnnSitePanel-CommentOptions" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("lblCommentOptions")%></a></h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="lblUsersComments" runat="server" controlname="rdoUsersComments" />
                <asp:RadioButtonList ID="rdoUsersComments" runat="server" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Value="Allow" resourceKey="rdoComments_Allow" Selected="True" />
                    <asp:ListItem Value="RequireApproval" resourceKey="rdoComments_RequireApproval" />
                    <asp:ListItem Value="Disallow" resourceKey="rdoComments_Disallow" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblAnonymousComments" runat="server" controlname="rdoAnonymousComments" suffix=":" />
                <asp:RadioButtonList ID="rdoAnonymousComments" runat="server" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Value="Allow" resourceKey="rdoComments_Allow" Selected="True" />
                    <asp:ListItem Value="RequireApproval" resourceKey="rdoComments_RequireApproval" />
                    <asp:ListItem Value="Disallow" resourceKey="rdoComments_Disallow" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblTrackbacks" runat="server" controlname="rdoTrackbacks" suffix=":" />
                <asp:RadioButtonList ID="rdoTrackbacks" runat="server" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Value="Allow" resourceKey="rdoComments_Allow" Selected="True" />
                    <asp:ListItem Value="RequireApproval" resourceKey="rdoComments_RequireApproval" />
                    <asp:ListItem Value="Disallow" resourceKey="rdoComments_Disallow" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblCaptcha" runat="server" controlname="chkCaptcha" suffix=":" />
                <asp:CheckBox ID="chkCaptcha" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblAutoTrackbacks" runat="server" controlname="chkAutoTrackbacks" suffix=":" />
                <asp:CheckBox ID="chkAutoTrackbacks" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblEmailNotification" runat="server" controlname="chkEmailNotification" suffix=":" />
                <asp:CheckBox ID="chkEmailNotification" runat="server" />           
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblSyndicate" runat="server" controlname="chkSyndicate" suffix=":" />
                <asp:CheckBox ID="chkSyndicate" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblSyndicateIndependant" runat="server" controlname="chkSyndicateIndependant" suffix=":" />
                <asp:CheckBox ID="chkSyndicateIndependant" runat="server" />
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
        <h2 id="dnnSitePanel-TimeOptions" class="dnnFormSectionHead"><a href="" class=""><%=LocalizeString("lblTimeOptions")%></a></h2>
        <fieldset>            
            <div class="dnnFormItem">
                <dnn:Label ID="lblTimeZone" runat="server" controlname="cboTimeZone" suffix=":" />
                <dnnweb:DnnTimeZoneComboBox runat="server" id="ddTimeZone" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblCulture" runat="server" controlname="cboCulture" suffix=":" />
                <asp:DropDownList ID="cboCulture" runat="server" AutoPostBack="True" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblDateFormat" runat="server" controlname="cboDateFormat" suffix=":" />
                <asp:DropDownList ID="cboDateFormat" runat="server" />
            </div>
        </fieldset>
        <h2 id="dnnSitePanelChildBlogs" class="dnnFormSectionHead" runat="server"><a href="" class=""><%=LocalizeString("lblChildBlogs")%></a></h2>
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
            <div>
                <asp:Label runat="server" ID="lblChildBlogsOff" resourcekey="lblChildBlogsOff" Visible="true" CssClass="NormalRed" />
            </div>
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
            $('#dnnEditBlog').dnnPanels();

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