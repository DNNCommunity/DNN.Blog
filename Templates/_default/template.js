(function ($, Sys) {
 $(document).ready(function () {
  $("abbr.blog_commenttimeago").timeago();
  $('#cmdComment').click(function () {
   $dialogComment.dialog('open');
   return false;
  });
  $('#sharrre').sharrre({
   share: {
    googlePlus: true,
    facebook: true,
    twitter: true
   },
   template: '<a href="#" class="blogicon-facebook socialicons facebook"></a><a href="#" class="blogicon-twitter socialicons twitter"></a><a href="#" class="blogicon-google-plus socialicons googleplus"></a>',
   enableHover: false,
   enableTracking: true,
   render: function (api, options) {
    $(api.element).on('click', '.twitter', function () {
     api.openPopup('twitter');
    });
    $(api.element).on('click', '.facebook', function () {
     api.openPopup('facebook');
    });
    $(api.element).on('click', '.googleplus', function () {
     api.openPopup('googlePlus');
    });
   }
  });
 });
} (jQuery, window.Sys));
function karmaToggle(btn, blogId, commentId, karma, confirmation) {
 if (!$(btn).is('.blog_commentLinkSelected')) {
  if (confirmation != null) {
   if (!confirm(confirmation)) {
    return false;
   }
  }
  blogService.karmaComment(blogId, commentId, karma, function() {$(btn).addClass('blog_commentLinkSelected')})
 };
 return false;
}
