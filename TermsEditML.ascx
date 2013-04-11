<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="TermsEditML.ascx.vb" Inherits="DotNetNuke.Modules.Blog.TermsEditML" %>


<div id="termtable"></div>

<script type="text/javascript">
(function ($, Sys) {
 $(document).ready(function () {
  $("#termtable").handsontable({
   startRows: 8,
   startCols: 4,
   rowHeaders: false,
   colHeaders: true,
   minSpareRows: 1,
   contextMenu: false,
   colHeaders: ['Default', 'nl', 'fr'],
   columns: [
    {data: "DefaultName"},
    {data: "LocNames._texts.nl-NL"},
    {data: "LocNames._texts.fr-FR"}
   ]
  });
  var handsontable = $("#termtable").data('handsontable');
 
  blogService.getVocabularyML(<%=VocabularyId%>, function (res) {
    handsontable.loadData(res);
  })
 });
} (jQuery, window.Sys));


</script>
