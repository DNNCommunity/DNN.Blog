var blogService
jQuery(function ($) {
 blogService = new BlogService($, {
   serverErrorText: '[resx:ServerError]',
   serverErrorWithDescriptionText: '[resx:ServerErrorWithDescription]',
   errorBoxId: '#blogServiceErrorBox[module:moduleId]'
  },
  [module:moduleID]);
});
