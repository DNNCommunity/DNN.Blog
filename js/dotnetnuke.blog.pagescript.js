var blogService
jQuery(function ($) {
 blogService = new BlogService($, {
   serverErrorText: '[resx:ServerError]',
   serverErrorWithDescriptionText: '[resx:ServerErrorWithDescription]'
  },
  $.dnnSF([module:moduleID]));
});
