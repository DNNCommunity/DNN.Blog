<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditCategories.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditCategories" %>

<table style="width: 100%;">
    <tr>
        <td>
            <asp:Label ID="lblAddEditCategory" runat="server" Text="Category Management" 
                CssClass="SubHead" ResourceKey="lblAddEditCategory"></asp:Label>
            <br />
            <br />
            <asp:Label ID="lblCategoryMgmtDescription" runat="server" 
                Text="Category Mgmt description" ResourceKey="lblCategoryMgmtDescription"></asp:Label>
            <br />
            <br />
                        <asp:Label ID="lblCategoryName" runat="server" Text="Category Name" 
                            CssClass="SubHead"></asp:Label>
                    <br />
            <asp:Label ID="lblCatNameDescription" runat="server" 
                Text="Category Name Description" ResourceKey="lblCatNameDescription"></asp:Label>
            <br />
                        <asp:TextBox ID="tbCategory" runat="server" Width="350px"></asp:TextBox>
                    <br />
                    <br />
                        <asp:Label ID="lblParentCategory" runat="server" Text="Parent Category" 
                            CssClass="SubHead"></asp:Label>
                    <br />
            <asp:Label ID="lblCatParentDescription" runat="server" 
                Text="Category Parent Description" ResourceKey="lblCatParentDescription"></asp:Label>
                    <br />
                        <asp:DropDownList ID="ddlCategory" runat="server" Width="350px"  
                            DataValueField="CatID" DataTextField="FullCat">
                                    </asp:DropDownList>
                    <br />
                    <br />
            <asp:LinkButton ID="btnAddEdit" runat="server">Add</asp:LinkButton>
&nbsp;<asp:LinkButton ID="btnDelete" runat="server" ResourceKey="btnDelete">Delete</asp:LinkButton>
&nbsp;<asp:LinkButton ID="btnCancel" runat="server" ResourceKey="btnCancel">Cancel</asp:LinkButton>
        </td>
        <td valign="top">
            <asp:Label ID="lblPortalCategories" runat="server" CssClass="SubHead" 
                Text="Portal Categories" ResourceKey="lblPortalCategories"></asp:Label>
            <br />
            <br />
            <asp:TreeView ID="tvCategories" runat="server" ShowExpandCollapse="False" ShowLines="False"></asp:TreeView>
        </td>
    </tr>
    </table>
