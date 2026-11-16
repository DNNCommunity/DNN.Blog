using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using System;

namespace DotNetNuke.Modules.Blog.Common
{
    public class ModuleController : IPortable
    {
        string IPortable.ExportModule(int ModuleID)
        {
            var m = DotNetNuke.Entities.Modules.ModuleController.Instance.GetModule(ModuleID, Null.NullInteger, true);
            var settings = ModuleSettings.GetSettings(m);
            return Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.None);
        }

        void IPortable.ImportModule(int ModuleID, string Content, string Version, int UserID)
        {
            try
            {
                var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<ModuleSettings>(Content);
                var m = DotNetNuke.Entities.Modules.ModuleController.Instance.GetModule(ModuleID, Null.NullInteger, true);
                settings.SaveSettings(m);
            }
            catch (Exception ex)
            {
            }
        }
    }
}