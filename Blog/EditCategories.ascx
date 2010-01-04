<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditCategories.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditCategories" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<table style="width: 100%;">
 <tr>
  <td>
   <asp:Label ID="lblAddEditCategory" runat="server" CssClass="SubHead" ResourceKey="lblAddEditCategory" />
   <br />
   <br />
   <asp:Label ID="lblCategoryMgmtDescription" runat="server" ResourceKey="lblCategoryMgmtDescription" />
   <br />
   <br />
   <asp:Label ID="lblCategoryName" runat="server" CssClass="SubHead" />
   <br />
   <asp:Label ID="lblCatNameDescription" runat="server" ResourceKey="lblCatNameDescription" />
   <br />
   <asp:TextBox ID="tbCategory" runat="server" Width="350px" />&nbsp;
   <asp:RequiredFieldValidator runat="server" ID="reqCategory" ControlToValidate="tbCategory" resourcekey="reqCategory.Error" Display="Dynamic" />
   <br />
   <br />
   <asp:Label ID="lblParentCategory" runat="server" CssClass="SubHead" />
   <br />
   <asp:Label ID="lblCatParentDescription" runat="server" ResourceKey="lblCatParentDescription" />
   <br />
   <asp:DropDownList ID="ddlCategory" runat="server" Width="350px" DataValueField="CatID" DataTextField="FullCat" />
   <br />
   <br />
   <asp:LinkButton ID="btnAddEdit" runat="server" CssClass="CommandButton" />
   &nbsp;<asp:LinkButton ID="btnDelete" runat="server" ResourceKey="btnDelete" CssClass="CommandButton" />
   &nbsp;<asp:LinkButton ID="btnCancel" runat="server" ResourceKey="btnCancel" CssClass="CommandButton" />
  </td>
  <td valign="top">
   <asp:Label ID="lblPortalCategories" runat="server" CssClass="SubHead" ResourceKey="lblPortalCategories" />
   <br />
   <br />
   <asp:TreeView ID="tvCategories" runat="server" ShowExpandCollapse="False" ShowLines="False" />
  </td>
 </tr>
</table>
<p style="margin-top: 30px;">
 <dnn:commandbutton class="CommandButton" id="btnBack" resourcekey="btnBack" runat="server" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
