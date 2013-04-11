function BlogService($, settings, servicesFramework) {
 var baseServicepath = servicesFramework.getServiceRoot('Blog') + 'Post/';
 var commentsServicepath = servicesFramework.getServiceRoot('Blog') + 'Comment/';
 var modulesServicepath = servicesFramework.getServiceRoot('Blog') + 'Modules/';
 var blogServicepath = servicesFramework.getServiceRoot('Blog') + 'Blog/';
 var termServicepath = servicesFramework.getServiceRoot('Blog') + 'Terms/';

 this.approvePost = function (blogId, PostId, success) {
  $.ajax({
   type: "POST",
   url: baseServicepath + "Approve",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, PostId: PostId }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.deletePost = function (blogId, PostId, success) {
  $.ajax({
   type: "POST",
   url: baseServicepath + "Delete",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, PostId: PostId }
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

 this.addModule = function (paneName, position, title, template, success) {
  $.ajax({
   type: "POST",
   url: modulesServicepath + "Add",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { paneName: paneName, position: position, title: title, template: template }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.exportBlog = function (blogId, success) {
  $.ajax({
   type: "POST",
   url: blogServicepath + "Export",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId }
  }).done(function (data) {
   if (success != undefined) {
    success(data.Result);
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.getVocabularyML = function (vocabularyId, success) {
  $.ajax({
   type: "GET",
   url: termServicepath + "VocabularyML",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { vocabularyId: vocabularyId }
  }).done(function (data) {
   if (success != undefined) {
    success(data);
   }
  }).fail(function (xhr, status) {
   displayMessage("#blogServiceErrorBox", settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 }

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
