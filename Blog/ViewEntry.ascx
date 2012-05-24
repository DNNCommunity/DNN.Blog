<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dba" Assembly="DotNetNuke.Modules.Blog" Namespace="DotNetNuke.Modules.Blog" %>
<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewEntry.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewEntry" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<asp:Label ID="lblTrackback" runat="server" />
<div class="dnnForm dnnViewEntry dnnClear">
	<h2 id="lblBlogTitle" runat="server" />
	<div class="vbHeader dnnClear">
		<%= Localization.GetString("by", LocalResourceFile)%><asp:HyperLink ID="hlAuthor" runat="server" /><%= Localization.GetString("on", LocalResourceFile)%><asp:Label ID="lblDateTime" CssClass="BlogDate" runat="server" />
		<div class="dnnRight">
			<asp:Literal ID="litSocialSharing" runat="server" />
		</div>
	</div>
	<div class="vbBody dnnClear">
		<asp:Literal ID="litSummary" runat="server" />
		<asp:Literal ID="litEntry" runat="server" />
		<p><asp:Label ID="lblCopyright" CssClass="BlogCopyright" runat="server" Visible="False" /></p>
	</div>
	<div class="vbFooter dnnClear">
		<div class="dnnLeft">
			<div class="BlogCategories">
				<label><%= Localization.GetString("lblCategories", LocalResourceFile)%></label>
				<asp:HyperLink ID="lnkBlogs" runat="server" Text="Blogs" />
				<asp:Image ID="imgBlogParentSeparator" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" AlternateText="Parent Separator" />
				<asp:HyperLink ID="lnkParentBlog" runat="server" />
				<asp:Image ID="imgParentChildSeparator" runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif" Visible="False" AlternateText="Child Separator" />
				<asp:HyperLink ID="lnkChildBlog" runat="server" Visible="False" />
			</div>
			<div class="BlogCategories">
				<asp:Literal ID="litCategories" runat="server" />
			</div>
			<div class="tags dnnClear BlogTopics">
				<div class="dnnLeft">
					<asp:Repeater ID="rptTags" runat="server" OnItemDataBound="RptTagsItemDataBound">
						<ItemTemplate>
							<dba:Tags ID="dbaSingleTag" runat="server" EnableViewState="false" />
						</ItemTemplate>
					</asp:Repeater>
				</div>
			</div> 
		</div>  
		<div class="dnnRight">
			<asp:HyperLink ID="lnkTrackBack" ResourceKey="lnkTrackBack" CssClass="BlogTrackback" runat="server" />
			<asp:LinkButton ID="cmdPrint" runat="server" CausesValidation="False" CssClass="BlogPrint" resourcekey="cmdPrint" />
			<asp:HyperLink ID="lnkEditEntry" ResourceKey="msgEditEntry" CssClass="BlogEditLink" runat="server" />
		</div>
	</div>
	<div class="vbAuthor dnnClear">
		<div class="dnnLeft">
			<asp:HyperLink id="imgAuthorLink" runat="server"><dnnweb:DnnBinaryImage ID="dbiUser" runat="server" Width="40" /></asp:HyperLink>
		</div>
		<div class="dnnLeft">
			<asp:HyperLink ID="hlAuthorBio" runat="server" />
			<div class="dnnLeft">
				<asp:Literal ID="litBio" runat="server" />
			</div>
		</div>
	</div>
	<asp:Panel ID="pnlComments" runat="server" Visible="False">
		<a id="Comments" name="Comments"></a>
		<h3 class="BlogComments"><a id="linkAddComment" href="#AddComment"><asp:Label ID="lblComments" runat="server" /></a></h3>
		<asp:DataList ID="lstComments" runat="server" Width="100%">
		<ItemTemplate>
			<asp:Panel ID="divBlogBubble" runat="server" CssClass="BlogBubble">
				<blockquote>
					<div class="dnnLeft">
						<asp:HyperLink ID="hlUser" runat="server"><asp:Image ID="imgUser" runat="server" /></asp:HyperLink>
					</div>
					<div class="dnnLeft">
						<p>
						<asp:ImageButton ID="lnkEditComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="EditComment" ImageUrl="~/images/edit.gif" />
						<asp:LinkButton ID="btEditComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="EditComment" resourcekey="cmdEdit" />
						<asp:ImageButton ID="lnkApproveComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="ApproveComment" ImageUrl="~/desktopmodules/Blog/images/blog_accept.png" CausesValidation="false" />
						<asp:LinkButton ID="btApproveComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="ApproveComment" resourcekey="Approve" CssClass="CommandButton" CausesValidation="false" />
						<asp:ImageButton ID="lnkDeleteComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="DeleteComment" ImageUrl="~/images/delete.gif" CausesValidation="false" CssClass="dnnBlogCommentDelete" />
						<asp:LinkButton ID="btDeleteComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="DeleteComment" resourcekey="Delete" CssClass="dnnBlogCommentDelete"  CausesValidation="false" />
						</p>
						<p><%# server.htmldecode(DataBinder.Eval(Container.DataItem,"Comment")) %></p>
					</div>
				</blockquote>
				<cite>
					<asp:HyperLink ID="hlCommentAuthor" runat="server" />&nbsp;<asp:Label ID="lblCommentDate" runat="server" />
				</cite>
			</asp:Panel>
		</ItemTemplate>
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
		<ul class="dnnActions" id="ulAddComment">
			<li><asp:Literal ID="litAddComment" runat="server" /></li>
		</ul>
		<asp:TextBox ID="txtClientIP" runat="server" Visible="false" />
	</asp:Panel>
</div>
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setupDnnQuestions() {
			$('.qaTooltip').qaTooltip();

			$("#CommentForm").hide();
			$("#linkAdd").click(function () {
				$("#CommentForm").show('highlight', '', 200, '');
				$("#linkAdd").hide();
				$("#ulAddComment").hide();
				return true;
			});
			$("#linkAddComment").click(function () {
				$("#CommentForm").show('highlight', '', 200, '');
				$("#linkAdd").hide();
				$("#ulAddComment").hide();
				return true;
			});

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

//            $("#linkAdd").click(function () {
//                $("#CommentForm").show('highlight', '', 500, '');
//                $("#linkAdd").hide();
//                return false;
//            });

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