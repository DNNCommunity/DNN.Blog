(function ($, Sys) {
 $(document).ready(function () {
  $('#[ID]').dynatree({
   onSelect: function (flag, node) {
    var selectedNodes = $("#[ID]").dynatree("getTree").serializeArray();
    var res = '';
    var i;
    for (i = 0; i < selectedNodes.length; i += 1) {
     res += selectedNodes[i].value + ',';
    }
    $('#[StorageControlId]').val(res)
   },
   checkbox: true
   [Children]
  })
  var selectedNodes = $("#[ID]").dynatree("getTree").serializeArray();
  var res = '';
  var i;
  for (i = 0; i < selectedNodes.length; i += 1) {
   res += selectedNodes[i].value + ',';
  }
  $('#[StorageControlId]').val(res)
 });
} (jQuery, window.Sys));
