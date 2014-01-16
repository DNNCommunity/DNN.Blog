(function ($, Sys) {
 $(document).ready(function () {
  $("a > img").parent().colorbox();
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
   urlCurl: '',
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
  $('a[href$=".gif"], a[href$=".jpg"], a[href$=".png"], a[href$=".bmp"]').fancybox();
  SyntaxHighlighter.autoloader.apply(null, shpath(
  'applescript            @shBrushAppleScript.js',
  'actionscript3 as3      @shBrushAS3.js',
  'bash shell             @shBrushBash.js',
  'coldfusion cf          @shBrushColdFusion.js',
  'cpp c                  @shBrushCpp.js',
  'c# c-sharp csharp      @shBrushCSharp.js',
  'css                    @shBrushCss.js',
  'delphi pascal          @shBrushDelphi.js',
  'diff patch pas         @shBrushDiff.js',
  'erl erlang             @shBrushErlang.js',
  'groovy                 @shBrushGroovy.js',
  'java                   @shBrushJava.js',
  'jfx javafx             @shBrushJavaFX.js',
  'js jscript javascript  @shBrushJScript.js',
  'perl pl                @shBrushPerl.js',
  'php                    @shBrushPhp.js',
  'text plain             @shBrushPlain.js',
  'py python              @shBrushPython.js',
  'ruby rails ror rb      @shBrushRuby.js',
  'sass scss              @shBrushSass.js',
  'scala                  @shBrushScala.js',
  'sql                    @shBrushSql.js',
  'vb vbnet               @shBrushVb.js',
  'xml xhtml xslt html    @shBrushXml.js'
  ));
  SyntaxHighlighter.all();
 });
} (jQuery, window.Sys));
function karmaToggle(blogId, commentId, karma, confirmation, success) {
  if (confirmation != null) {
   if (!confirm(confirmation)) {
    return false;
   }
  }
  blogService.karmaComment(blogId, commentId, karma, success)
  return false;
};
function shpath() {
 var args = arguments, result = [];
 for (var i = 0; i < args.length; i++)
  result.push(args[i].replace('@', appPath+'/desktopmodules/blog/templates/_default/js/highlighter/'));
 return result
};
