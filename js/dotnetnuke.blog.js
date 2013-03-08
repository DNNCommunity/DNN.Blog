function BlogService($, settings, servicesFramework) {
 var baseServicepath = servicesFramework.getServiceRoot('Blog') + 'Entry/';
 var commentsServicepath = servicesFramework.getServiceRoot('Blog') + 'Comment/';

 this.approveEntry = function (blogId, entryId, success) {
  $.ajax({
   type: "POST",
   url: baseServicepath + "Approve",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, entryId: entryId }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.deleteEntry = function (blogId, entryId, success) {
  $.ajax({
   type: "POST",
   url: baseServicepath + "Delete",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, entryId: entryId }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.approveComment = function (blogId, commentId, success) {
  $.ajax({
   type: "POST",
   url: commentsServicepath + "Approve",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, commentId: commentId }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.deleteComment = function (blogId, commentId, success) {
  $.ajax({
   type: "POST",
   url: commentsServicepath + "Delete",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, commentId: commentId }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

}

function displayMessage(placeholderSelector, message, cssclass) {
 var messageNode = $("<div/>")
                .addClass('dnnFormMessage ' + cssclass)
                .text(message);
 $(containerElement + " " + placeholderSelector).prepend(messageNode);
 messageNode.fadeOut(3000, 'easeInExpo', function () {
  messageNode.remove();
 });
};
