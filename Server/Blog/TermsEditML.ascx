<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="TermsEditML.ascx.vb" Inherits="DotNetNuke.Modules.Blog.TermsEditML" %>


<div id="termtable"></div>

<p style="width:100%;text-align:center">
 <asp:LinkButton runat="server" ID="cmdCancel" resourcekey="cmdCancel" CssClass="dnnSecondaryAction" />
 <asp:LinkButton runat="server" ID="cmdUpdate" resourcekey="cmdUpdate" CssClass="dnnPrimaryAction" />
</p>


<asp:HiddenField runat="server" ID="Storage" />

<script type="text/javascript">
(function ($, Sys) {
 $(document).ready(function () {
  $("#termtable").handsontable({
   rowHeaders: false,
   colHeaders: true,
   contextMenu: false,
   colHeaders: <%=ColumnHeaders%>,
   columns: <%=Columns%>,
   onChange: function (change, source) {
    $('#<%=Storage.ClientID%>').val(JSON.stringify(handsontable.getData()))
   },
   onBeforeChange: function (data) {
    for (var i = data.length - 1; i >= 0; i--) {
     if(data[i][1]=='DefaultName' && data[i][3]=='') {
      data[i][3]=data[i][2];
     }
    }
   }
  });
  var handsontable = $("#termtable").data('handsontable');
 
  blogService.getVocabularyML(<%=VocabularyId%>, function (res) {
    handsontable.loadData(res);
  })
 });
} (jQuery, window.Sys));


</script>
