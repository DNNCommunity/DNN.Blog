<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BlogImport.ascx.vb" Inherits="DotNetNuke.Modules.Blog.BlogImport" %>
<asp:DataList ID="lstBlogs" runat="server" Width="100%" ExtractTemplateRows="True" ShowFooter="False">
 <HeaderTemplate>
  <asp:Table ID="tblHeader" CellPadding="0" CellSpacing="0" BorderWidth="0" runat="server" Visible="True">
   <asp:TableRow ID="trHeader" runat="server">
    <asp:TableCell ID="tdCategory" runat="server" Width="150">
     <asp:Label ID="lblHeaderCategory" runat="server" CssClass="SubHead" ResourceKey="lblHeaderCategory" Visible="True" />
    </asp:TableCell>
    <asp:TableCell>
     <asp:Label ID="lblMappeTo" runat="server" CssClass="SubHead" ResourceKey="lblMappeTo" Visible="True" />
    </asp:TableCell>
    <asp:TableCell HorizontalAlign="Right">
     <asp:Label ID="lblImport" runat="server" CssClass="SubHead" ResourceKey="lblImport" />
    </asp:TableCell>
   </asp:TableRow>
   <asp:TableRow>
    <asp:TableCell ColumnSpan="3">
					<hr />
    </asp:TableCell>
   </asp:TableRow>
  </asp:Table>
 </HeaderTemplate>
 <ItemTemplate>
  <asp:Table ID="tblBlogList" CellPadding="0" CellSpacing="0" BorderWidth="0" runat="server">
   <asp:TableRow runat="server" ID="trBlogList">
    <asp:TableCell runat="server" ID="tdIcon" Width="250">
     <asp:Label ID="lblCategoryID" runat="server" Visible="False" CssClass="Normal" />
     <asp:Label ID="lblCategory" runat="server" Visible="True" CssClass="Normal" Text="Category:" />
    </asp:TableCell>
    <asp:TableCell>
     <asp:DropDownList ID="ddlBlogs" runat="server" />
    </asp:TableCell>
    <asp:TableCell HorizontalAlign="Right">
     <asp:CheckBox ID="chkImport" runat="server" CssClass="Normal" ResourceKey="chkImport" Text=" " />
    </asp:TableCell>
   </asp:TableRow>
   <asp:TableRow runat="server" ID="trForumList">
    <asp:TableCell ID="tdForumList" runat="server" ColumnSpan="3">
     <asp:DataList ID="lstForum" runat="server" Width="100%" ExtractTemplateRows="True">
      <HeaderTemplate>
       <asp:Table ID="Table1" CellPadding="0" CellSpacing="0" BorderWidth="0" runat="server">
        <asp:TableRow ID="Tablerow1" runat="server">
         <asp:TableCell ID="Tablecell1" runat="server">
          <asp:Label ID="lblHeaderForum" runat="server" Visible="True" CssClass="SubHead" ResourceKey="lblHeaderForum" />
         </asp:TableCell>
        </asp:TableRow>
       </asp:Table>
      </HeaderTemplate>
      <FooterTemplate>
       <asp:Table ID="tblFooter" runat="server" Width="100%">
        <asp:TableRow>
         <asp:TableCell ColumnSpan="2">
										<hr />
         </asp:TableCell>
        </asp:TableRow>
       </asp:Table>
      </FooterTemplate>
      <ItemTemplate>
       <asp:Table ID="tblForumsList" CellPadding="0" CellSpacing="0" BorderWidth="0" runat="server" Width="100%">
        <asp:TableRow ID="trForums" runat="server">
         <asp:TableCell ID="tdForums" runat="server">
          <asp:Label ID="lblForum" runat="server" Visible="True" CssClass="Normal" />
         </asp:TableCell>
        </asp:TableRow>
       </asp:Table>
      </ItemTemplate>
     </asp:DataList>
    </asp:TableCell>
   </asp:TableRow>
  </asp:Table>
 </ItemTemplate>
</asp:DataList>
<asp:LinkButton ID="cmdImport" ResourceKey="cmdImport" runat="server" CssClass="CommandButton" BorderStyle="None" OnClick="cmdImport_Click" Style="height: 19px" Text="Import" />&nbsp;
<asp:LinkButton ID="cmdCancel" ResourceKey="cmdCancel" runat="server" CssClass="CommandButton" BorderStyle="None" CausesValidation="False" Text="Cancel" />&nbsp; 