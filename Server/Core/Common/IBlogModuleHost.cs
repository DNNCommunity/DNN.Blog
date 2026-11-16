using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.Blog.Core.Common
{
  /// <summary>
  /// Abstraction over WebForms PortalModuleBase / MVC DnnController so BlogContextInfo works in both hosts.
  /// </summary>
  public interface IBlogModuleHost
  {
    int ModuleId { get; }
    int TabModuleId { get; }
    int TabId { get; }
    int PortalId { get; }
    UserInfo UserInfo { get; }
    ModuleInfo ModuleConfiguration { get; }
    ViewSettings ViewSettings { get; }
    ModuleSettings Settings { get; }
    TimeZoneInfo TimeZone { get; }
  }
}
