<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Comments.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.Comments" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
	<asp:Panel ID="pnlComments" runat="server">
		<a id="Comments" name="Comments"></a>
		<h3 class="BlogComments"><asp:Label ID="lblComments" runat="server" /></h3>
		<asp:DataList ID="lstComments" runat="server" Width="100%">
			<ItemTemplate>
				<div class="blogComments dnnClear" id="commentDiv<%# DataBinder.Eval(Container.DataItem, "CommentID") %>">
					<div class="commentAuthor dnnLeft">
						<asp:HyperLink ID="hlUser" runat="server"><asp:Image ID="imgUser" runat="server" /></asp:HyperLink>
					</div>
					<div class="commentContent">
						<div class="ccAuthor"><asp:HyperLink ID="hlCommentAuthor" runat="server" />&nbsp;<asp:Label ID="lblCommentDate" runat="server" /></div>
						<div><p><asp:Literal runat="server" ID="txtCommentBody"></asp:Literal></p></div>
						<div class="commentMod dnnRight">
       <asp:Button ID="lnkEditComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="EditComment" Text="&#9998;" CssClass="icon16 entypoButton" />
       <a href="#" onclick="blogModule.approveComment(<%=BlogID%>, <%# DataBinder.Eval(Container.DataItem, "CommentID") %>, function() {$('#cmdApproveComment<%# DataBinder.Eval(Container.DataItem, "CommentID") %>').hide()});return false;" 
          id="cmdApproveComment<%# DataBinder.Eval(Container.DataItem, "CommentID") %>"
          class="icon16 entypoButton" 
          title="Approve"
          style="display:<%# IIF(NOT CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Comments.CommentInfo).Approved AND Security.CanApproveComment, "inline", "none") %>">&#128077;</a>
       <a href="#" onclick="blogModule.deleteComment(<%=BlogID%>, <%# DataBinder.Eval(Container.DataItem, "CommentID") %>, function() {$('#commentDiv<%# DataBinder.Eval(Container.DataItem, "CommentID") %>').hide()});return false;" 
          class="icon16 entypoButton" 
          title="Delete"
          style="display:<%# IIF(Security.CanApproveComment, "inline", "none") %>">&#59177;</a>
						</div>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate>
				<div class="blogSeparate">&nbsp;</div>
			</SeparatorTemplate>
		</asp:DataList>
	</asp:Panel>
	<asp:Panel ID="pnlAddComment" runat="server">
		<a id="AddComment" name="AddComment"></a>
		<asp:RequiredFieldValidator ID="valComment" runat="server" ResourceKey="valComment.ErrorMessage" CssClass="dnnFormError" Enabled="False" ErrorMessage="Comment is required" ControlToValidate="txtComment" Display="None" EnableClientScript="False" />
		<div id="CommentForm" class="dnnClear">
			<fieldset>
				<dl>
					<dt><label for="<%= txtComment.ClientID %>"><%= Localization.GetString("lblComment", LocalResourceFile)%></label></dt>
					<dd><asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="8" Width="100%" /></dd>
				</dl>
				<ul class="dnnActions">
					<li><asp:LinkButton ID="cmdAddComment" runat="server" CssClass="dnnPrimaryAction" /></li>
					<li><asp:LinkButton ID="cmdDeleteComment" runat="server" ResourceKey="cmdDelete" CssClass="dnnSecondaryAction" Visible="False" /></li>
				</ul>
			</fieldset>
		</div>
		<asp:TextBox ID="txtClientIP" runat="server" Visible="false" />
	</asp:Panel>
</div>
<script language="javascript" type="text/javascript">
 var blogModule
 jQuery(function ($) {
  blogModule = new BlogModule($, {
    serverErrorText: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("ServerError"))%>',
    serverErrorWithDescriptionText: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("ServerErrorWithDescription"))%>'
   },
   $.dnnSF(<%=ModuleId %>))
 });
 (function ($, Sys) {
  function setupDnnQuestions() {
   $('.dnnBlogCommentDelete').dnnConfirm({
    text: '<%= LocalizeString("DeleteItem") %>',
    yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
    noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
    title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
   });
   $('.dnnBlogDeleteAllComments').dnnConfirm({
    text: '<%= LocalizeString("msgDeleteAllUnapproved") %>',
    yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
    noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
    title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
   });
   $('.dnnBlogAddComment').dnnConfirm({
    text: '<%= LocalizeString("cmdAddCommentMessage") %>',
    yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
    noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
    title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
   });

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
