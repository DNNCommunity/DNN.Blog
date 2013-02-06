<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BlogList.ascx.vb" Inherits="DotNetNuke.Modules.Blog.BlogList" %>
<h2>Blog List Control</h2>
<div class="dnnBlogList dnnClear">
    <asp:DataList ID="lstBlogs" ShowFooter="False" ExtractTemplateRows="True" Width="100%" runat="server">
     <HeaderTemplate>
      <asp:Table ID="tblHeader" CellPadding="0" CellSpacing="0" BorderWidth="0" runat="server">
       <asp:TableRow runat="server" ID="trHeader">
        <asp:TableCell runat="server" ID="tdHeaderIcon" Width="10">
         <asp:HyperLink runat="server" ID="lnkBlogLink" Font-Bold="True">
          <asp:Image ID="lnkBlogLinkIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_item.gif" AlternateText="FolderIcon" />
         </asp:HyperLink>
        </asp:TableCell>
        <asp:TableCell runat="server" ID="tdHeaderLink" ColumnSpan="2">
         <asp:HyperLink CssClass="CommandButton" ID="lnkBlog" runat="server" ResourceKey="lnkBlog" Text="	View All Recent Entries " />
        </asp:TableCell>
       </asp:TableRow>
      </asp:Table>
     </HeaderTemplate>
     <FooterTemplate>
      <asp:Table ID="Table1" CellPadding="0" CellSpacing="0" BorderWidth="0" runat="server">
       <asp:TableRow runat="server" ID="Tablerow1">
        <asp:TableCell runat="server" ID="tdFooter" ColumnSpan="2" CssClass="Normal">
         <asp:Label ID="lblFooter" runat="server" CssClass="Normal" />
        </asp:TableCell>
       </asp:TableRow>
      </asp:Table>
     </FooterTemplate>
     <ItemTemplate>
      <asp:Table ID="tblBlogList" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="0">
       <asp:TableRow runat="server" ID="trBlogList">
        <asp:TableCell runat="server" ID="tdIcon" Width="10">
         <asp:HyperLink runat="server" ID="lnkBlogLink" Font-Bold="True">
          <asp:Image ID="lnkBlogLinkIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/tree_item.gif" AlternateText="FolderIcon" />
         </asp:HyperLink>
        </asp:TableCell>
        <asp:TableCell runat="server" ID="tdBlogName">
         <asp:HyperLink CssClass="CommandButton" ID="lnkBlog" runat="server" Font-Bold="True" />
        </asp:TableCell>
        <asp:TableCell runat="server" ID="tdBlogRSS" HorizontalAlign="Right">
         <asp:HyperLink runat="server" ID="lnkBlogRSS" Visible="False" Target="_blank">
          <asp:Image ID="lnkBlogRSSIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" AlternateText="RSSIcon" />
         </asp:HyperLink>
        </asp:TableCell>
       </asp:TableRow>
       <asp:TableRow runat="server" ID="trBlogChildren" Visible="False">
        <asp:TableCell runat="server" ID="Tablecell2" Width="10">
         <asp:Image ImageUrl="~/images/spacer.gif" runat="server" Width="8" Height="8" ID="Image1" AlternateText="spacer" />
        </asp:TableCell>
        <asp:TableCell runat="server" ID="tdBlogChildren" ColumnSpan="2">
         <asp:DataList ID="lstBlogChildren" runat="server" Width="100%" ExtractTemplateRows="True">
          <ItemTemplate>
           <asp:Table ID="tblChildBlogs" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="0">
            <asp:TableRow runat="server" ID="trChildBlogs">
             <asp:TableCell runat="server" ID="tdChildIcon" Width="10">
              <asp:HyperLink runat="server" ID="lnkChildIcon">
               <asp:Image ID="imgChildIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/tree_item.gif" AlternateText="FolderIcon" />
              </asp:HyperLink>
             </asp:TableCell>
             <asp:TableCell runat="server" ID="tdChildBlog">
              <asp:HyperLink CssClass="CommandButton" ID="lnkChildBlog" runat="server" />
             </asp:TableCell>
             <asp:TableCell runat="server" ID="tdChildBlogRSS" HorizontalAlign="Right">
              <asp:HyperLink runat="server" ID="lnkChildBlogRSS" Visible="False" Target="_blank">
               <asp:Image ID="lnkChildBlogRSSIcon" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" runat="server" AlternateText="RssIcon" />
              </asp:HyperLink>
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
    <ul style="display:none;">
        <asp:Repeater ID="rptBlogs" runat="server">
            <ItemTemplate>
                <li></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>