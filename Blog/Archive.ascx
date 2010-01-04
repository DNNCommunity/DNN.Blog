<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Archive.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Archive" %>
<asp:Label ID="lblArchive" runat="server" CssClass="SubHead" ResourceKey="lblArchive" />
<asp:Calendar ID="calMonth" runat="server" CssClass="Normal" DayHeaderStyle-CssClass="Blog_Archive_DayHeader" DayStyle-CssClass="Blog_Archive_Day" NextPrevStyle-CssClass="Blog_Archive_NextPrev" OtherMonthDayStyle-CssClass="Blog_Archive_OtherMonth" SelectedDayStyle-CssClass="Blog_Archive_SelectedDay" SelectorStyle-CssClass="Blog_Archive_Selector" TitleStyle-CssClass="Blog_Archive_Title" TodayDayStyle-CssClass="Blog_Archive_TodayDay" WeekendDayStyle-CssClass="Blog_Archive_WeekendDay">
 <TodayDayStyle CssClass="Blog_Archive_TodayDay" />
 <SelectorStyle CssClass="Blog_Archive_Selector" />
 <DayStyle CssClass="Blog_Archive_Day" />
 <NextPrevStyle CssClass="Blog_Archive_NextPrev" />
 <DayHeaderStyle CssClass="Blog_Archive_DayHeader" />
 <SelectedDayStyle CssClass="Blog_Archive_SelectedDay" />
 <TitleStyle CssClass="Blog_Archive_Title" />
 <WeekendDayStyle CssClass="Blog_Archive_WeekendDay" />
 <OtherMonthDayStyle CssClass="Blog_Archive_OtherMonth" />
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
