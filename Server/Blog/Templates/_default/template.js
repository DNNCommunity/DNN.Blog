(function ($, Sys) {
  $(document).ready(function () {
    $('a[href$=".gif"], a[href$=".jpg"], a[href$=".png"], a[href$=".bmp"]').colorbox();
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
