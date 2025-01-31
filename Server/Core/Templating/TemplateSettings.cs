using System;
using System.Collections.Generic;
// 
// DNN Connect - http://dnn-connect.org
// Copyright (c) 2015
// by DNN Connect
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Templating
{

  [Serializable()]
  [XmlRoot("templateSettings")]
  public class TemplateSettings
  {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    [XmlElement("templateSetting")]
    public List<TemplateSetting> Settings { get; set; } = new List<TemplateSetting>();

    private string _settingsFile = "";
    public TemplateSettings(string templateMapPath) : base()
    {
      _settingsFile = templateMapPath + "settings.xml";
      var x = new XmlSerializer(typeof(TemplateSettings));
      if (System.IO.File.Exists(_settingsFile))
      {
        using (var rdr = new System.IO.StreamReader(_settingsFile))
        {
          TemplateSettings a = (TemplateSettings)x.Deserialize(rdr);
          Settings = a.Settings;
        }
      }
    }
    public TemplateSettings()
    {
    }

  }
}