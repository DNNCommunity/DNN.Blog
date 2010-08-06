<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ModuleOptions.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ModuleOptions" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table cellspacing="0" cellpadding="2" summary="ViewBlog setting design table" border="0">
 <tr>
  <td valign="top">
   <dnn:sectionhead id="secGeneralSettings" isExpanded="true" includerule="True" resourcekey="secGeneralSettings" section="tblGeneralSetting" runat="server" cssclass="Head" />
   <table id="tblGeneralSetting" cellspacing="2" cellpadding="2" width="100%" summary="Edit ViewBlog Basic Settings" border="0" runat="server">
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblMandatory" runat="server" controlname="chkForceDescription" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkForceDescription" runat="server" TextAlign="Left" AutoPostBack="True" />
     </td>
    </tr>
    <tr id="trSummary" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblSummary" runat="server" controlname="txtSummaryLimit" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtSummaryLimit" runat="server" Text="1024" />
     </td>
    </tr>
    <tr id="trSearchSummary" valign="top" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblSearchSummary" runat="server" controlname="txtSearchLimit" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtSearchLimit" runat="server" Text="255" />
     </td>
    </tr>
    <tr id="Tr1" valign="top" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblMaxImageWidth" runat="server" controlname="txtMaxImageWidth" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtMaxImageWidth" runat="server" Text="400" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblRecentEntriesMax" runat="server" controlname="txtRecentEntriesMax" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtRecentEntriesMax" runat="server" Text="10" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblRecentRssEntriesMax" runat="server" controlname="txtRecentRssEntriesMax" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtRecentRssEntriesMax" runat="server" Text="10" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
 <tr>
  <td valign="top">
   <dnn:sectionhead id="secCommentSettings" isExpanded="True" includerule="True" resourcekey="secCommentSettings" section="tblCommentSettings" runat="server" cssclass="Head" />
   <table id="tblCommentSettings" cellspacing="2" cellpadding="2" width="100%" summary="Edit Blog Comment Settings" border="0" runat="server">
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblShowGravatars" runat="server" controlname="lblShowGravatars" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkShowGravatars" runat="server" AutoPostBack="True" />
     </td>
    </tr>
    <tr id="trGravatarImageWidth" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblGravatarImageWidth" runat="server" controlname="lblGravatarImageWidth" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtGravatarImageWidth" runat="server" Text="48" />
     </td>
    </tr>
    <tr id="trGravatarRating" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblGravatarRating" runat="server" controlname="lblGravatarRating" suffix="" />
     </td>
     <td>
      <asp:RadioButtonList ID="rblGravatarRating" runat="server">
       <asp:ListItem Value="G" Selected="True" ResourceKey="rblGravatarRating_g" />
       <asp:ListItem Value="PG" ResourceKey="rblGravatarRating_pg" />
       <asp:ListItem Value="R" ResourceKey="rblGravatarRating_r" />
       <asp:ListItem Value="X" ResourceKey="rblGravatarRating_x" />
      </asp:RadioButtonList>
     </td>
    </tr>
    <tr id="trGravatarDefaultImageUrl" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblGravatarDefaultImageUrl" runat="server" controlname="lblGravatarDefaultImageUrl" suffix="" />
     </td>
     <td>
      <asp:RadioButtonList ID="rblDefaultImage" runat="server">
       <asp:ListItem Value="" Selected="True" Text="Gray Man" />
       <asp:ListItem Value="identicon" Text="Identicon" />
       <asp:ListItem Value="wavatar" Text="Wavatar" />
       <asp:ListItem Value="monsterid" Text="MonsterID" />
       <asp:ListItem Value="custom" Text="Custom" />
      </asp:RadioButtonList>
     </td>
    </tr>
    <tr id="trGravatarDefaultImageCustomURL" runat="server">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblGravatarDefaultImageCustomURL" runat="server" controlname="lblGravatarDefaultImageCustomURL" suffix="" />
     </td>
     <td>
      <asp:TextBox ID="txtGravatarDefaultImageCustomURL" runat="server" Width="290px" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblShowWebsite" runat="server" controlname="lblShowWebsite" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkShowWebsite" Checked="True" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblShowTitle" runat="server" controlname="lblShowTitle" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkShowCommentTitle" Checked="True" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblAllowCommentAnchors" runat="server" controlname="lblAllowCommentAnchors" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkAllowCommentAnchors" Checked="True" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblAllowCommentImages" runat="server" controlname="lblAllowCommentImages" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkAllowCommentImages" Checked="True" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblAllowCommentFormatting" runat="server" controlname="lblAllowCommentFormatting" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkAllowCommentFormatting" Checked="True" runat="server" TextAlign="Left" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <dnn:sectionhead id="secSEOSettings" isExpanded="true" includerule="True" resourcekey="tblSEOSettings" section="tblSEOSettings" runat="server" cssclass="Head" />
   <table id="tblSEOSettings" cellspacing="0" cellpadding="2" width="100%" summary="Edit ViewBlog SEO Settings" border="0" runat="server">
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblShowUniqueTitle" runat="server" controlname="chkShowUniqueTitle" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkShowUniqueTitle" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblShowSeoFriendlyUrl" runat="server" controlname="chkShowSeoFriendlyUrl" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkShowSeoFriendlyUrl" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblRegenerateLinks" runat="server" suffix="" controlname="cmdGenerateLinks" />
     </td>
     <td>
      <asp:LinkButton ID="cmdGenerateLinks" runat="server" CausesValidation="False" BorderStyle="none" ResourceKey="cmdGenerateLinks" CssClass="CommandButton" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <dnn:sectionhead id="secRSSSettings" isExpanded="true" includerule="True" resourcekey="secRSSSettings" section="tblRSSSettings" runat="server" cssclass="Head" />
   <table id="tblRSSSettings" cellspacing="0" cellpadding="2" width="100%" summary="Edit Blog RSS Settings" border="0" runat="server">
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblIncludeBody" runat="server" controlname="chkIncludeBody" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkIncludeBody" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblIncludeCategoriesInDescription" runat="server" controlname="chkIncludeCategoriesInDescription" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkIncludeCategoriesInDescription" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblIncludeTagsInDescription" runat="server" controlname="chkIncludeTagsInDescription" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkIncludeTagsInDescription" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblFeedCacheTime" runat="server" controlname="txtFeedCacheTime" suffix="" />
     </td>
     <td>
      <asp:TextBox runat="server" ID="txtFeedCacheTime" Width="100" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <dnn:sectionhead id="secAdvancedSettings" isExpanded="true" includerule="True" resourcekey="tblAdvancedSettings" section="tblAdvancedSettings" runat="server" cssclass="Head" />
   <table id="tblAdvancedSettings" cellspacing="0" cellpadding="2" width="100%" summary="Edit ViewBlog Advanced Settings" border="0" runat="server">
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblAllowWLW" runat="server" controlname="chkAllowWLW" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkAllowWLW" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblUploadOption" runat="server" suffix="" controlname="chkUploadOption" />
     </td>
     <td>
      <asp:CheckBox ID="chkUploadOption" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblPageBlogs" runat="server" controlname="cmbPageBlogs" suffix="" />
     </td>
     <td>
      <asp:DropDownList ID="cmbPageBlogs" AutoPostBack="True" DataValueField="BlogID" DataTextField="Title" runat="server" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblEnableDNNSearch" runat="server" controlname="chkEnableDNNSearch" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkEnableDNNSearch" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblEnableBookmarks" runat="server" suffix="" controlname="lblEnableBookmarks" />
     </td>
     <td>
      <asp:CheckBox ID="chkEnableBookmarks" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblEnforceSummaryTruncation" runat="server" suffix="" controlname="lblEnforceSummaryTruncation" />
     </td>
     <td>
      <asp:CheckBox ID="chkEnforceSummaryTruncation" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblShowSummary" runat="server" controlname="chkShowSummary" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkShowSummary" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblAllowSummaryHtml" runat="server" controlname="chkAllowSummaryHtml" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkAllowSummaryHtml" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblAllowChildBlogs" runat="server" controlname="chkAllowChildBlogs" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkAllowChildBlogs" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr>
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblEnableArchiveDropDown" runat="server" controlname="chkEnableArchiveDropDown" suffix="" />
     </td>
     <td>
      <asp:CheckBox ID="chkEnableArchiveDropDown" runat="server" TextAlign="Left" />
     </td>
    </tr>
    <tr valign="top">
     <td class="SubHead" valign="top" width="300">
      <dnn:label id="lblMigrateChildblogs" runat="server" suffix="" controlname="cmdMigrateChildblogs" />
     </td>
     <td>
      <asp:Label runat="server" ID="lblChildBlogsStatus" /><br />
      <asp:LinkButton ID="cmdMigrateChildblogs" runat="server" CausesValidation="False" BorderStyle="none" ResourceKey="cmdMigrateChildblogs" CssClass="CommandButton" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <asp:LinkButton class="CommandButton" ID="cmdUpdateOptions" runat="server" CausesValidation="False" BorderStyle="none" ResourceKey="cmdUpdate" />&nbsp;
   <asp:LinkButton ID="cmdCancelOptions" runat="server" CausesValidation="False" BorderStyle="none" ResourceKey="cmdCancel" CssClass="CommandButton" />
  </td>
 </tr>
</table>
<p>
</p>
