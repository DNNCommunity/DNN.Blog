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
     <a href="<%# DotNetNuke.Common.Globals.UserProfileURL(DataBinder.Eval(Container.DataItem, "CreatedByUserID")) %>" title="<%# DataBinder.Eval(Container.DataItem, "DisplayName") %>">
      <img src="<%# IIF(DataBinder.Eval(Container.DataItem, "CreatedByUserID")=-1, ResolveUrl("~/images/spacer.gif"), DotNetNuke.Common.Globals.UserProfilePicFormattedUrl.Replace("{0}", DataBinder.Eval(Container.DataItem, "CreatedByUserID")).replace("{1}", "50").replace("{2}", "50")) %>" alt="<%# DataBinder.Eval(Container.DataItem, "DisplayName") %>" width="50" height="50" />
     </a>
				</div>
				<div class="commentContent">
					<div class="ccAuthor">
      <a href="<%# DotNetNuke.Common.Globals.UserProfileURL(DataBinder.Eval(Container.DataItem, "CreatedByUserID")) %>"><%# IIf(DataBinder.Eval(Container.DataItem, "CreatedByUserID") = -1, LocalizeString("Anonymous"), DataBinder.Eval(Container.DataItem, "DisplayName"))%>
      </a>
      &nbsp;
      <abbr title="<%# CDATE(DataBinder.Eval(Container.DataItem, "CreatedOnDate")).ToString("u") %>" class="commenttimeago"></abbr>
     </div>
					<div>
      <%# System.Text.RegularExpressions.Regex.Replace(DataBinder.Eval(Container.DataItem, "Comment"), "(?<!["">])((http|https|ftp)\://.+?)(?=\s|$)", "<a rel=""nofollow"" href=""$1"">$1</a>").Replace(System.Environment.NewLine, "<br />")%>
     </div>
					<div class="commentMod dnnRight">
      <asp:Button ID="lnkEditComment" runat="server" Visible='<%# Security.CanApproveComment.ToString() %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="EditComment" Text="&#9998;" CssClass="icon16 entypoButton" />
      <a href="#" onclick="blogService.approveComment(<%=BlogID%>, <%# DataBinder.Eval(Container.DataItem, "CommentID") %>, function() {$('#cmdApproveComment<%# DataBinder.Eval(Container.DataItem, "CommentID") %>').hide()});return false;" 
         id="cmdApproveComment<%# DataBinder.Eval(Container.DataItem, "CommentID") %>"
         class="icon16 entypoButton approveComment" 
         title="<%= LocalizeString("Approve") %>"
         style="display:<%# IIF(NOT CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Comments.CommentInfo).Approved AND Security.CanApproveComment, "inline", "none") %>">&#128077;</a>
      <a href="#" onclick="if (confirm('<%= LocalizeString("DeleteComment") %>')) {blogService.deleteComment(<%=BlogID%>, <%# DataBinder.Eval(Container.DataItem, "CommentID") %>, function() {$('#commentDiv<%# DataBinder.Eval(Container.DataItem, "CommentID") %>').hide()})};return false;" 
         class="icon16 entypoButton deleteComment" 
         title="<%= LocalizeString("Delete") %>"
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
				<li><asp:LinkButton ID="cmdAddComment" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdAddComment" /></li>
				<li><asp:LinkButton ID="cmdDeleteComment" runat="server" ResourceKey="cmdDeleteComment" CssClass="dnnSecondaryAction" Visible="False" /></li>
			</ul>
		</fieldset>
	</div>
	<asp:TextBox ID="txtClientIP" runat="server" Visible="false" />
</asp:Panel>

<div id="blogServiceErrorBox">
</div>

<script language="javascript" type="text/javascript">
 jQuery(function ($) {
  $("abbr.commenttimeago").timeago();
 } (jQuery, window.Sys));
</script>
