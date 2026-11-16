using DotNetNuke.Web.Mvc.Framework.Controllers;

namespace DotNetNuke.Modules.Blog.Common
{
  public class BlogMvcController : DnnController
  {

    private ContextHelper _BlogModuleContext;
    public ContextHelper BlogModuleContext
    {
      get { return _BlogModuleContext ?? (_BlogModuleContext = new ContextHelper(this)); }
    }

  }
}