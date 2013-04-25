<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="TemplateSettings.ascx.vb" Inherits="DotNetNuke.Modules.Blog.Controls.TemplateSettings" %>
<asp:Table runat="server" ID="tblSettings" CellPadding="2" CellSpacing="0" BorderWidth="0" />
<div runat="server" id="divMessage" class="dnnFormMessage dnnFormWarning"></div>
<p style="width:100%;text-align:center;">
 <asp:LinkButton runat="server" ID="cmdCancel" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdUpdate" resourcekey="cmdUpdate" CssClass="dnnPrimaryAction" />
</p>
