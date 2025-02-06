
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

using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  public abstract class GenericTokenReplace : TokenReplace
  {

    public object PrimaryObject { get; set; } = null;

    public GenericTokenReplace(Scope accessLevel) : base(Scope.DefaultSettings)
    {
    }

    public GenericTokenReplace(Scope accessLevel, int moduleId) : base(Scope.DefaultSettings, moduleId)
    {
    }

    public new string ReplaceTokens(string strSourceText)
    {
      strSourceText = strSourceText.Replace(@"\[", "{{").Replace(@"\]", "}}");
      return base.ReplaceTokens(strSourceText).Replace("{{", "[").Replace("}}", "]");
    }

    public new string ReplaceTokens(string strSourceText, params string[] additionalParameters)
    {
      strSourceText = strSourceText.Replace(@"\[", "{{").Replace(@"\]", "}}");
      PropertySource["custom"] = new CustomParameters(additionalParameters);
      return base.ReplaceTokens(strSourceText).Replace("{{", "[").Replace("}}", "]");
    }

    public void AddCustomParameters(params string[] additionalParameters)
    {
      PropertySource["custom"] = new CustomParameters(additionalParameters);
    }

    public void AddResources(string TemplateRelPath)
    {
      PropertySource["resx"] = new Resources(TemplateRelPath);
    }

    public void AddPropertySource(string key, IPropertyAccess resource)
    {
      PropertySource[key] = resource;
    }

    public string GetTokenValue(string obj, string prop, string format)
    {
      if (!PropertySource.ContainsKey(obj))
        return "";
      bool bFound = false;
      return PropertySource[obj].GetProperty(prop, format, System.Threading.Thread.CurrentThread.CurrentCulture, null, Scope.DefaultSettings, ref bFound);
    }

  }
}