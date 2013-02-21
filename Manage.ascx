<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Manage.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Manage" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<div class="dnnForm" id="tabs">
 <ul class="dnnAdminTabNav">
  <li><a href="#Blogs">Blogs</a></li>
  <li><a href="#Posts">Posts</a></li>
  <li><a href="#Categories">Categories</a></li>
 </ul>
 <div id="Blogs" class="dnnClear">
  <asp:DataList runat="server" ID="dlBlogs">
   <HeaderTemplate>
    <table class="dnnGrid">
     <tr class="dnnGridHeader">
      <td></td>
      <td></td>
      <td></td>
      <td>Blog</td>
      <td>Owner</td>
     </tr>
   </HeaderTemplate>
   <ItemTemplate>
    <tr class="dnnGridItem">
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogEdit") %>" class="icon16 entypoButton" title="Edit">&#9998;</a></td>
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogImport") %>" class="icon16 entypoButton" title="Import">&#59200;</a></td>
     <td><a href="<%# EditUrl("Blog", Eval("BlogId"), "BlogEdit") %>" class="icon16 entypoButton" title="Export">&#59201;</a></td>
     <td><%# Eval("Title") %></td>
     <td><%# Eval("DisplayName") %></td>
    </tr>
   </ItemTemplate>
   <FooterTemplate>
    </table>
   </FooterTemplate>
  </asp:DataList>
  <p>
   <asp:LinkButton runat="server" ID="cmdCancel" resourcekey="cmdCancel" CssClass="dnnPrimaryAction" />
   <asp:LinkButton runat="server" ID="cmdAdd" resourcekey="cmdAdd" CssClass="dnnSecondaryAction" />
  </p>
 </div>
 <div id="Posts" class="dnnClear">
  <dnn:DNNGrid id="grdEntries" autogeneratecolumns="false" cssclass="dnnGrid dnnSecurityRolesGrid"
   runat="server" allowpaging="True" allowcustompaging="True" enableviewstate="True"
   onneeddatasource="GetEntries">
   <MasterTableView>
    <Columns>
     <dnn:DnnGridTemplateColumn>
      <ItemTemplate>
       <a href="<%# EditUrl("Post", Eval("ContentItemId"), "EntryEdit") %>" class="icon16 entypoButton">&#9998;</a>
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridTemplateColumn>
      <ItemTemplate>
       <a href="#" class="icon16 entypoButton">&#59177;</a>
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridTemplateColumn>
      <ItemTemplate>
       <a href="#" class="icon16 entypoButton">&#128077;</a>
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridTemplateColumn HeaderText="Date">
      <ItemTemplate>
       <asp:Label ID="Label2" Runat="server" Text='<%# CDate(Eval("PublishedOnDate")).ToString("d") %>' ToolTip='<%# CDate(Eval("PublishedOnDate")).ToString("U") %>' />
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridBoundColumn datafield="Title" headertext="Title"/>
     <dnn:DnnGridTemplateColumn HeaderText="Published">
      <ItemTemplate>
       <span class="entypoIcon icon16"><%# IIf(Eval("Published"), "&#10003;", "&#10060;")%></span>
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
     <dnn:DnnGridTemplateColumn HeaderText="Blog">
      <ItemTemplate>
       <asp:Label ID="Label1" Runat="server" Text='<%# CType(Container.DataItem, DotNetNuke.Modules.Blog.Entities.Entries.EntryInfo).Blog.Title %>' />
      </ItemTemplate>
     </dnn:DnnGridTemplateColumn>
    </Columns>
   </MasterTableView>
  </dnn:DNNGrid>
 </div>
 <div id="Categories" class="dnnClear">
 </div>
</div>

<script type="text/javascript">
 jQuery(function ($) {
  $('#tabs').dnnTabs();
 });
</script>
