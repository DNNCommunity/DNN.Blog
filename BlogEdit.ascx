<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BlogEdit.ascx.vb" Inherits="DotNetNuke.Modules.Blog.BlogEdit" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="blog" Namespace="DotNetNuke.Modules.Blog.Security.Controls" Assembly="DotNetNuke.Modules.Blog" %>
<%@ Register TagPrefix="blog" Namespace="DotNetNuke.Modules.Blog.Controls" Assembly="DotNetNuke.Modules.Blog" %>
<div class="dnnForm dnnBlogEdit dnnClear" id="dnnBlogEdit">
 <div id="ebContent">
  <h2 id="dnnSitePanel-Options" class="dnnFormSectionHead">
   <%=LocalizeString("lblOptions")%></h2>
  <fieldset>
   <div class="dnnFormItem">
    <dnn:Label ID="lblTitle" runat="server" controlname="txtTitle" suffix=":" />
    <blog:ShortTextEdit id="txtTitle" runat="server" Required="True" Width="520" TextBoxWidth="450" CssClass="blog_rte" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblDescription" runat="server" controlname="txtDescription" suffix=":" />
    <blog:LongTextEdit id="txtDescription" runat="server" ShowRichTextBox="False" Width="520" TextBoxWidth="450" CssClass="blog_rte" />
   </div>
   <div class="dnnFormItem" id="rowLocale" runat="server">
    <dnn:Label ID="lblLocale" runat="server" controlname="ddLocale" suffix=":" />
    <asp:DropDownList ID="ddLocale" runat="server" DataTextField="NativeName" />
   </div>
   <div class="dnnFormItem" id="rowFullLocalization" runat="server">
    <dnn:Label ID="lblFullLocalization" runat="server" controlname="chkPublic" suffix=":" />
    <asp:CheckBox ID="chkFullLocalization" runat="server" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblImage" runat="server" controlname="fileImage" suffix=":" />
    <div style="float:left;">
    <asp:Image runat="server" ID="imgBlogImage" /><br /><br />
    <asp:FileUpload runat="server" ID="fileImage" Width="300" />&nbsp;
    <asp:LinkButton runat="server" ID="cmdImageRemove" resourcekey="cmdImageRemove" cssclass="dnnSecondaryAction" />
    </div>
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblPublic" runat="server" controlname="chkPublic" suffix=":" />
    <asp:CheckBox ID="chkPublic" runat="server" />
   </div>
  </fieldset>
  <h2 id="dnnSitePanel-CommentOptions" class="dnnFormSectionHead">
   <%=LocalizeString("lblCommentOptions")%></h2>
  <fieldset>
   <div class="dnnFormItem">
    <dnn:Label ID="lblSyndicate" runat="server" controlname="chkSyndicate" suffix=":" />
    <asp:CheckBox ID="chkSyndicate" runat="server" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblSyndicationEmail" runat="server" controlname="txtSyndicationEmail" suffix=":" />
    <asp:TextBox ID="txtSyndicationEmail" runat="server" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblCopyright" runat="server" controlname="txtCopyright" suffix=":" />
    <asp:TextBox ID="txtCopyright" runat="server" Width="400" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblIncludeImagesInFeed" runat="server" controlname="chkIncludeImagesInFeed" suffix=":" />
    <asp:CheckBox ID="chkIncludeImagesInFeed" runat="server" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblIncludeAuthorInFeed" runat="server" controlname="chkIncludeAuthorInFeed" suffix=":" />
    <asp:CheckBox ID="chkIncludeAuthorInFeed" runat="server" />
   </div>
  </fieldset>
  <h2 id="dnnSitePanel-GhostOptions" class="dnnFormSectionHead">
   <%=LocalizeString("lblGhostOptions")%></h2>
  <fieldset>
   <div class="dnnFormItem">
    <dnn:Label ID="lblMustApproveGhostPosts" runat="server" controlname="chkMustApproveGhostPosts" suffix=":" />
    <asp:CheckBox ID="chkMustApproveGhostPosts" runat="server" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblPublishAsOwner" runat="server" controlname="chkPublishAsOwner" suffix=":" />
    <asp:CheckBox ID="chkPublishAsOwner" runat="server" />
   </div>
   <div class="dnnFormItem">
    <dnn:Label ID="lblPermissions" runat="server" controlname="chkSyndicate" suffix=":" />
    <div style="float:left;width:400px;">
     <blog:BlogPermissionsGrid runat="server" id="ctlPermissions" IncludeAdministratorRole="False" />
    </div>
   </div>
  </fieldset>
 </div>
 <ul class="dnnActions">
  <li>
   <asp:LinkButton ID="cmdUpdate" CssClass="dnnPrimaryAction" runat="server" resourceKey="cmdUpdate" /></li>
  <li>
   <asp:HyperLink ID="hlCancel" runat="server" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" /></li>
  <li>
   <asp:LinkButton ID="cmdDelete" CssClass="dnnSecondaryAction dnnBlogDelete" runat="server"
    CausesValidation="False" Visible="False" resourceKey="cmdDelete" /></li>
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
  };

  $(document).ready(function () {
   setupDnnBlogSettings();
   Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
    setupDnnBlogSettings();
   });
  });

 } (jQuery, window.Sys));
</script>   