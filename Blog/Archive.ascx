<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Archive.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Archive" %>
<div class="dnnClear dnnArchive" id="blogArchive">
	<div>
		<h3>Archive</h3>
		<asp:Calendar ID="calMonth" runat="server" CssClass="socialEvent-calendar" TitleStyle-CssClass="socialEvent-calendarTitle" 
		DayHeaderStyle-CssClass="socialEvent-calendarDayHeader" DayStyle-CssClass="socialEvent-calendarDay" 
		NextPrevStyle-CssClass="socialEvent-calendarNextPrev" OtherMonthDayStyle-CssClass="socialEvent-calendarOtherMonth" 
		SelectedDayStyle-CssClass="socialEvent-calendarSelectedDay" SelectorStyle-CssClass="socialEvent-calendarSelector" 
		TodayDayStyle-CssClass="socialEvent-calendarToday" WeekendDayStyle-CssClass="socialEvent-calendarWeekend" 
		TitleStyle-BackColor="transparent" NextMonthText="&gt;&gt;" PrevMonthText="&lt;&lt;" />
	</div>
	<div>
		<h3>Monthly</h3>
		<asp:DataList ID="lstArchiveMonths" runat="server">
			<HeaderTemplate>
				<ul class="qaRecentTags">
			</HeaderTemplate>
			<ItemTemplate>
				<li>
					<asp:HyperLink runat="server" ID="lnkMonthYear" />
					<span>
						<asp:HyperLink runat="server" ID="lnkBlogRSS" Visible="False" Target="_blank">
							<asp:Image ID="lnkBlogRSSIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" />
						</asp:HyperLink>
					</span>
				</li>
			</ItemTemplate>
			<FooterTemplate>
				</ul>
			</FooterTemplate>
		</asp:DataList>
	</div>
	<div class="dnnFormItem">
		<asp:DropDownList ID="ddlArchiveMonths" runat="server" Visible="false" /><asp:LinkButton ID="cmdGo" runat="server" resourcekey="cmdGo" CssClass="dnnSecondaryAction" Visible="false" />
	</div>
</div>