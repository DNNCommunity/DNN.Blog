<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EditBlog.ascx.vb" Inherits="DotNetNuke.Modules.Blog.EditBlog" %>
<asp:ValidationSummary ID="valSummary" EnableClientScript="True" runat="server" CssClass="NormalRed">
</asp:ValidationSummary>
<asp:RequiredFieldValidator ID="valTitle" runat="server" Display="None" ErrorMessage="Title is Required"
    ControlToValidate="txtTitle" resourcekey="valTitle.ErrorMessage"></asp:RequiredFieldValidator>
<asp:RequiredFieldValidator ID="valTitleDescription" runat="server" Display="None"
    ErrorMessage="Title Description is Required" resourcekey="valTitleDescription.ErrorMessage"
    ControlToValidate="txtDescription"></asp:RequiredFieldValidator>
<table class="Normal" id="Table2" cellspacing="1" cellpadding="1" width="500" border="0">
    <tr>
        <td>
            <asp:Label ID="lblOptions" CssClass="SubHead" runat="server" ResourceKey="lblOptions">Blog Options:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblOptionsDescription" runat="server" ResourceKey="lblOptionsDescription">These options control the main blog features.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTitle" CssClass="SubHead" runat="server" ResourceKey="lblTitle">Title:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTitleDescription" runat="server" ResourceKey="lblTitleDescription">This is the display title for your blog. It will display at the top of your 
			entry list and in the blog directory.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:TextBox ID="txtTitle" CssClass="NormalTextBox" runat="server" Width="100%" ResourceKey="txtTitle"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblDescription" CssClass="SubHead" runat="server" ResourceKey="lblDescription">Description:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblDescriptionDescription" runat="server" ResourceKey="lblDescriptionDescription">This is a brief summary description of your blog. It's a good place to describe 
			your intentions with your blog and what information readers can expect.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:TextBox ID="txtDescription" CssClass="NormalTextBox" runat="server" Width="100%"
                Rows="5" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:CheckBox ID="chkPublic" CssClass="Normal" runat="server" Text="Make this blog public"
                ResourceKey="chkPublic"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblUserIdentity" runat="server" ResourceKey="lblUserIdentity">When displaying your identity use:</asp:Label><br />
            <asp:RadioButtonList ID="rdoUserName" CssClass="Normal" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="False" Selected="True" ResourceKey="rdoUserName_UserName">User Name</asp:ListItem>
                <asp:ListItem Value="True" ResourceKey="rdoUserName_FullName">Full Name</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblMetaWeblogOptions" CssClass="SubHead" runat="server" ResourceKey="lblMetaWeblogOptions">MetaWeblog Options:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblMetaWeblogOptionsDescription" runat="server" ResourceKey="lblMetaWeblogOptionsDescription">Use the following URL to connect to your blog using a MetaWeblog enabled client such as Windows Live Writer or Word 2007.  Change the tabid parameter as needed if this blog appears on a different tab.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <br />
            <asp:Label ID="lblMetaWeblogUrl" runat="server">http://www.yourdomain.com/desktopmodules/blog/blogpost.ashx</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTwitterIntegration" CssClass="SubHead" runat="server" ResourceKey="lblTwitterIntegration">Twitter Integration:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:CheckBox ID="chkEnableTwitterIntegration" CssClass="Normal" runat="server" Text="Enable Twitter Integration"
                ResourceKey="chkEnableTwitterIntegration" AutoPostBack="true"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTwitterUsername" runat="server" Text="Twitter Username:" ResourceKey="lblTwitterUsername"></asp:Label><br />
            <asp:TextBox class="NormalTextBox" ID="txtTwitterUsername" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTwitterPassword" runat="server" Text="Password:" ResourceKey="lblTwitterPassword"></asp:Label><br />
            <asp:TextBox class="NormalTextBox" ID="txtTwitterPassword" runat="server" Width="300px"
                TextMode="Password"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTweetTemplate" runat="server" Text="Tweet Template:" ResourceKey="lblTweetTemplate"></asp:Label><br />
            <asp:TextBox class="NormalTextBox" ID="txtTweetTemplate" runat="server" Width="300px"
                Height="61px" TextMode="MultiLine"></asp:TextBox>
            <br />
            <i><b>Available Tokens:</b></i> {title}, {url}
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblCommentOptions" CssClass="SubHead" runat="server" ResourceKey="lblCommentOptions">Comment Options:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblCommentOptionsDescription" runat="server" ResourceKey="lblCommentOptionsDescription">These options control the comment related settings.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <p>
                <asp:Label ID="lblUsersComments" runat="server" ResourceKey="lblUsersComments">Comments by registered users:</asp:Label>
                <asp:RadioButtonList ID="rdoUsersComments" runat="server" RepeatDirection="Horizontal"
                    CssClass="Normal">
                    <asp:ListItem Value="Allow" ResourceKey="rdoComments_Allow" Selected="True">Allow</asp:ListItem>
                    <asp:ListItem Value="RequireApproval" ResourceKey="rdoComments_RequireApproval">Require approval</asp:ListItem>
                    <asp:ListItem Value="Disallow" ResourceKey="rdoComments_Disallow">Disallow</asp:ListItem>
                </asp:RadioButtonList>
                <asp:Label ID="lblAnonymousComments" runat="server" ResourceKey="lblAnonymousComments">Comments by unauthenticated users:</asp:Label>
                <asp:RadioButtonList ID="rdoAnonymousComments" runat="server" RepeatDirection="Horizontal"
                    CssClass="Normal">
                    <asp:ListItem Value="Allow" ResourceKey="rdoComments_Allow" Selected="True">Allow</asp:ListItem>
                    <asp:ListItem Value="RequireApproval" ResourceKey="rdoComments_RequireApproval">Require approval</asp:ListItem>
                    <asp:ListItem Value="Disallow" ResourceKey="rdoComments_Disallow">Disallow</asp:ListItem>
                </asp:RadioButtonList>
                <asp:Label ID="lblTrackbacks" runat="server" ResourceKey="lblTrackbacks">Trackback comments:</asp:Label>
                <asp:RadioButtonList ID="rdoTrackbacks" runat="server" RepeatDirection="Horizontal"
                    CssClass="Normal">
                    <asp:ListItem Value="Allow" ResourceKey="rdoComments_Allow" Selected="True">Allow</asp:ListItem>
                    <asp:ListItem Value="RequireApproval" ResourceKey="rdoComments_RequireApproval">Require approval</asp:ListItem>
                    <asp:ListItem Value="Disallow" ResourceKey="rdoComments_Disallow">Disallow</asp:ListItem>
                </asp:RadioButtonList>
                <asp:CheckBox ID="chkEmailNotification" CssClass="Normal" runat="server" Text="Send mail notification after comments are posted"
                    ResourceKey="chkEmailNotification"></asp:CheckBox><br />
                <asp:CheckBox ID="chkCaptcha" CssClass="Normal" runat="server" Text="Use CAPTCHA for comments"
                    ResourceKey="chkCaptcha"></asp:CheckBox><br />
            </p>
            <p>
            </p>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTrackbackOptions" CssClass="SubHead" runat="server" ResourceKey="lblTrackbackOptions">Trackback Options:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblTrackbackOptionsDescription" runat="server" ResourceKey="lblTrackbackOptionsDescription">These options control the Trackback related settings.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <p>
                <asp:CheckBox ID="chkAutoTrackbacks" CssClass="Normal" runat="server" Text="Auto Discovery (Client Mode)"
                    ResourceKey="chkAutoTrackbacks"></asp:CheckBox></p>
            <p>
            </p>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblSyndicationOptions" CssClass="SubHead" runat="server" ResourceKey="lblSyndicationOptions">Syndication Options:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <p>
                <asp:CheckBox ID="chkSyndicate" CssClass="Normal" runat="server" Text="Syndicate this blog."
                    ResourceKey="chkSyndicate"></asp:CheckBox><br />
                <asp:CheckBox ID="chkSyndicateIndependant" CssClass="Normal" runat="server" Text="Syndicate independantly<br />(If not checked it will be syndicated as a category of the parent blog)"
                    ResourceKey="chkSyndicateIndependant"></asp:CheckBox></p>
            <p class="Normal">
                <asp:Label ID="lblSyndicationEmail" runat="server" ResourceKey="lblSyndicationEmail">Use this email for the "ManagingEditor" rss field:</asp:Label><br />
                <asp:TextBox class="NormalTextBox" ID="txtSyndicationEmail" runat="server" Width="300px"></asp:TextBox></p>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblDateTime" CssClass="SubHead" runat="server" ResourceKey="lblDateTime">Date and Time Options:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblDateTimeDescription" runat="server" ResourceKey="lblDateTimeDescription">These options control how date and time are displayed within your blog. This 
			setting effects all categories and entries within your blog.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <table id="Table3" cellspacing="1" cellpadding="1" width="100%" border="0">
                <tr>
                    <td height="20">
                        <asp:Label ID="lblTimeZone" CssClass="SubHead" runat="server" ResourceKey="lblTimeZone">Time Zone:</asp:Label>
                    </td>
                    <td height="20">
                        <asp:DropDownList ID="cboTimeZone" Width="400px" CssClass="NormalTextBox" runat="server"
                            AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblCulture" CssClass="SubHead" runat="server" ResourceKey="lblCulture">Culture:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cboCulture" Width="400px" CssClass="NormalTextBox" runat="server"
                            AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDateFormat" CssClass="SubHead" runat="server" ResourceKey="lblDateFormat">Date Format:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cboDateFormat" Width="400px" CssClass="NormalTextBox" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblRegenerate" CssClass="SubHead" runat="server" ResourceKey="lblRegenerate">Regenerate Blog PermaLinks:</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblRegenerateDescription" runat="server" ResourceKey="lblRegenerateDescription">Regenerate all blog permalinks.  This link can be used after the Friendly URLs setting has been changed for the site.</asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:LinkButton ID="cmdGenerateLinks" runat="server" CausesValidation="False" BorderStyle="none"
                Text="Regenerate Blog Permalinks" ResourceKey="cmdGenerateLinks" CssClass="CommandButton"></asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td>
            <hr />
            <br />
        </td>
    </tr>
    <asp:Panel ID="pnlChildBlogs" runat="server">
        <tr>
            <td>
                <asp:Label ID="lblChildBlogs" ResourceKey="lblChildBlogs" runat="server" CssClass="SubHead">Child Blogs:</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblChildBlogsDescription" ResourceKey="lblChildBlogsDescription" runat="server">If you would like to break your blog up into different categories, this is 
				where you define them. Having&nbsp;Child-Blogs allows you to create 
				sub-blogs&nbsp;within your blog. Each one has it's own options for publication 
				and can syndicated separately from your root blog.</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table id="Table1" cellspacing="1" cellpadding="0" width="100%" border="0">
                    <tr>
                        <td width="100%" rowspan="3">
                            <asp:ListBox ID="lstChildBlogs" CssClass="NormalTextBox" runat="server" Width="100%"
                                Rows="5"></asp:ListBox>
                        </td>
                        <td width="40">
                            <asp:Button ID="btnAddChildBlog" CssClass="Normal" runat="server" Enabled="False"
                                Text="Add" Width="70px" resourceKey="cmdAdd"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td width="40">
                            <asp:Button ID="btnEditChildBlog" CssClass="Normal" runat="server" Enabled="False"
                                Text="Edit" Width="70px" resourceKey="cmdEdit"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td width="40">
                            <asp:Button ID="btnDeleteChildBlog" CssClass="Normal" runat="server" Enabled="False"
                                Text="Delete" Width="70px" resourceKey="cmdDelete"></asp:Button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <hr />
                <br />
            </td>
        </tr>
    </asp:Panel>
</table>
<asp:LinkButton ID="cmdUpdate" CssClass="CommandButton" runat="server" BorderStyle="None"
    ResourceKey="cmdUpdate">Update</asp:LinkButton>&nbsp;
<asp:LinkButton ID="cmdCancel" CssClass="CommandButton" runat="server" CausesValidation="False"
    BorderStyle="None" ResourceKey="cmdCancel">Cancel</asp:LinkButton>&nbsp;
<asp:LinkButton ID="cmdDelete" CssClass="CommandButton" runat="server" CausesValidation="False"
    BorderStyle="None" Visible="False" ResourceKey="cmdDelete">Delete</asp:LinkButton>
