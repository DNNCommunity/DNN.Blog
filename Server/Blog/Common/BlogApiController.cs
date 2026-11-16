using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Api;
using System.Net;
using System.Net.Http;

namespace DotNetNuke.Modules.Blog.Common
{
    public class BlogApiController : DnnApiController
    {
        public static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DnnApiController));
        
        private ContextHelper _BlogModuleContext;
        public ContextHelper BlogModuleContext
        {
            get { return _BlogModuleContext ?? (_BlogModuleContext = new ContextHelper(this)); }
        }

        public HttpResponseMessage ServiceError(string message) {
            return Request.CreateResponse(HttpStatusCode.InternalServerError, message);
        }

        public HttpResponseMessage AccessViolation(string message)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized, message);
        }

    }
}