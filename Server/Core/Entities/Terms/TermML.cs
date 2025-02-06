using System.Collections.Generic;

namespace DotNetNuke.Modules.Blog.Core.Entities.Terms
{
  public struct TermML
  {
    public int TermID;
    public string DefaultName;
    public Dictionary<string, string> LocNames;
  }
}
