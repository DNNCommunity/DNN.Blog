(function ($, Sys) {
 $(document).ready(function () {
  var servicesFramework = $.dnnSF([ModuleId]);
  $('#[ID]').tagit({
   fieldName: '[ClientID]',
   allowSpaces: [AllowSpaces],
   tagLimit: [TagLimit],
   placeholderText: '[PlaceholderText]',
   autocomplete: {
    delay: 0,
    minLength: 2,
    source: servicesFramework.getServiceRoot('Blog') + "/Terms/Search?ModuleId=[ModuleId]&TabId=[TabId]&Vocab=[VocabularyId]",
    appendTo: $('#[ClientID]')
   }
  })
 });
} (jQuery, window.Sys));
