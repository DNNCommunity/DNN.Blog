<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditEntry.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditEntry" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" %>
<p>
 <asp:Label ID="lblPublished" runat="server" CssClass="NormalRed" /><br />
</p>
<asp:ValidationSummary ID="valSummary" CssClass="NormalRed" EnableClientScript="False" runat="server" DisplayMode="BulletList" />
<asp:RequiredFieldValidator ID="valTitle" EnableClientScript="False" runat="server" ResourceKey="valTitle.ErrorMessage" Display="None" ControlToValidate="txtTitle" ErrorMessage="Title is required" />
<asp:RequiredFieldValidator ID="valDescription" EnableClientScript="False" runat="server" ResourceKey="valDescription.ErrorMessage" Display="None" ControlToValidate="txtDescription" ErrorMessage="Description is required" />
<asp:CustomValidator ID="valEntry" EnableClientScript="False" runat="server" ResourceKey="valEntry.ErrorMessage" Display="None" ErrorMessage="Entry is Required" />
<asp:CustomValidator ID="valUpload" EnableClientScript="False" runat="server" Display="None" ErrorMessage="Valid File Types Include JPG, GIF and PNG" />
<asp:RequiredFieldValidator ID="valEntryDate" EnableClientScript="False" runat="server" ResourceKey="valEntryDate.ErrorMessage" Display="None" ControlToValidate="txtEntryDate" ErrorMessage="Entry Date is required" />
<asp:CustomValidator ID="valEntryDateData" EnableClientScript="False" runat="server" ResourceKey="valEntryDateData.ErrorMessage" Display="None" ErrorMessage="Entry Date is not a valid date" />
<table id="Table1" cellspacing="1" cellpadding="1" width="80%" border="0">
 <tr>
  <td style="white-space:nowrap;width:10%;padding-right:10px;">
   <asp:Label ID="lblEntryDate" CssClass="SubHead" ResourceKey="lblEntryDate" runat="server" />
  </td>
  <td width="40%">
   <asp:TextBox ID="txtEntryDate" CssClass="NormalTextBox" runat="server" Width="90%" />
  </td>
  <td style="white-space:nowrap;width:10%;padding-right:10px;">
   <asp:Label ID="lblChildBlog" CssClass="SubHead" ResourceKey="lblChildBlog" runat="server" />
  </td>
  <td width="40%" style="text-align:right">
   <asp:DropDownList ID="cboChildBlogs" CssClass="NormalTextBox" runat="server" Width="90%" ResourceKey="cboChildBlogs.DataTextField" DataValueField="BlogID" DataTextField="Title" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   &nbsp;
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <asp:Label ID="lblTitle" ResourceKey="lblTitle" runat="server" CssClass="SubHead" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <asp:TextBox ID="txtTitle" runat="server" CssClass="NormalTextBox" Width="100%" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <hr style="width: 100%;" size="1" />
   <br />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <asp:Label ID="lblSummary" ResourceKey="lblSummary" runat="server" CssClass="SubHead" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <asp:Label ID="txtDescriptionOptional" CssClass="Normal" runat="server" ResourceKey="txtDescriptionOptional" Visible="False" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <dnn:texteditor id="txtDescription" runat="server" width="100%" height="200">
   </dnn:texteditor>
   <asp:TextBox runat="server" ID="txtDescriptionText" Width="100%" Height="200" TextMode="MultiLine" Visible="false" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <hr width="100%" size="1" />
   <br />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <asp:Label ID="lblDescription" ResourceKey="lblDescription" runat="server" CssClass="SubHead" />
  </td>
 </tr>
 <tr>
  <td colspan="4">
   <dnn:texteditor id="teBlogEntry" runat="server" width="100%" height="400">
   </dnn:texteditor>
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblTags" runat="server" CssClass="SubHead" resourcekey="lblTags" />
  </td>
  <td colspan="3">
   <asp:TextBox ID="tbTags" runat="server" Width="340px" CssClass="NormalTextBox" />
  </td>
 </tr>
 <tr>
  <td>
   <asp:Label ID="lblCategories" runat="server" CssClass="SubHead" resourcekey="lblCategories" />
  </td>
  <td colspan="3">
   <dnn:dnntree runat="server" id="treeCategories" CheckBoxes="True" SystemImagesPath="~/images/" CollapsedNodeImage="~/images/max.gif" ExpandedNodeImage="~/images/min.gif" />
  </td>
 </tr>
 <asp:Panel ID="pnlUploads" runat="server" Visible="true">
  <tr>
   <td colspan="4">
   </td>
  </tr>
  <tr>
   <td colspan="4">
    <dnn:sectionhead id="secUploadOption" runat="server" cssclass="SubHead" isExpanded="false" includerule="True" resourcekey="secUploadOption" section="tblUploadOptions">
    </dnn:sectionhead>
    <table id="tblUploadOptions" runat="server">
     <tr>
      <td style="white-space: nowrap;" width="20%">
       <asp:Label ID="lblAddPicture" runat="server" CssClass="Normal" ResourceKey="lblAddPicture" />
      </td>
      <td width="50%">
       <input id="picFilename" type="file" size="40" name="picFilename" runat="server" />
      </td>
      <td width="30%">
       <asp:Button ID="btnUploadPicture" runat="server" ResourceKey="btnUploadPicture" />
      </td>
     </tr>
     <tr>
      <td>
       <asp:Label ID="lblAltText" runat="server" CssClass="Normal" ResourceKey="lblAltText" />
      </td>
      <td>
       <asp:TextBox ID="txtAltText" runat="server" CssClass="Normal" size="40" />
      </td>
      <td>
      </td>
     </tr>
     <tr>
      <td style="white-space: nowrap;" width="20%">
       <asp:Label ID="lblAddAttachment" runat="server" CssClass="Normal" ResourceKey="lblAddAttachment" />
      </td>
      <td width="50%">
       <input id="attFilename" type="file" size="40" name="attFilename" runat="server" />
      </td>
      <td width="30%">
       <asp:Button ID="btnUploadAttachment" runat="server" ResourceKey="btnUploadAttachment" />
      </td>
     </tr>
     <tr>
      <td>
       <asp:Label ID="lblAttachmentDescription" runat="server" CssClass="Normal" ResourceKey="lblAttachmentDescription" />
      </td>
      <td>
       <asp:TextBox ID="txtAttachmentDescription" runat="server" CssClass="Normal" size="40" />
      </td>
      <td>
      </td>
     </tr>
    </table>
   </td>
  </tr>
  <tr>
   <td colspan="4">
    <dnn:sectionhead id="secLinkedFiles" runat="server" cssclass="SubHead" isExpanded="false" includerule="True" resourcekey="secLinkedFiles" section="tblLinkedFiles">
    </dnn:sectionhead>
    <table id="tblLinkedFiles" width="100%" runat="server">
     <tr>
      <td width="100%">
       <asp:DataGrid ID="dgLinkedFiles" runat="server" Width="100%" AutoGenerateColumns="False" BorderStyle="Solid" ShowFooter="False" GridLines="Horizontal">
        <Columns>
         <asp:TemplateColumn HeaderText="Filename" ItemStyle-HorizontalAlign="Left" ItemStyle-CssClass="normal" HeaderStyle-CssClass="SubHead">
          <ItemTemplate>
           <asp:Label ID="lblFileName" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem") %>'>
           </asp:Label>
          </ItemTemplate>
         </asp:TemplateColumn>
         <asp:TemplateColumn ItemStyle-HorizontalAlign="Right">
          <ItemTemplate>
           <asp:ImageButton ID="lnkDeleteFile" runat="server" ResourceKey="lnkDeleteFile.AlternateText" AlternateText="Delete File" OnCommand="lnkDeleteFile_Command" CommandName='<%# DataBinder.Eval(Container, "DataItem") %>'>
           </asp:ImageButton>
          </ItemTemplate>
         </asp:TemplateColumn>
        </Columns>
       </asp:DataGrid>
      </td>
     </tr>
    </table>
   </td>
  </tr>
 </asp:Panel>
 <tr>
  <td colspan="4">
   <dnn:sectionhead id="secEntryOptions" runat="server" cssclass="SubHead" isExpanded="true" includerule="True" resourcekey="secEntryOptions" section="tblEntryOptions">
   </dnn:sectionhead>
   <table id="tblEntryOptions" width="100%" runat="server">
    <tr>
     <td>
      <asp:CheckBox ID="chkAllowComments" CssClass="Normal" ResourceKey="chkAllowComments" runat="server" />
      <br />
      <asp:CheckBox ID="chkDisplayCopyright" CssClass="Normal" ResourceKey="chkDisplayCopyright" runat="server" AutoPostBack="True" />
      <asp:Panel ID="pnlCopyright" runat="server" Visible="False">
       <asp:Label ID="lblCopyright" runat="server" CssClass="Normal" ResourceKey="lblCopyright" />
       <asp:TextBox ID="txtCopyright" runat="server" CssClass="NormalTextBox" Width="280px" />
      </asp:Panel>
     </td>
    </tr>
    <tr id="trTrackingUrl" runat="server">
     <td>
      <asp:Label ID="lblTrackbackUrl" runat="server" ResourceKey="lblTrackbackUrl" CssClass="Normal" />
      <asp:TextBox ID="txtTrackBackUrl" runat="server" CssClass="NormalTextBox" Width="100%" />
     </td>
    </tr>
   </table>
  </td>
 </tr>
</table>
<asp:LinkButton ID="cmdDraft" runat="server" CssClass="CommandButton" BorderStyle="None" />&nbsp;
<asp:LinkButton ID="cmdPublish" runat="server" CssClass="CommandButton" BorderStyle="None" />
&nbsp;
<asp:LinkButton ID="cmdCancel" ResourceKey="cmdCancel" runat="server" CssClass="CommandButton" BorderStyle="None" CausesValidation="False" />
&nbsp;
<asp:LinkButton ID="cmdDelete" ResourceKey="cmdDelete" runat="server" CssClass="CommandButton" BorderStyle="None" CausesValidation="False" Visible="False" />
