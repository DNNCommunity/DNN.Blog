<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewBlog" %>
<div>
 <asp:Panel ID="pnlBlogInfo" Visible="False" runat="server">
  <table class="BlogInfo" cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
    <td class="blog_Description_Heavy" nowrap align="right" width="20">
     <asp:Label ID="lblAuthorHeader" runat="server" ResourceKey="lblAuthorHeader" />
    </td>
    <td nowrap>
     <asp:Label ID="lblAuthor" runat="server" CssClass="blog_Description" />
    </td>
    <td class="blog_Description_Heavy" nowrap align="right" width="20">
     <asp:Label ID="lblCreatedHeader" runat="server" ResourceKey="lblCreatedHeader" />
    </td>
    <td nowrap>
     <asp:Label ID="lblCreated" runat="server" CssClass="blog_Description" />
    </td>
    <td align="right">
     <asp:HyperLink ID="lnkRSS" runat="server" Visible="False" Target="_blank" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" />
    </td>
   </tr>
   <tr>
    <td colspan="5">
     <asp:Label ID="lblBlogDescription" CssClass="blog_Description" runat="server" />
    </td>
   </tr>
  </table>
 </asp:Panel>
 <asp:Panel ID="pnlBlogRss" runat="server" Visible="False">
  <table class="BlogInfo" cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
    <td align="right">
     <asp:HyperLink ID="lnkRecentRss" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" Target="_blank" />
    </td>
   </tr>
  </table>
 </asp:Panel>
 <asp:Label ID="InfoEntry" ResourceKey="lblInfoEntry" CssClass="NormalBold" runat="server" />
 <asp:DataList ID="lstBlogView" runat="server" Width="100%">
  <ItemTemplate>
   <div class="blog_body">
    <!-- Begin Blog Entry Title -->
    <div class="blog_head">
     <h2 class="blog_title">
      <asp:HyperLink ID="lnkEntry" runat="server">
							<%# DataBinder.Eval(Container.DataItem, "Title") %>
      </asp:HyperLink>
     </h2>
    </div>
    <asp:Label ID="lblUserName" runat="server" Visible="false" CssClass="blog_dateline" />
    <asp:Label ID="lblPublishDate" runat="server" CssClass="blog_dateline" />
    <p>
     <asp:Label ID="lblPublished" runat="server" Visible="False" CssClass="NormalRed" ResourceKey="lblPublished">
						<p>This entry has not been published.</p>
     </asp:Label>
     <asp:Label runat="server" ID="lblDescription" />
     <asp:HyperLink ID="lnkReadMore" runat="server" ResourceKey="lnkReadMore" CssClass="blog_more_link" />
    </p>
    <div class="blog_footer">
     <div class="blog_footer_right">
      <asp:LinkButton ID="lnkComments" runat="server" CommandName="Comments" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EntryID") %>'
       CssClass="blog_comments_normal"><%= getLnkComment() %> (<%# DataBinder.Eval(Container.DataItem, "CommentCount") %>)</asp:LinkButton>
      <asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="blog_edit_link" runat="server" />
     </div>
     <div class="blog_footer_left">
      <span class="blog_topics">
       <asp:HyperLink ID="lnkParentBlog" runat="server" />
       <asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/DesktopModules/Blog/images/folder_closed.gif" />
       <asp:HyperLink ID="lnkChildBlog" runat="server" />
      </span>
     </div>
    </div>
   </div>
  </ItemTemplate>
 </asp:DataList>
 <asp:DataList ID="lstSearchResults" Visible="False" runat="server" Width="100%">
  <ItemTemplate>
   <table border="0" cellpadding="1" cellspacing="0" width="100%">
    <tr>
     <td class="SubHead" nowrap="nowrap">
      <asp:HyperLink CssClass="SubHead" ID="lnkEntryTitle" runat="server">
							<%# Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "EntryTitle")) %>
      </asp:HyperLink>
     </td>
     <td align="right">
      <asp:Label runat="server" ID="lblEntryUserName" CssClass="Normal" />
      <asp:Label runat="server" ID="lblEntryDate" CssClass="Normal" />
     </td>
    </tr>
    <tr>
     <td class="Normal">
      <asp:HyperLink ID="lnkParentBlogSearch" runat="server" CssClass="CommandButton" />
      <asp:Image ID="imgBlogParentSeparatorSearch" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" />
      <asp:HyperLink ID="lnkChildBlogSearch" runat="server" CssClass="CommandButton" Visible="False" />
     </td>
     <td class="SubHead" align="right">
      Hits:
      <%# DataBinder.Eval(Container.DataItem, "Rank") %>
     </td>
    </tr>
    <tr>
     <td colspan="2" class="Normal">
      <asp:Label ID="lblItemSummary" runat="server" />
     </td>
    </tr>
    <tr>
     <td class="Normal" align="right" colspan="2">
      <asp:LinkButton ID="lnkMoreSearch" runat="server" CommandName="Entry" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EntryID") %>'
       CssClass="CommandButton" ResourceKey="lnkMoreSearch"> More... </asp:LinkButton>
     </td>
    </tr>
   </table>
   <hr />
  </ItemTemplate>
 </asp:DataList>
</div>
