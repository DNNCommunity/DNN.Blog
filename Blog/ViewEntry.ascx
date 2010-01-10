<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewEntry.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewEntry" %>
<asp:Label ID="lblTrackback" runat="server" />
<div class="blog_body">
 <!-- Begin Blog Entry Title -->
 <div class="blog_head">
  <h2 class="blog_title">
   <asp:HyperLink ID="lblBlogTitle" runat="server" />
  </h2>
 </div>
 <!-- End Blog Entry Title -->
 <!-- Begin Blog Sub Head -->
 <acronym class="blog_published" title="<%= lblDateTime.Text %>"><span class="blog_pub-month">
  <asp:Label ID="lblEntryMonth" runat="server" />
 </span><span class="blog_pub-date">
  <asp:Label ID="lblEntryDay" runat="server" />
 </span></acronym>
 <p class="blog_subhead">
  <span class="blog_author">
   <asp:Label ID="lblPostedBy" ResourceKey="lblPostedBy" runat="server" />
   <asp:Label ID="lblUserID" runat="server" />
  </span>
  <br />
  <asp:Label ID="lblDateTime" CssClass="blog_date" runat="server" />&nbsp;
  <asp:HyperLink ID="lnkRss" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" Target="_blank" resourcekey="lnkRss" />
 </p>
 <!-- End Blog Sub Head -->
 <div class="horizontalline">
 </div>
 <!-- Begin Blog Entry -->
 <div class="blog_entry_description">
  <asp:Label ID="lblSummary" runat="server" /></div>
 <asp:Label ID="lblEntry" runat="server" />
 <p>
  <asp:Label ID="lblCopyright" CssClass="blog_copyright" runat="server" Visible="False" />
 </p>
 <!-- End Blog Entry -->
 <!-- Blog Entry Footer Section -->
 <div class="blog_footer">
  <div class="blog_footer_right">
   <asp:HyperLink ID="lnkTrackBack" ResourceKey="lnkTrackBack" CssClass="blog_trackback" runat="server" />
   <asp:LinkButton ID="cmdPrint" runat="server" CausesValidation="False" CssClass="blog_print" resourcekey="cmdPrint" />
   <asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="blog_edit_link" runat="server" />
  </div>
  <div class="blog_footer_left">
   <span class="blog_tag">
    <asp:Label ID="lblTags" runat="server" ResourceKey="lblTags" />
    <asp:Repeater ID="rptTags" runat="server">
     <ItemTemplate>
      <asp:HyperLink runat="server" ID="lnkTags" Text='<%# Eval("Tag") %>' NavigateUrl='<%# DotNetNuke.Modules.Blog.Business.Utility.GetSEOLink(PortalId, TabId, "", Eval("Slug"), "tagid=" & Eval("TagID")) %>'>HyperLink</asp:HyperLink></ItemTemplate>
     <SeparatorTemplate>
      ,
     </SeparatorTemplate>
    </asp:Repeater>
   </span>
   <br />
   <span class="blog_tag">
    <asp:Label ID="lblCategories" runat="server" ResourceKey="lblCategories" />
    <asp:Repeater ID="rptCategories" runat="server">
     <ItemTemplate>
      <asp:HyperLink runat="server" ID="lnkTags" Text='<%# Eval("Category") %>' NavigateUrl='<%# DotNetNuke.Modules.Blog.Business.Utility.GetSEOLink(PortalId, TabId, "", Eval("Slug"), "catid=" & Eval("CatID")) %>'>HyperLink</asp:HyperLink></ItemTemplate>
     <SeparatorTemplate>
      ,
     </SeparatorTemplate>
    </asp:Repeater>
   </span><span class="blog_topics">
    <asp:Label ID="lblLocation" runat="server" ResourceKey="lblLocation" />
    <asp:HyperLink ID="lnkBlogs" runat="server" Text="Blogs" />
    <asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" />
    <asp:HyperLink ID="lnkParentBlog" runat="server" />
    <asp:Image ID="imgParentChildSeparator" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" />
    <asp:HyperLink ID="lnkChildBlog" runat="server" Visible="False" />
   </span>
  </div>
 </div>
 <div id="ShareBadgePRO_Toolbar">
 </div>
 <div class="clear">
 </div>
 <!-- Comments Section -->
 <asp:Panel ID="pnlComments" runat="server" Visible="False">
  <p>
   <a id="Comments" name="Comments" /><a href="#AddComment">
    <asp:Label ID="lblComments" runat="server" CssClass="blog_comments" /></a></p>
  <asp:ImageButton ID="lnkDeleteAllUnapproved" runat="server" ImageUrl="~/images/delete.gif" Visible="false" ImageAlign="AbsMiddle" CausesValidation="false" />
  <asp:LinkButton ID="btDeleteAllUnapproved" runat="server" Visible="false" resourcekey="DeleteAllUnapproved" CssClass="CommandButton" CausesValidation="false" /><br />
  <asp:DataList ID="lstComments" runat="server" Width="100%">
   <ItemTemplate>
    <asp:Panel ID="divBlogBubble" runat="server" CssClass="blog_bubble">
     <blockquote>
      <asp:Panel ID="divBlogGravatar" runat="server" CssClass="blog_gravatar">
       <asp:Image runat="server" Width="48" ID="imgGravatar" />
      </asp:Panel>
      <p>
       <asp:ImageButton ID="lnkEditComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
        CommandName="EditComment" ImageUrl="~/images/edit.gif" ImageAlign="AbsMiddle"></asp:ImageButton>
       <asp:LinkButton ID="btEditComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
        CommandName="EditComment" resourcekey="cmdEdit" CssClass="CommandButton">Edit</asp:LinkButton>
       <asp:ImageButton ID="lnkApproveComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
        CommandName="ApproveComment" ImageUrl="~/desktopmodules/Blog/images/blog_accept.png"
        ImageAlign="AbsMiddle" CausesValidation="false"></asp:ImageButton>
       <asp:LinkButton ID="btApproveComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
        CommandName="ApproveComment" resourcekey="Approve" CssClass="CommandButton" CausesValidation="false">Approve</asp:LinkButton>
       <asp:ImageButton ID="lnkDeleteComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
        CommandName="DeleteComment" ImageUrl="~/images/delete.gif" ImageAlign="AbsMiddle"
        CausesValidation="false"></asp:ImageButton>
       <asp:LinkButton ID="btDeleteComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
        CommandName="DeleteComment" resourcekey="Delete" CssClass="CommandButton" CausesValidation="false">Delete</asp:LinkButton>
       <asp:Label ID="lblTitle" runat="server" CssClass="NormalBold" />
      </p>
      <p>
       <%# server.htmldecode(DataBinder.Eval(Container.DataItem,"Comment")) %></p>
     </blockquote>
     <cite>
      <asp:HyperLink ID="lnkUserName" CssClass="NormalBold" runat="server" Visible="true" />
      &nbsp;
      <asp:Label ID="lblCommentDate" runat="server" CssClass="Normal" />
     </cite>
    </asp:Panel>
   </ItemTemplate>
  </asp:DataList>
 </asp:Panel>
 <asp:Panel ID="pnlLogin" runat="server" Visible="false">
  <asp:LinkButton ID="cmdLogin" TabIndex="7" runat="server" CssClass="CommandButton" BorderStyle="None" ResourceKey="cmdLogin" />
 </asp:Panel>
 <asp:Panel ID="pnlAddComment" runat="server">
  <a id="AddComment" name="AddComment" />
  <asp:ValidationSummary ID="valSummary" runat="server" CssClass="NormalRed" Enabled="False" />
  <asp:RequiredFieldValidator ID="valCommentAuthor" runat="server" ResourceKey="valCommentAuthor.ErrorMessage" CssClass="NormalRed" Enabled="False" ErrorMessage="Author is required" ControlToValidate="txtAuthor" Display="None" EnableClientScript="False" />
  <asp:RequiredFieldValidator ID="valCommentTitle" runat="server" ResourceKey="valCommentTitle.ErrorMessage" CssClass="NormalRed" Enabled="False" ErrorMessage="Title is required" ControlToValidate="txtCommentTitle" Display="None" EnableClientScript="False" />
  <asp:RequiredFieldValidator ID="valComment" runat="server" ResourceKey="valComment.ErrorMessage" CssClass="NormalRed" Enabled="False" ErrorMessage="Comment is required" ControlToValidate="txtComment" Display="None" EnableClientScript="False" />
  <br />
  <table cellspacing="1" cellpadding="1" width="100%" border="0">
   <tr>
    <td class="blog_lefttd" width="1%">
     <asp:Label ID="lblAuthor" runat="server" ResourceKey="lblAuthor" CssClass="NormalBold" Width="80px" />
    </td>
    <td id="tdAuthor" valign="top" runat="server">
     <asp:TextBox ID="txtAuthor" TabIndex="1" runat="server" Width="99%" />
    </td>
    <td id="tdGravatarPreview" valign="top" align="right" width="1%" rowspan="2" runat="server">
     <div class="blog_gravatar_preview">
      <asp:Image ID="imgGravatarPreview" runat="server" /></div>
    </td>
   </tr>
   <tr id="trGravatarEmail" runat="server">
    <td class="blog_lefttd" width="1%">
     <asp:Label ID="lblEmail" runat="server" ResourceKey="lblEmail" CssClass="NormalBold" />
    </td>
    <td valign="top">
     <asp:TextBox ID="txtEmail" TabIndex="2" runat="server" Width="99%" />
    </td>
   </tr>
   <tr id="trUseGravatar" runat="server">
    <td>
    </td>
    <td>
     <asp:Label ID="lblEmailExplanation" runat="server" ResourceKey="lblEmailExplanation" CssClass="Normal">(Optional) Used only to display <a href="http://www.gravatar.com">
							Gravatar</a></asp:Label>
    </td>
    <td>
    </td>
   </tr>
   <tr id="trCommentWebsite" runat="server">
    <td class="blog_lefttd" width="1%">
     <asp:Label ID="lblWebsite" runat="server" ResourceKey="lblWebsite" CssClass="NormalBold" Width="80px" />
    </td>
    <td colspan="2">
     <asp:TextBox ID="txtWebsite" TabIndex="3" runat="server" Width="99%" />
    </td>
   </tr>
   <tr id="trCommentTitle" runat="server">
    <td class="blog_lefttd" width="1%">
     <asp:Label ID="lblCommentTitle" runat="server" ResourceKey="lblCommentTitle" CssClass="NormalBold" />
    </td>
    <td colspan="2">
     <asp:TextBox ID="txtCommentTitle" TabIndex="4" runat="server" Width="99%" />
    </td>
   </tr>
   <tr>
    <td colspan="3">
     <asp:Label ID="lblComment" runat="server" ResourceKey="lblComment" CssClass="NormalBold" />
    </td>
   </tr>
   <tr>
    <td colspan="3">
     <asp:TextBox ID="txtComment" TabIndex="5" runat="server" CssClass="NormalTextBox" Width="99%" TextMode="MultiLine" Rows="8" />
    </td>
   </tr>
   <tr id="rowCaptcha" runat="server">
    <td colspan="3">
     <asp:Label ID="lblCaptcha" runat="server" ResourceKey="lblCaptcha" CssClass="NormalBold" Width="80px" />
     <dnn:CaptchaControl id="ctlCaptcha" tabIndex="6" runat="server" cssclass="Normal" errorstyle-cssclass="NormalRed" captchawidth="130" captchaheight="40">
     </dnn:CaptchaControl>
    </td>
   </tr>
   <tr>
    <td colspan="3">
     <asp:LinkButton ID="cmdAddComment" TabIndex="7" runat="server" CssClass="CommandButton" BorderStyle="None" />&nbsp;&nbsp;
     <asp:LinkButton ID="cmdCancel" TabIndex="8" runat="server" ResourceKey="cmdCancel" CssClass="CommandButton" BorderStyle="None" CausesValidation="False" />&nbsp;
     <asp:LinkButton ID="cmdDeleteComment" TabIndex="9" runat="server" ResourceKey="cmdDelete" CssClass="CommandButton" Visible="False" BorderStyle="None" />
    </td>
   </tr>
  </table>
  <asp:TextBox ID="txtClientIP" runat="server" Visible="false" />
 </asp:Panel>
</div>
