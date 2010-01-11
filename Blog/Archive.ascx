<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Archive.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Archive" %>
<asp:Label ID="lblArchive" runat="server" CssClass="SubHead" ResourceKey="lblArchive" />
<asp:Calendar ID="calMonth" runat="server" CssClass="Normal" DayHeaderStyle-CssClass="BlogArchiveDayHeader" DayStyle-CssClass="BlogArchiveDay" NextPrevStyle-CssClass="BlogArchiveNextPrev" OtherMonthDayStyle-CssClass="BlogArchiveOtherMonth" SelectedDayStyle-CssClass="BlogArchiveSelectedDay" SelectorStyle-CssClass="BlogArchiveSelector" TitleStyle-CssClass="BlogArchiveTitle" TodayDayStyle-CssClass="BlogArchiveTodayDay" WeekendDayStyle-CssClass="BlogArchiveWeekendDay">
 <TodayDayStyle CssClass="BlogArchiveTodayDay" />
 <SelectorStyle CssClass="BlogArchiveSelector" />
 <DayStyle CssClass="BlogArchiveDay" />
 <NextPrevStyle CssClass="BlogArchiveNextPrev" />
 <DayHeaderStyle CssClass="BlogArchiveDayHeader" />
 <SelectedDayStyle CssClass="BlogArchiveSelectedDay" />
 <TitleStyle CssClass="BlogArchiveTitle" />
 <WeekendDayStyle CssClass="BlogArchiveWeekendDay" />
 <OtherMonthDayStyle CssClass="BlogArchiveOtherMonth" />
</asp:Calendar>
<asp:Label ID="lblMonthly" runat="server" CssClass="SubHead" ResourceKey="lblMonthly" />
<asp:DataList ID="lstArchiveMonths" runat="server" Width="100%" CssClass="CommandButton"
 CellPadding="0" CellSpacing="0" BorderWidth="0">
 <ItemTemplate>
  <table width="100%" cellpadding="0" cellspacing="0" border="0" align="left">
   <tr>
    <td align="left">
     <asp:HyperLink runat="server" ID="lnkMonthYear" CssClass="CommandButton" NavigateUrl="default.aspx" />
    </td>
    <td align="right">
     <asp:HyperLink CssClass="CommandButton" runat="server" ID="lnkBlogRSS" Visible="False" Target="_blank">
      <asp:Image ID="lnkBlogRSSIcon" runat="server" ImageUrl="~/desktopmodules/Blog/Images/feed-icon-12x12.gif" />
     </asp:HyperLink>
    </td>
   </tr>
  </table>
 </ItemTemplate>
</asp:DataList>
