var blogService
jQuery(function ($) {
 blogService = new BlogService($, {
   serverErrorText: '[resx:ServerError|jssafe]',
   serverErrorWithDescriptionText: '[resx:ServerErrorWithDescription|jssafe]',
   errorBoxId: '#blogServiceErrorBox[module:moduleId]'
  },
  [module:moduleID]);
});
