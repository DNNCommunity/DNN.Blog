<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewBlog" %>
<div>
 <asp:Panel ID="pnlBlogInfo" Visible="False" runat="server">
  <table class="BlogInfo" cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
    <td class="BlogDescriptionHeavy" style="white-space: nowrap; text-align: right; width: 20px;">
     <asp:Label ID="lblAuthorHeader" runat="server" ResourceKey="lblAuthorHeader" />
    </td>
    <td style="white-space: nowrap;">
     <asp:Label ID="lblAuthor" runat="server" CssClass="BlogDescription" />
    </td>
    <td class="BlogDescriptionHeavy" style="white-space: nowrap; text-align: right; width: 20px;">
     <asp:Label ID="lblCreatedHeader" runat="server" ResourceKey="lblCreatedHeader" />
    </td>
    <td style="white-space: nowrap;">
     <asp:Label ID="lblCreated" runat="server" CssClass="BlogDescription" />
    </td>
    <td align="right">
     <asp:HyperLink ID="lnkRSS" runat="server" Visible="False" Target="_blank">
      <asp:Image ID="Image1" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" AlternateText="RssIcon" />
     </asp:HyperLink>
    </td>
   </tr>
   <tr>
    <td colspan="5">
     <div class="BlogDescription">
      <asp:Literal ID="litBlogDescription" runat="server" />
     </div>
    </td>
   </tr>
  </table>
 </asp:Panel>
 <asp:Panel ID="pnlBlogRss" runat="server" Visible="False">
  <table class="BlogInfo" cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
    <td align="right">
     <asp:HyperLink ID="lnkRecentRss" runat="server" Target="_blank">
      <asp:Image ID="lnkRecentRssIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-24x24.gif" AlternateText="RssIcon" />
     </asp:HyperLink>
    </td>
   </tr>
  </table>
 </asp:Panel>
 <asp:Label ID="InfoEntry" ResourceKey="lblInfoEntry" CssClass="NormalBold" runat="server" />
 <asp:DataList ID="lstBlogView" runat="server" Width="100%">
  <ItemTemplate>
   <div class="BlogBody">
    <!-- Begin Blog Entry Title -->
    <div class="BlogHead">
     <h2 class="BlogTitle">
      <asp:HyperLink ID="lnkEntry" runat="server">
							<%# DataBinder.Eval(Container.DataItem, "Title") %>
      </asp:HyperLink>
     </h2>
    </div>
    <asp:Label ID="lblUserName" runat="server" Visible="false" CssClass="BlogDateline" />
    <asp:Label ID="lblPublishDate" runat="server" CssClass="BlogDateline" />
    <div style="padding-top: 1em">
     <asp:Label ID="lblPublished" runat="server" Visible="False" CssClass="NormalRed" ResourceKey="lblPublished" />
     <asp:Literal ID="litDescription" runat="server" />
     <div class="BlogReadMore">
      <asp:HyperLink ID="lnkReadMore" runat="server" ResourceKey="lnkReadMore" CssClass="BlogMoreLink" />
     </div>
    </div>
    <div class="BlogFooter">
     <div class="BlogFooterRight">
      <asp:LinkButton ID="lnkComments" runat="server" CommandName="Comments" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EntryID") %>'
       CssClass="BlogCommentsNormal"><%= getLnkComment() %> (<%# DataBinder.Eval(Container.DataItem, "CommentCount") %>)</asp:LinkButton>
      <asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="BlogEditLink" runat="server" />
     </div>
     <div class="BlogFooterLeft">
      <span class="BlogTopics">
       <asp:HyperLink ID="lnkParentBlog" runat="server" />
       <asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/DesktopModules/Blog/images/folder_closed.gif" AlternateText="Parent Separator" />
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
     <td class="SubHead" style="white-space: nowrap;">
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
      <asp:Image ID="imgBlogParentSeparatorSearch" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" AlternateText="Parent Separator" />
      <asp:HyperLink ID="lnkChildBlogSearch" runat="server" CssClass="CommandButton" Visible="False" />
     </td>
     <td class="SubHead" align="right">
      <asp:Label runat="server" ID="lblHits" resourcekey="lblHits" />
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
      <asp:LinkButton ID="lnkMoreSearch" runat="server" CommandName="Entry" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EntryID") %>' CssClass="CommandButton" ResourceKey="lnkMoreSearch />
     </td>
    </tr>
   </table>
   <hr />
  </ItemTemplate>
 </asp:DataList>
</div>
