(function ($, Sys) {
 $(document).ready(function () {
  $("abbr.blog_commenttimeago").timeago();
  $('#cmdComment').click(function () {
   $dialogComment.dialog('open');
   return false;
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
