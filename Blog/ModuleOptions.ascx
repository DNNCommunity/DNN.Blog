<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ModuleOptions.ascx.vb" Inherits="DotNetNuke.Modules.Blog.ModuleOptions" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<script runat="server">

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
</script>
<table cellSpacing="0" cellPadding="2" summary="ViewBlog setting design table" border="0">
	<tr>
		<td vAlign="top"><dnn:sectionhead id="secGeneralSettings" isExpanded="true" includerule="True" resourcekey="secGeneralSettings"
				section="tblGeneralSetting" text="Vertical" runat="server" cssclass="Head"></dnn:sectionhead>
			<table id="tblGeneralSetting" cellSpacing="2" cellPadding="2" width="100%" summary="Edit ViewBlog Basic Settings"
				border="0" runat="server">
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblMandatory" runat="server" controlname="chkForceDescription" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkForceDescription" runat="server" TextAlign="Left" AutoPostBack="True"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblAllowSummaryHtml" runat="server" controlname="chkAllowSummaryHtml" suffix="" /></td>
					<td><asp:checkbox id="chkAllowSummaryHtml" runat="server" TextAlign="Left" AutoPostBack="True" /></td>
				</tr>
				<tr id="trSummary" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblSummary" runat="server" controlname="txtSummaryLimit" suffix=""></dnn:label></td>
					<td><asp:textbox id="txtSummaryLimit" runat="server">1024</asp:textbox></td>
				</tr>
				<tr id="trSearchSummary" vAlign="top" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblSearchSummary" runat="server" controlname="txtSearchLimit" suffix=""></dnn:label></td>
					<td><asp:textbox id="txtSearchLimit" runat="server">255</asp:textbox></td>
				</tr>
				<tr id="Tr1" vAlign="top" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblMaxImageWidth" runat="server" controlname="txtMaxImageWidth" suffix=""></dnn:label></td>
					<td><asp:textbox id="txtMaxImageWidth" runat="server">400</asp:textbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblRecentEntriesMax" runat="server" controlname="txtRecentEntriesMax" suffix=""></dnn:label></td>
					<td><asp:textbox id="txtRecentEntriesMax" runat="server">10</asp:textbox></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblRecentRssEntriesMax" runat="server" controlname="txtRecentRssEntriesMax"
							suffix=""></dnn:label></td>
					<td><asp:textbox id="txtRecentRssEntriesMax" runat="server">10</asp:textbox></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td vAlign="top"><dnn:sectionhead id="secCommentSettings" isExpanded="True" includerule="True" resourcekey="secCommentSettings"
				section="tblCommentSettings" text="Vertical" runat="server" cssclass="Head"></dnn:sectionhead>
			<table id="tblCommentSettings" cellSpacing="2" cellPadding="2" width="100%" summary="Edit Blog Comment Settings"
				border="0" runat="server">
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblShowGravatars" runat="server" controlname="lblShowGravatars" suffix=""></dnn:label></td>
					<td>
						<asp:CheckBox ID="chkShowGravatars" runat="server" AutoPostBack="True" />
					</td>
				</tr>
				<tr id="trGravatarImageWidth" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblGravatarImageWidth" runat="server" controlname="lblGravatarImageWidth" suffix=""></dnn:label></td>
					<td><asp:textbox id="txtGravatarImageWidth" runat="server">48</asp:textbox></td>
				</tr>
				<tr id="trGravatarRating" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblGravatarRating" runat="server" controlname="lblGravatarRating" suffix=""></dnn:label></td>
					<td>
						<asp:RadioButtonList id="rblGravatarRating" runat="server">
							<asp:ListItem Value="G" Selected="True" ResourceKey="rblGravatarRating_g">G - Suitable for all audiences</asp:ListItem>
							<asp:ListItem Value="PG" ResourceKey="rblGravatarRating_pg">PG - Possibly offensive, typically suitable for audiences 13 and above</asp:ListItem>
							<asp:ListItem Value="R" ResourceKey="rblGravatarRating_r">R - Intended for audiences 17 and above</asp:ListItem>
							<asp:ListItem Value="X" ResourceKey="rblGravatarRating_x">X - Even more mature than R</asp:ListItem>
						</asp:RadioButtonList></td>
				</tr>
				<tr id="trGravatarDefaultImageUrl" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblGravatarDefaultImageUrl" runat="server" controlname="lblGravatarDefaultImageUrl"
							suffix=""></dnn:label></td>
					<td>
						<asp:RadioButtonList ID="rblDefaultImage" runat="server">
							<asp:ListItem Value="" Selected="True">Gray Man</asp:ListItem>
							<asp:ListItem Value="identicon">Identicon</asp:ListItem>
							<asp:ListItem Value="wavatar">Wavatar</asp:ListItem>
							<asp:ListItem Value="monsterid">MonsterID</asp:ListItem>
							<asp:ListItem Value="custom">Custom</asp:ListItem>
						</asp:RadioButtonList>
					</td>
				</tr>
				<tr id="trGravatarDefaultImageCustomURL" runat="server">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblGravatarDefaultImageCustomURL" runat="server" controlname="lblGravatarDefaultImageCustomURL"
							suffix=""></dnn:label></td>
					<td>
						<asp:TextBox ID="txtGravatarDefaultImageCustomURL" runat="server" Width="290px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblShowWebsite" runat="server" controlname="lblShowWebsite" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkShowWebsite" Checked="True" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblShowTitle" runat="server" controlname="lblShowTitle" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkShowCommentTitle" Checked="True" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblAllowCommentAnchors" runat="server" controlname="lblAllowCommentAnchors"
							suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkAllowCommentAnchors" Checked="True" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblAllowCommentImages" runat="server" controlname="lblAllowCommentImages" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkAllowCommentImages" Checked="True" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblAllowCommentFormatting" runat="server" controlname="lblAllowCommentFormatting"
							suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkAllowCommentFormatting" Checked="True" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><dnn:sectionhead id="secSEOSettings" isExpanded="true" includerule="True" resourcekey="tblSEOSettings"
				section="tblSEOSettings" text="Advanced Settings" runat="server" cssclass="Head"></dnn:sectionhead>
			<table id="tblSEOSettings" cellSpacing="0" cellPadding="2" width="100%" summary="Edit ViewBlog SEO Settings"
				border="0" runat="server">
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblShowUniqueTitle" runat="server" controlname="chkShowUniqueTitle" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkShowUniqueTitle" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblShowSeoFriendlyUrl" runat="server" controlname="chkShowSeoFriendlyUrl" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkShowSeoFriendlyUrl" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblRegenerateLinks" runat="server" suffix="" controlname="cmdGenerateLinks"></dnn:label></td>
					<td><asp:linkbutton id="cmdGenerateLinks" runat="server" CausesValidation="False" BorderStyle="none"
							Text="Generate Links" ResourceKey="cmdGenerateLinks" CssClass="CommandButton"></asp:linkbutton></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><dnn:sectionhead id="secRSSSettings" isExpanded="true" includerule="True" resourcekey="secRSSSettings"
				section="tblRSSSettings" text="RSS Settings" runat="server" cssclass="Head"></dnn:sectionhead>
			<table id="tblRSSSettings" cellSpacing="0" cellPadding="2" width="100%" summary="Edit Blog RSS Settings"
				border="0" runat="server">
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblIncludeBody" runat="server" controlname="chkIncludeBody" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkIncludeBody" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblIncludeCategoriesInDescription" runat="server" controlname="chkIncludeCategoriesInDescription" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkIncludeCategoriesInDescription" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblIncludeTagsInDescription" runat="server" controlname="chkIncludeTagsInDescription" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkIncludeTagsInDescription" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><dnn:sectionhead id="secAdvancedSettings" isExpanded="true" includerule="True" resourcekey="tblAdvancedSettings"
				section="tblAdvancedSettings" text="Advanced Settings" runat="server" cssclass="Head"></dnn:sectionhead>
			<table id="tblAdvancedSettings" cellSpacing="0" cellPadding="2" width="100%" summary="Edit ViewBlog Advanced Settings"
				border="0" runat="server">
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblUploadOption" runat="server" suffix="" controlname="chkUploadOption"></dnn:label></td>
					<td><asp:checkbox id="chkUploadOption" runat="server" AutoPostBack="False" TextAlign="Left"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblPageBlogs" runat="server" controlname="cmbPageBlogs" suffix=""></dnn:label></td>
					<td><asp:dropdownlist id="cmbPageBlogs" AutoPostBack="True" DataValueField="BlogID" DataTextField="Title"
							Runat="server"></asp:dropdownlist></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblEnableDNNSearch" runat="server" controlname="chkEnableDNNSearch" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkEnableDNNSearch" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300">
						<dnn:label id="lblEnableBookmarks" runat="server" suffix="" controlname="lblEnableBookmarks"></dnn:label></td>
					<td>
						<asp:checkbox id="chkEnableBookmarks" runat="server" AutoPostBack="False" TextAlign="Left"></asp:checkbox></td>
				</tr>
				<tr>
					<td class="SubHead" vAlign="top" width="300">
						<dnn:label id="lblEnforceSummaryTruncation" runat="server" suffix="" controlname="lblEnforceSummaryTruncation"></dnn:label></td>
					<td>
						<asp:checkbox id="chkEnforceSummaryTruncation" runat="server" AutoPostBack="False" TextAlign="Left"></asp:checkbox></td>
				</tr>
				<tr vAlign="top">
					<td class="SubHead" vAlign="top" width="300"><dnn:label id="lblShowSummary" runat="server" controlname="chkShowSummary" suffix=""></dnn:label></td>
					<td><asp:checkbox id="chkShowSummary" runat="server" TextAlign="Left" AutoPostBack="False"></asp:checkbox></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><asp:linkbutton class="CommandButton" id="cmdUpdateOptions" runat="server" CausesValidation="False"
				BorderStyle="none" Text="Update" ResourceKey="cmdUpdate"></asp:linkbutton>&nbsp;
			<asp:linkbutton id="cmdCancelOptions" runat="server" CausesValidation="False" BorderStyle="none"
				Text="Cancel" ResourceKey="cmdCancel" CssClass="CommandButton"></asp:linkbutton></td>
	</tr>
</table>
<p></p>
