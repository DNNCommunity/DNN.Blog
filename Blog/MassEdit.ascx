<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MassEdit.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MassEdit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:Repeater ID="rptEdit" runat="server">
 <HeaderTemplate>
  <table>
   <tr>
    <th width="35%">
     <asp:Label runat="server" ID="lblTitle" resourcekey="lblTitle" />
    </th>
    <th width="25%">
     <asp:Label runat="server" ID="lblCategory" resourcekey="lblCategory" />
    </th>
    <th width="25%">
     <asp:Label runat="server" ID="lblTags" resourcekey="lblTags" />
    </th>
    <th>
     <asp:Label runat="server" ID="lblPub" resourcekey="lblPub" />
    </th>
    <th>
     <asp:Label runat="server" ID="lblCmts" resourcekey="lblCmts" />
    </th>
   </tr>
 </HeaderTemplate>
 <ItemTemplate>
  <tr>
   <td>
    <asp:TextBox ID="tbTitle" runat="server" Text='<%# Eval("Title") %>' />
    <asp:Literal ID="litTitle" runat="server" Text='<%# Eval("Title") %>' />
   </td>
   <td>
    <asp:Literal ID="litCat" runat="server" />
    <asp:DropDownList ID="ddlCat" runat="server" DataValueField="CatID" DataTextField="FullCat" />
   </td>
   <td>
    <asp:TextBox ID="tbTags" runat="server" />
    <asp:Literal ID="litTags" runat="server" />
   </td>
   <td>
    <asp:CheckBox ID="cbPublished" runat="server" Checked='<%# Eval("Published") %>' />
   </td>
   <td>
    <asp:CheckBox ID="cbComments" runat="server" Checked='<%# Eval("AllowComments") %>' />
   </td>
   <td>
    <asp:LinkButton ID="btnEditCmd" Visible="true" CommandName="Edit" runat="server" Text="Edit" resourcekey="btnEditCmd" />
    <asp:LinkButton ID="btnSaveCmd" Visible="false" CommandName="Save" runat="server" CommandArgument='<%# Eval("EntryID") %>' Text="Save" resourcekey="btnSaveCmd" />
    <asp:LinkButton ID="btnCancelCmd" Visible="false" CommandName="Cancel" runat="server" text="Cancel" resourcekey="btnCancelCmd" />
   </td>
  </tr>
 </ItemTemplate>
 <FooterTemplate>
  </table>
 </FooterTemplate>
</asp:Repeater>
<dnn:PagingControl id="Pagecontrol" runat="server" />
<p style="margin-top: 30px;">
	<dnn:commandbutton class="CommandButton" id="btnBack" resourcekey="btnBack" runat="server" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
