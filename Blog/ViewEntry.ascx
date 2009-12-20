<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ViewEntry.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ViewEntry" %>
<asp:Label id="lblTrackback" Runat="server"></asp:Label>
<div class="blog_body">
	<!-- Begin Blog Entry Title -->
	<div class="blog_head">
		<h2 class="blog_title">
			<asp:hyperlink id="lblBlogTitle" runat="server"></asp:hyperlink>
		</h2>
	</div>
	<!-- End Blog Entry Title -->
	<!-- Begin Blog Sub Head -->
	<acronym class="blog_published" title="<%= lblDateTime.Text %>">
		<span class="blog_pub-month">
			<asp:label id="lblEntryMonth" runat="server"></asp:label>
		</span>
		<span class="blog_pub-date">
			<asp:label id="lblEntryDay" runat="server"></asp:label>
		</span>
	</acronym>
	<p class="blog_subhead">
		<span class="blog_author">
			<asp:label id="lblPostedBy" ResourceKey="lblPostedBy" runat="server">Written By</asp:label>
			<asp:label id="lblUserID" runat="server"></asp:label>
		</span><br>
		<asp:label id="lblDateTime" cssclass="blog_date" runat="server"></asp:label>&nbsp;
		<asp:hyperlink id="lnkRss" Runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif"
			Target="_blank" resourcekey="lnkRss"></asp:hyperlink>
	</p>
	<!-- End Blog Sub Head -->
	<div class="horizontalline"></div>
	<!-- Begin Blog Entry -->
		<div class="blog_entry_description"><asp:label id="lblSummary" runat="server"></asp:label></div>
		<asp:label id="lblEntry" runat="server"></asp:label>
	<p>
		<asp:label id="lblCopyright" CssClass="blog_copyright" Runat="server" Visible="False"></asp:label>
	</p>
	<!-- End Blog Entry -->
	<!-- Blog Entry Footer Section -->
	<div class="blog_footer">
		<div class="blog_footer_right">
			<asp:hyperlink id="lnkTrackBack" ResourceKey="lnkTrackBack" cssclass="blog_trackback" runat="server">Trackback</asp:hyperlink>
			<asp:linkbutton id="cmdPrint" Runat="server" CausesValidation="False" cssclass="blog_print" resourcekey="cmdPrint"></asp:linkbutton>
			<asp:hyperlink id="lnkEditEntry" ResourceKey="msgEditEntry" cssclass="blog_edit_link" runat="server">Edit</asp:hyperlink>
		</div>
		<div class="blog_footer_left">
   <span class="blog_tag">
    <asp:label id="lblTags" Runat="server" ResourceKey="lblTags">Tags: </asp:label>
    <asp:Repeater ID="rptTags" runat="server">
     <ItemTemplate><asp:HyperLink runat="server" id="lnkTags" text='<%# Eval("Tag") %>' NavigateUrl='<%# DotNetNuke.Common.NavigateURL(TabId, "", "tagid=" & Eval("TagID")) %>'>HyperLink</asp:HyperLink></ItemTemplate>
     <SeparatorTemplate>, </SeparatorTemplate>
     </asp:Repeater>
   </span><br />
   <span class="blog_tag">
    <asp:label id="lblCategories" Runat="server" ResourceKey="lblCategories">Categories: </asp:label>
    <asp:Repeater ID="rptCategories" runat="server">
     <ItemTemplate><asp:HyperLink runat="server" id="lnkTags" text='<%# Eval("Category") %>' NavigateUrl='<%# DotNetNuke.Common.NavigateURL(TabId, "", "catid=" & Eval("CatID")) %>'>HyperLink</asp:HyperLink></ItemTemplate>
     <SeparatorTemplate>, </SeparatorTemplate>
     </asp:Repeater>
   </span>
			<span class="blog_topics">
				<asp:label id="lblLocation" Runat="server" ResourceKey="lblLocation">Location: </asp:label>
				<asp:hyperlink id="lnkBlogs" Runat="server">Blogs</asp:hyperlink>
				<asp:image id="imgBlogParentSeparator" Runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif"></asp:image>
				<asp:hyperlink id="lnkParentBlog" Runat="server"></asp:hyperlink>
				<asp:image id="imgParentChildSeparator" Runat="server" ImageUrl="~/desktopmodules/Blog/Images/folder_closed.gif"
					Visible="False"></asp:image>
				<asp:hyperlink id="lnkChildBlog" Runat="server" Visible="False"></asp:hyperlink>
			</span>
		</div>
	</div>
	<div id="ShareBadgePRO_Toolbar"></div>
	<div class="clear"></div>
	<!-- Comments Section -->
	<asp:panel id="pnlComments" runat="server" visible="False">
		<P><A id="Comments" name="Comments"></A><A href="#AddComment">
				<asp:label id="lblComments" runat="server" cssclass="blog_comments"></asp:label></A></P>
		<asp:ImageButton ID="lnkDeleteAllUnapproved" runat="server" ImageUrl="~/images/delete.gif" Visible="false"
                                ImageAlign="AbsMiddle" CausesValidation="false" />
                            <asp:LinkButton ID="btDeleteAllUnapproved" runat="server" Visible="false"
                                resourcekey="DeleteAllUnapproved" CssClass="CommandButton" CausesValidation="false">Delete all unapproved comments</asp:LinkButton><br />
		<asp:datalist id="lstComments" runat="server" width="100%">
			<ItemTemplate>
				<asp:Panel id="divBlogBubble" runat="server" CssClass="blog_bubble">
					<blockquote>
						<asp:Panel ID="divBlogGravatar" Runat="server" CssClass="blog_gravatar">
							<asp:Image runat="server" width="48" id="imgGravatar"></asp:Image>
						</asp:Panel>
						<p>
							<asp:imagebutton id=lnkEditComment runat="server" visible="false" commandargument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' commandname="EditComment" imageurl="~/images/edit.gif" ImageAlign="AbsMiddle">
							</asp:imagebutton>
							<asp:LinkButton ID=btEditComment Runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' commandname="EditComment" resourcekey="cmdEdit" CssClass="CommandButton">Edit</asp:LinkButton>
							<asp:imagebutton id=lnkApproveComment runat="server" visible="false" commandargument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' commandname="ApproveComment" imageurl="~/desktopmodules/Blog/images/blog_accept.png" ImageAlign="AbsMiddle" CausesValidation="false">
							</asp:imagebutton>
							<asp:LinkButton ID="btApproveComment" Runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' commandname="ApproveComment" resourcekey="Approve" CssClass="CommandButton" CausesValidation="false">Approve</asp:LinkButton>
							<asp:ImageButton ID="lnkDeleteComment" runat="server" Visible="false" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
                                CommandName="DeleteComment" ImageUrl="~/images/delete.gif"
                                ImageAlign="AbsMiddle" CausesValidation="false"></asp:ImageButton>
                            <asp:LinkButton ID="btDeleteComment" runat="server" Visible="False" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'
                                CommandName="DeleteComment" resourcekey="Delete" CssClass="CommandButton" CausesValidation="false">Delete</asp:LinkButton>
							<asp:label id="lblTitle" runat="server" cssclass="NormalBold"></asp:label>
						</p>
						<p><%# server.htmldecode(DataBinder.Eval(Container.DataItem,"Comment")) %></p>
					</blockquote><cite>
						<asp:Hyperlink id="lnkUserName" cssclass="NormalBold" runat="server" visible="true"></asp:Hyperlink>
						&nbsp;
						<asp:Label id="lblCommentDate" Runat="server" cssclass="Normal"></asp:Label>
					</cite>
				</asp:Panel>
			</ItemTemplate>
		</asp:datalist>
	</asp:panel>
	<asp:Panel id="pnlLogin" runat="server" Visible="false">
	    <asp:LinkButton id="cmdLogin" tabIndex="7" runat="server" cssclass="CommandButton" borderstyle="None" ResourceKey="cmdLogin"></asp:LinkButton>
	</asp:Panel>
	<asp:panel id="pnlAddComment" Runat="server">
		<A id="AddComment" name="AddComment"></A>
		<asp:ValidationSummary id="valSummary" Runat="server" CssClass="NormalRed" Enabled="False"></asp:ValidationSummary>
		<asp:RequiredFieldValidator id="valCommentAuthor" runat="server" ResourceKey="valCommentAuthor.ErrorMessage"
			CssClass="NormalRed" Enabled="False" ErrorMessage="Author is required" ControlToValidate="txtAuthor" Display="None"
			EnableClientScript="False"></asp:RequiredFieldValidator>
		<asp:RequiredFieldValidator id="valCommentTitle" runat="server" ResourceKey="valCommentTitle.ErrorMessage" CssClass="NormalRed"
			Enabled="False" ErrorMessage="Title is required" ControlToValidate="txtCommentTitle" Display="None" EnableClientScript="False"></asp:RequiredFieldValidator>
		<asp:RequiredFieldValidator id="valComment" runat="server" ResourceKey="valComment.ErrorMessage" CssClass="NormalRed"
			Enabled="False" ErrorMessage="Comment is required" ControlToValidate="txtComment" Display="None" EnableClientScript="False"></asp:RequiredFieldValidator>
		<BR>
		<TABLE cellSpacing="1" cellPadding="1" width="100%" border="0">
			<TR>
				<TD class="blog_lefttd" width="1%">
					<asp:Label id="lblAuthor" Runat="server" ResourceKey="lblAuthor" CssClass="NormalBold" Width="80px">Your name:</asp:Label></TD>
				<TD id="tdAuthor" vAlign="top" runat="server">
					<asp:TextBox id="txtAuthor" tabIndex="1" Runat="server" Width="99%"></asp:TextBox></TD>
				<TD id="tdGravatarPreview" vAlign="top" align="right" width="1%" rowSpan="2" runat="server">
					<DIV class="blog_gravatar_preview">
						<asp:Image id="imgGravatarPreview" runat="server"></asp:Image></DIV>
				</TD>
			</TR>
			<TR id="trGravatarEmail" runat="server">
				<TD class="blog_lefttd" width="1%">
					<asp:Label id="lblEmail" Runat="server" ResourceKey="lblEmail" CssClass="NormalBold">Your email:</asp:Label></TD>
				<TD vAlign="top">
					<asp:TextBox id="txtEmail" tabIndex="2" Runat="server" Width="99%"></asp:TextBox></TD>
			</TR>
			<TR id="trUseGravatar" runat="server">
				<TD></TD>
				<TD>
					<asp:Label id="lblEmailExplanation" Runat="server" ResourceKey="lblEmailExplanation" CssClass="Normal">(Optional) Used only to display <a href="http://www.gravatar.com">
							Gravatar</a></asp:Label></TD>
				<TD></TD>
			</TR>
			<TR id="trCommentWebsite" runat="server">
				<TD class="blog_lefttd" width="1%">
					<asp:Label id="lblWebsite" Runat="server" ResourceKey="lblWebsite" CssClass="NormalBold" Width="80px">Your website:</asp:Label></TD>
				<TD colSpan="2">
					<asp:TextBox id="txtWebsite" tabIndex="3" Runat="server" Width="99%"></asp:TextBox></TD>
			</TR>
			<TR id="trCommentTitle" runat="server">
				<TD class="blog_lefttd" width="1%">
					<asp:label id="lblCommentTitle" runat="server" ResourceKey="lblCommentTitle" cssclass="NormalBold">Title:</asp:label></TD>
				<TD colSpan="2">
					<asp:TextBox id="txtCommentTitle" tabIndex="4" Runat="server" Width="99%"></asp:TextBox></TD>
			</TR>
			<TR>
				<TD colSpan="3">
					<asp:Label id="lblComment" runat="server" ResourceKey="lblComment" cssclass="NormalBold">Comment:</asp:Label></TD>
			</TR>
			<TR>
				<TD colSpan="3">
					<asp:TextBox id="txtComment" tabIndex="5" runat="server" cssclass="NormalTextBox" width="99%"
						textmode="MultiLine" rows="8"></asp:TextBox></TD>
			</TR>
			<TR id="rowCaptcha" runat="server">
				<TD colSpan="3">
					<asp:Label id="lblCaptcha" Runat="server" ResourceKey="lblCaptcha" CssClass="NormalBold" Width="80px">Captcha:</asp:Label>
					<dnn:CaptchaControl id="ctlCaptcha" tabIndex="6" runat="server" cssclass="Normal" errorstyle-cssclass="NormalRed"
						captchawidth="130" captchaheight="40"></dnn:CaptchaControl></TD>
			</TR>
			<TR>
				<TD colSpan="3">
					<asp:LinkButton id="cmdAddComment" tabIndex="7" runat="server" cssclass="CommandButton" borderstyle="None"></asp:LinkButton>&nbsp;&nbsp;
					<asp:LinkButton id="cmdCancel" tabIndex="8" runat="server" ResourceKey="cmdCancel" cssclass="CommandButton"
						borderstyle="None" causesvalidation="False">Cancel</asp:LinkButton>&nbsp;
					<asp:LinkButton id="cmdDeleteComment" tabIndex="9" runat="server" ResourceKey="cmdDelete" cssclass="CommandButton"
						Visible="False" borderstyle="None">Delete</asp:LinkButton></TD>
			</TR>
		</TABLE>
		<asp:TextBox id="txtClientIP" runat="server" Visible="false"></asp:TextBox>
	</asp:panel>
</div>
