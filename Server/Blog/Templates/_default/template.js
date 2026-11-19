(function ($, Sys) {
  $(document).ready(function () {
    // Legacy posts wrap images in <a href="file.jpg">; migrated markup rewrites that to /Image/Get.
    // Force photo mode because Image/Get URLs have no file extension for Colorbox's auto-detect.
    $('a[href$=".gif"], a[href$=".jpg"], a[href$=".jpeg"], a[href$=".png"], a[href$=".bmp"], a[href*="/Image/Get?"]').colorbox({
      photo: true
    });
    $(".moment").each(function (i, el) {
      var e = $(el);
      e.html(moment(e.attr('data-time')).fromNow());
    });
  $(".momentabs").each(function (i, el) {
    var e = $(el);
    e.html(moment(e.attr('data-time')).format('LL'));
  });
  });
}(jQuery, window.Sys));
