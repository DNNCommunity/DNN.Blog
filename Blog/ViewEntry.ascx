<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewEntry.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewEntry" %>
<asp:Label ID="lblTrackback" runat="server" />
<div class="dnnForm dnnViewEntry dnnClear">
    <div class="BlogHead">
        <h2 class="dnnFormSectionHead" id="lblBlogTitle" runat="server" />
    </div>
    <acronym class="BlogPublished" title="<%= lblDateTime.Text %>">
        <span class="BlogPubMonth">
            <asp:Label ID="lblEntryMonth" runat="server" />
        </span>
        <span class="BlogPubDate">
            <asp:Label ID="lblEntryDay" runat="server" />
        </span>
    </acronym>
 <p class="BlogSubHead">
  <span class="blog_author">
   <asp:Label ID="lblPostedBy" ResourceKey="lblPostedBy" runat="server" />
   <asp:Label ID="lblUserID" runat="server" />
  </span>
  <br />
  <asp:Label ID="lblDateTime" CssClass="BlogDate" runat="server" />&nbsp;
  <asp:HyperLink ID="lnkRss" runat="server" Target="_blank" resourcekey="lnkRss">
   <asp:Image ID="lnkRssIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif"
    AlternateText="RssIcon" />
  </asp:HyperLink>
 </p>
 <!-- End Blog Sub Head -->
 <div class="HorizontalLine"></div>
 <!-- Begin Blog Entry -->
 <div class="BlogEntryDescription">
  <asp:Literal ID="litSummary" runat="server" />
 </div>
 <asp:Literal ID="litEntry" runat="server" />
 <p>
  <asp:Label ID="lblCopyright" CssClass="BlogCopyright" runat="server" Visible="False" />
 </p>
 <!-- End Blog Entry -->
 <!-- Blog Entry Footer Section -->
 <div class="dnnClear">
 <div class="dnnRight">
        <asp:HyperLink ID="lnkTrackBack" ResourceKey="lnkTrackBack" CssClass="BlogTrackback" runat="server" />
        <asp:LinkButton ID="cmdPrint" runat="server" CausesValidation="False" CssClass="BlogPrint" resourcekey="cmdPrint" />
        <asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="BlogEditLink" runat="server" />
    </div>
  <div class="dnnLeft">
   <div class="tags dnnClear BlogTopics">
       <div class="dnnLeft">
            <asp:Repeater ID="rptTags" runat="server" OnItemDataBound="RptTagsItemDataBound">
                <ItemTemplate>
                    <dba:Tags ID="dbaSingleTag" runat="server" EnableViewState="false" />
                </ItemTemplate>
            </asp:Repeater>
       </div>
   </div>
   <div class="BlogFooterSub BlogCategories">
    <asp:Label ID="lblCategories" runat="server" ResourceKey="lblCategories" />
    <asp:Repeater ID="rptCategories" runat="server">
     <ItemTemplate>
      <asp:HyperLink runat="server" ID="lnkTags" Text='<%# Eval("Category") %>' NavigateUrl='<%# DotNetNuke.Common.NavigateURL(TabId, "", "catid=" & Eval("CatID")) %>'>HyperLink</asp:HyperLink></ItemTemplate>
     <SeparatorTemplate>, </SeparatorTemplate>
    </asp:Repeater>
    <asp:HyperLink ID="lnkBlogs" runat="server" Text="Blogs" />
    <asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" AlternateText="Parent Separator" />
    <asp:HyperLink ID="lnkParentBlog" runat="server" />
    <asp:Image ID="imgParentChildSeparator" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" AlternateText="Child Separator" />
    <asp:HyperLink ID="lnkChildBlog" runat="server" Visible="False" />
   </div>
  </div>  
 </div>
 <div id="ShareBadgePRO_Toolbar" style="display:none;"></div>
 <div class="dnnRight">
    <asp:Literal ID="litSocialSharing" runat="server" />
 </div>

 <!-- Comments Section -->
 <asp:Panel ID="pnlComments" runat="server" Visible="False">
   <a id="Comments" name="Comments"></a>
   <h3 class="BlogComments"><a href="#AddComment"><asp:Label ID="lblComments" runat="server" /></a></h3>
  <asp:ImageButton ID="lnkDeleteAllUnapproved" runat="server" ImageUrl="~/images/delete.gif" Visible="false" CausesValidation="false" AlternateText="Delete Unaproved" />
  <asp:LinkButton ID="btDeleteAllUnapproved" runat="server" Visible="false" resourcekey="DeleteAllUnapproved" CssClass="CommandButton" CausesValidation="false" /><br />
  <asp:DataList ID="lstComments" runat="server" Width="100%">
   <ItemTemplate>
    <asp:Panel ID="divBlogBubble" runat="server" CssClass="BlogBubble">
     <blockquote>
      <asp:Panel ID="divBlogGravatar" runat="server" CssClass="BlogGravatar">
       <asp:Image runat="server" Width="48" ID="imgGravatar" AlternateText="Gravatar" />
      </asp:Panel>
      <p>
       <asp:ImageButton ID="lnkEditComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="EditComment" ImageUrl="~/images/edit.gif" AlternateText="Edit Comment" />
       <asp:LinkButton ID="btEditComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="EditComment" resourcekey="cmdEdit" CssClass="CommandButton" />
       <asp:ImageButton ID="lnkApproveComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="ApproveComment" ImageUrl="~/desktopmodules/Blog/images/blog_accept.png" CausesValidation="false" AlternateText="Approve Comment" />
       <asp:LinkButton ID="btApproveComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="ApproveComment" resourcekey="Approve" CssClass="CommandButton" CausesValidation="false" />
       <asp:ImageButton ID="lnkDeleteComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="DeleteComment" ImageUrl="~/images/delete.gif" CausesValidation="false" AlternateText="Delete Comment" />
       <asp:LinkButton ID="btDeleteComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="DeleteComment" resourcekey="Delete" CssClass="CommandButton" CausesValidation="false" />
       <asp:Label ID="lblTitle" runat="server" CssClass="NormalBold" />
      </p>
      <p>
       <%# server.htmldecode(DataBinder.Eval(Container.DataItem,"Comment")) %>
      </p>
     </blockquote>
     <cite>
      <asp:Label ID="lblUserName" CssClass="NormalBold" runat="server" Text="Label" Visible="true" />
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
  <a id="AddComment" name="AddComment"></a>
  <asp:ValidationSummary ID="valSummary" runat="server" CssClass="NormalRed" Enabled="False" />
  <asp:RequiredFieldValidator ID="valCommentAuthor" runat="server" ResourceKey="valCommentAuthor.ErrorMessage" CssClass="NormalRed" Enabled="False" ErrorMessage="Author is required" ControlToValidate="txtAuthor" Display="None" EnableClientScript="False" />
  <asp:RequiredFieldValidator ID="valCommentTitle" runat="server" ResourceKey="valCommentTitle.ErrorMessage" CssClass="NormalRed" Enabled="False" ErrorMessage="Title is required" ControlToValidate="txtCommentTitle" Display="None" EnableClientScript="False" />
  <asp:RequiredFieldValidator ID="valComment" runat="server" ResourceKey="valComment.ErrorMessage" CssClass="NormalRed" Enabled="False" ErrorMessage="Comment is required" ControlToValidate="txtComment" Display="None" EnableClientScript="False" />
  <div>
    <fieldset>
      <table cellspacing="1" cellpadding="1" width="100%" border="0">
       <tr>
        <td class="BlogLeftTD" width="1%">
         <asp:Label ID="lblAuthor" runat="server" ResourceKey="lblAuthor" CssClass="NormalBold" Width="80px" />
        </td>
        <td id="tdAuthor" valign="top" runat="server">
         <asp:TextBox ID="txtAuthor" TabIndex="1" runat="server" Width="99%" />
        </td>
        <td id="tdGravatarPreview" valign="top" align="right" width="1%" rowspan="2" runat="server">
         <div class="BlogGravatarPreview">
          <asp:Image ID="imgGravatarPreview" runat="server" AlternateText="Gravatar Preview" />
         </div>
        </td>
       </tr>
       <tr id="trGravatarEmail" runat="server">
        <td class="BlogLeftTD" width="1%">
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
         <asp:Label ID="lblEmailExplanation" runat="server" ResourceKey="lblEmailExplanation" CssClass="Normal" />
        </td>
        <td>
        </td>
       </tr>
       <tr id="trCommentWebsite" runat="server">
        <td class="BlogLeftTD" width="1%">
         <asp:Label ID="lblWebsite" runat="server" ResourceKey="lblWebsite" CssClass="NormalBold" Width="80px" />
        </td>
        <td colspan="2">
         <asp:TextBox ID="txtWebsite" TabIndex="3" runat="server" Width="99%" />
        </td>
       </tr>
       <tr id="trCommentTitle" runat="server">
        <td class="BlogLeftTD" width="1%">
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
         <asp:TextBox ID="txtComment" TabIndex="5" runat="server" Width="99%" TextMode="MultiLine" Rows="8" />
        </td>
       </tr>
       <tr id="rowCaptcha" runat="server">
        <td colspan="3">
         <asp:Label ID="lblCaptcha" runat="server" ResourceKey="lblCaptcha" CssClass="NormalBold" Width="80px" />
         <dnn:CaptchaControl id="ctlCaptcha" tabIndex="6" runat="server" cssclass="Normal" errorstyle-cssclass="NormalRed" captchawidth="130" captchaheight="40" />
        </td>
       </tr>
       <tr>
        <td colspan="3">
            <ul class="dnnActions">
                <li><asp:LinkButton ID="cmdAddComment" TabIndex="7" runat="server" CssClass="dnnPrimaryAction" /></li>
                 <li><asp:LinkButton ID="cmdCancel" TabIndex="8" runat="server" ResourceKey="cmdCancel" CssClass="dnnSecondaryAction" CausesValidation="False" /></li>
                 <li><asp:LinkButton ID="cmdDeleteComment" TabIndex="9" runat="server" ResourceKey="cmdDelete" CssClass="dnnSecondaryAction" Visible="False" /></li>
            </ul>
        </td>
       </tr>
      </table>
      </fieldset>
  </div>
  <asp:TextBox ID="txtClientIP" runat="server" Visible="false" />
 </asp:Panel>
</div>
<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupDnnQuestions() {
            $('.qaTooltip').qaTooltip();

            var po = document.createElement('script');
            po.type = 'text/javascript';
            po.async = true;
            po.src = 'https://apis.google.com/js/plusone.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);

            !function (d, s, id) {
                var js, fjs = d.getElementsByTagName(s)[0];
                if (!d.getElementById(id)) {
                    js = d.createElement(s); js.id = id; js.src = "//platform.twitter.com/widgets.js";
                    fjs.parentNode.insertBefore(js, fjs);
                }
            } (document, "script", "twitter-wjs");

            !function (d, s, id) {
                var js, fjs = d.getElementsByTagName(s)[0];
                if (d.getElementById(id)) return;
                js = d.createElement(s); js.id = id;
                js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
                fjs.parentNode.insertBefore(js, fjs);
            } (document, 'script', 'facebook-jssdk');
        }

        $(document).ready(function () {
            setupDnnQuestions();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupDnnQuestions();

                twttr.widgets.load();
                FB.XFBML.parse();
                IN.parse();
            });
        });

    } (jQuery, window.Sys));
</script>  