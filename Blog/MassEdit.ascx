<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MassEdit.ascx.vb" Inherits="DotNetNuke.Modules.Blog.MassEdit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:Repeater ID="rptEdit" runat="server">
 <HeaderTemplate>
  <table>
   <tr>
    <th width="35%">
     Title
    </th>
    <th width="25%">
     Category
    </th>
    <th width="25%">
     Tags
    </th>
    <th>
     Pub
    </th>
    <th>
     Cmts
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
    <asp:LinkButton ID="btnEditCmd" Visible="true" CommandName="Edit" runat="server" Text="Edit" />
    <asp:LinkButton ID="btnSaveCmd" Visible="false" CommandName="Save" runat="server" CommandArgument='<%# Eval("EntryID") %>' Text="Save" />
    <asp:LinkButton ID="btnCancelCmd" Visible="false" CommandName="Cancel" runat="server" text="Cancel"/>
   </td>
  </tr>
 </ItemTemplate>
 <FooterTemplate>
  </table>
 </FooterTemplate>
</asp:Repeater>
<dnn:PagingControl id="Pagecontrol" runat="server" />
