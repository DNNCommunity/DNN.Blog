function BlogService($, settings, servicesFramework) {
 var baseServicepath = servicesFramework.getServiceRoot('Blog') + 'Posts/';
 var commentsServicepath = servicesFramework.getServiceRoot('Blog') + 'Comments/';
 var modulesServicepath = servicesFramework.getServiceRoot('Blog') + 'Modules/';
 var blogServicepath = servicesFramework.getServiceRoot('Blog') + 'Blogs/';
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
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
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
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.approveComment = function (blogId, commentId, success) {
  $.ajax({
   type: "POST",
   url: commentsServicepath + "Approve",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, commentId: commentId, karma: 0 }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.deleteComment = function (blogId, commentId, success) {
  $.ajax({
   type: "POST",
   url: commentsServicepath + "Delete",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, commentId: commentId, karma: 0 }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.karmaComment = function (blogId, commentId, karma, success) {
  $.ajax({
   type: "POST",
   url: commentsServicepath + "Karma",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, commentId: commentId, karma: karma }
  }).done(function (data) {
   if (data.Result == 'exists') {
   // user already did this
   };
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.addComment = function (blogId, postId, parentId, comment, author, website, email, success) {
  $.ajax({
   type: "POST",
   url: commentsServicepath + "Add",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, postId: postId, parentId: parentId, comment: comment, author: author, website: website, email: email }
  }).done(function (data) {
   if (success != undefined) {
    success();
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 };

 this.getCommentsHtml = function (blogId, postId, success) {
  $.ajax({
   type: "GET",
   url: commentsServicepath + "List",
   beforeSend: servicesFramework.setModuleHeaders,
   data: { blogId: blogId, postId: postId }
  }).done(function (data) {
   if (success != undefined) {
    success(data);
   }
  }).fail(function (xhr, status) {
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
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
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
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
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
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
   displayMessage(settings.errorBoxId, settings.serverErrorWithDescription + eval("(" + xhr.responseText + ")").ExceptionMessage, "dnnFormWarning");
  });
 }

}

function displayMessage(msgBoxId, message, cssclass) {
 var messageNode = $("<div/>")
                .addClass('dnnFormMessage ' + cssclass)
                .text(message);
 $(msgBoxId).prepend(messageNode);
 messageNode.fadeOut(3000, 'easeInExpo', function () {
  messageNode.remove();
 });
};
