<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditBlog" %>
<asp:ValidationSummary ID="valSummary" EnableClientScript="True" runat="server" CssClass="NormalRed" />
<asp:RequiredFieldValidator ID="valTitle" runat="server" Display="None" ErrorMessage="Title is Required" ControlToValidate="txtTitle" resourcekey="valTitle.ErrorMessage" />
<asp:RequiredFieldValidator ID="valTitleDescription" runat="server" Display="None" ErrorMessage="Title Description is Required" resourcekey="valTitleDescription.ErrorMessage" ControlToValidate="txtDescription" />
<table class="Normal" id="Table2" cellspacing="1" cellpadding="1" width="500" border="0">
 <tr>
  <td>
   <asp:Label ID="lblOptions" CssClass="SubHead" runat="server" ResourceKey="lblOptions" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblOptionsDescription" runat="server" ResourceKey="lblOptionsDescription" />
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTitle" CssClass="SubHead" runat="server" ResourceKey="lblTitle" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTitleDescription" runat="server" ResourceKey="lblTitleDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:TextBox ID="txtTitle" CssClass="NormalTextBox" runat="server" Width="100%" ResourceKey="txtTitle" />
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblDescription" CssClass="SubHead" runat="server" ResourceKey="lblDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblDescriptionDescription" runat="server" ResourceKey="lblDescriptionDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:TextBox ID="txtDescription" CssClass="NormalTextBox" runat="server" Width="100%" Rows="5" TextMode="MultiLine" />
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <asp:CheckBox ID="chkPublic" CssClass="Normal" runat="server" ResourceKey="chkPublic" />
  </td>
 </tr>
 <tr>
  <td>
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblUserIdentity" runat="server" ResourceKey="lblUserIdentity" /><br />
   <asp:RadioButtonList ID="rdoUserName" CssClass="Normal" runat="server" RepeatDirection="Horizontal">
    <asp:ListItem Value="False" Selected="True" ResourceKey="rdoUserName_UserName" />
    <asp:ListItem Value="True" ResourceKey="rdoUserName_FullName" />
   </asp:RadioButtonList>
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblMetaWeblogOptions" CssClass="SubHead" runat="server" ResourceKey="lblMetaWeblogOptions" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblMetaWeblogOptionsDescription" runat="server" ResourceKey="lblMetaWeblogOptionsDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <br />
   <asp:Label ID="lblMetaWeblogUrl" runat="server" Text="http://www.yourdomain.com/desktopmodules/blog/blogpost.ashx" />
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTwitterIntegration" CssClass="SubHead" runat="server" ResourceKey="lblTwitterIntegration" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:CheckBox ID="chkEnableTwitterIntegration" CssClass="Normal" runat="server" ResourceKey="chkEnableTwitterIntegration" AutoPostBack="true" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTwitterUsername" runat="server" ResourceKey="lblTwitterUsername" /><br />
   <asp:TextBox class="NormalTextBox" ID="txtTwitterUsername" runat="server" Width="300px" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTwitterPassword" runat="server" ResourceKey="lblTwitterPassword" /><br />
   <asp:TextBox class="NormalTextBox" ID="txtTwitterPassword" runat="server" Width="300px" TextMode="Password" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTweetTemplate" runat="server" ResourceKey="lblTweetTemplate" /><br />
   <asp:TextBox class="NormalTextBox" ID="txtTweetTemplate" runat="server" Width="300px" Height="61px" TextMode="MultiLine" />
   <br />
   <i><b><asp:Label runat="server" ID="lblAvailableTokens" resourcekey="lblAvailableTokens" /></b></i> {title}, {url}
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblCommentOptions" CssClass="SubHead" runat="server" ResourceKey="lblCommentOptions" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblCommentOptionsDescription" runat="server" ResourceKey="lblCommentOptionsDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <p>
    <asp:Label ID="lblUsersComments" runat="server" ResourceKey="lblUsersComments" />
    <asp:RadioButtonList ID="rdoUsersComments" runat="server" RepeatDirection="Horizontal" CssClass="Normal">
     <asp:ListItem Value="Allow" ResourceKey="rdoComments_Allow" Selected="True" />
     <asp:ListItem Value="RequireApproval" ResourceKey="rdoComments_RequireApproval" />
     <asp:ListItem Value="Disallow" ResourceKey="rdoComments_Disallow" />
    </asp:RadioButtonList>
    <asp:Label ID="lblAnonymousComments" runat="server" ResourceKey="lblAnonymousComments" />
    <asp:RadioButtonList ID="rdoAnonymousComments" runat="server" RepeatDirection="Horizontal" CssClass="Normal">
     <asp:ListItem Value="Allow" ResourceKey="rdoComments_Allow" Selected="True" />
     <asp:ListItem Value="RequireApproval" ResourceKey="rdoComments_RequireApproval" />
     <asp:ListItem Value="Disallow" ResourceKey="rdoComments_Disallow" />
    </asp:RadioButtonList>
    <asp:Label ID="lblTrackbacks" runat="server" ResourceKey="lblTrackbacks" />
    <asp:RadioButtonList ID="rdoTrackbacks" runat="server" RepeatDirection="Horizontal" CssClass="Normal">
     <asp:ListItem Value="Allow" ResourceKey="rdoComments_Allow" Selected="True" />
     <asp:ListItem Value="RequireApproval" ResourceKey="rdoComments_RequireApproval" />
     <asp:ListItem Value="Disallow" ResourceKey="rdoComments_Disallow" />
    </asp:RadioButtonList>
    <asp:CheckBox ID="chkEmailNotification" CssClass="Normal" runat="server" ResourceKey="chkEmailNotification" /><br />
    <asp:CheckBox ID="chkCaptcha" CssClass="Normal" runat="server" ResourceKey="chkCaptcha" /><br />
   </p>
   <p>
   </p>
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTrackbackOptions" CssClass="SubHead" runat="server" ResourceKey="lblTrackbackOptions" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTrackbackOptionsDescription" runat="server" ResourceKey="lblTrackbackOptionsDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <p>
    <asp:CheckBox ID="chkAutoTrackbacks" CssClass="Normal" runat="server" ResourceKey="chkAutoTrackbacks" /></p>
   <p>
   </p>
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblSyndicationOptions" CssClass="SubHead" runat="server" ResourceKey="lblSyndicationOptions" />
  </td>
 </tr>
 <tr>
  <td>
   <p>
    <asp:CheckBox ID="chkSyndicate" CssClass="Normal" runat="server" ResourceKey="chkSyndicate" /><br />
    <asp:CheckBox ID="chkSyndicateIndependant" CssClass="Normal" runat="server" ResourceKey="chkSyndicateIndependant" />
   </p>
   <p class="Normal">
    <asp:Label ID="lblSyndicationEmail" runat="server" ResourceKey="lblSyndicationEmail" /><br />
    <asp:TextBox class="NormalTextBox" ID="txtSyndicationEmail" runat="server" Width="300px" /></p>
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   &nbsp;
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblDateTime" CssClass="SubHead" runat="server" ResourceKey="lblDateTime" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblDateTimeDescription" runat="server" ResourceKey="lblDateTimeDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <table id="Table3" cellspacing="1" cellpadding="1" width="100%" border="0">
    <tr>
     <td height="20">
      <asp:Label ID="lblTimeZone" CssClass="SubHead" runat="server" ResourceKey="lblTimeZone" />
     </td>
     <td height="20">
      <asp:DropDownList ID="cboTimeZone" Width="400px" CssClass="NormalTextBox" runat="server" AutoPostBack="True" />
     </td>
    </tr>
    <tr>
     <td>
      <asp:Label ID="lblCulture" CssClass="SubHead" runat="server" ResourceKey="lblCulture" />
     </td>
     <td>
      <asp:DropDownList ID="cboCulture" Width="400px" CssClass="NormalTextBox" runat="server" AutoPostBack="True" />
     </td>
    </tr>
    <tr>
     <td>
      <asp:Label ID="lblDateFormat" CssClass="SubHead" runat="server" ResourceKey="lblDateFormat" />
     </td>
     <td>
      <asp:DropDownList ID="cboDateFormat" Width="400px" CssClass="NormalTextBox" runat="server" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblRegenerate" CssClass="SubHead" runat="server" ResourceKey="lblRegenerate" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblRegenerateDescription" runat="server" ResourceKey="lblRegenerateDescription" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:LinkButton ID="cmdGenerateLinks" runat="server" CausesValidation="False" BorderStyle="none" ResourceKey="cmdGenerateLinks" CssClass="CommandButton" />
  </td>
 </tr>
 <tr>
  <td>
   <hr />
   <br />
  </td>
 </tr>
 <asp:Panel ID="pnlChildBlogs" runat="server">
  <tr>
   <td>
    <asp:Label ID="lblChildBlogs" ResourceKey="lblChildBlogs" runat="server" CssClass="SubHead" />
   </td>
  </tr>
  <tr>
   <td>
    <asp:Label ID="lblChildBlogsDescription" ResourceKey="lblChildBlogsDescription" runat="server" />
   </td>
  </tr>
  <tr>
   <td>
    <table id="Table1" cellspacing="1" cellpadding="0" width="100%" border="0">
     <tr>
      <td width="100%" rowspan="3">
       <asp:ListBox ID="lstChildBlogs" CssClass="NormalTextBox" runat="server" Width="100%" Rows="5" />
      </td>
      <td width="40">
       <asp:Button ID="btnAddChildBlog" CssClass="Normal" runat="server" Enabled="False" Width="70px" resourceKey="cmdAdd" />
      </td>
     </tr>
     <tr>
      <td width="40">
       <asp:Button ID="btnEditChildBlog" CssClass="Normal" runat="server" Enabled="False" Width="70px" resourceKey="cmdEdit" />
      </td>
     </tr>
     <tr>
      <td width="40">
       <asp:Button ID="btnDeleteChildBlog" CssClass="Normal" runat="server" Enabled="False" Width="70px" resourceKey="cmdDelete" />
      </td>
     </tr>
    </table>
   </td>
  </tr>
  <tr>
   <td><asp:Label runat="server" ID="lblChildBlogsOff" resourcekey="lblChildBlogsOff" Visible="true" CssClass="NormalRed" /></td>
  </tr>
  <tr>
   <td>
    <hr />
    <br />
   </td>
  </tr>
 </asp:Panel>
</table>
<asp:LinkButton ID="cmdUpdate" CssClass="CommandButton" runat="server" BorderStyle="None" ResourceKey="cmdUpdate" />&nbsp;
<asp:LinkButton ID="cmdCancel" CssClass="CommandButton" runat="server" CausesValidation="False" BorderStyle="None" ResourceKey="cmdCancel" />&nbsp;
<asp:LinkButton ID="cmdDelete" CssClass="CommandButton" runat="server" CausesValidation="False" BorderStyle="None" Visible="False" ResourceKey="cmdDelete" />
