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

using System.IO;
using static DotNetNuke.Common.Globals;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Cache;
using Microsoft.VisualBasic;

namespace DotNetNuke.Modules.Blog.Core.Templating
{
  public class Templating
  {

    #region  Public Methods 
    public static string FormatBoolean(bool value, string format, System.Globalization.CultureInfo formatProvider)
    {
      if (string.IsNullOrEmpty(format))
      {
        return value.ToString();
      }
      if (format.Contains(";"))
      {
        if (value)
        {
          return format.Substring(0, format.IndexOf(";"));
        }
        else
        {
          return format.Substring(format.IndexOf(";") + 1);
        }
      }
      return DotNetNuke.Services.Tokens.PropertyAccess.Boolean2LocalizedYesNo(value, formatProvider);
    }
    #endregion

    #region  Private Methods 
    private static object GetTemplateFileLookupDictionary(CacheItemArgs cacheItemArgs)
    {
      return new Dictionary<string, bool>();
    }

    private static Dictionary<string, bool> GetTemplateFileLookupDictionary()
    {
      return CBO.GetCachedObject<Dictionary<string, bool>>(new CacheItemArgs(DataCache.ResourceFileLookupDictionaryCacheKey, DataCache.ResourceFileLookupDictionaryTimeOut, DataCache.ResourceFileLookupDictionaryCachePriority), GetTemplateFileLookupDictionary);
    }

    private static object GetTemplateFileCallBack(CacheItemArgs cacheItemArgs)
    {

      string cacheKey = cacheItemArgs.CacheKey;
      string Template = null;
      var TemplateFileExistsLookup = GetTemplateFileLookupDictionary();
      if (!TemplateFileExistsLookup.ContainsKey(cacheKey) || TemplateFileExistsLookup[cacheKey])
      {
        string filePath;
        if (cacheKey.Contains(@":\") && Path.IsPathRooted(cacheKey))
        {
          filePath = cacheKey;
        }
        else
        {
          filePath = System.Web.Hosting.HostingEnvironment.MapPath(ApplicationPath + cacheKey);
        }
        if (File.Exists(filePath))
        {
          Template = Common.Globals.ReadFile(filePath);
          cacheItemArgs.CacheDependency = new DNNCacheDependency(filePath);
          TemplateFileExistsLookup[cacheKey] = true;
        }
        else
        {
          TemplateFileExistsLookup[cacheKey] = false;
        }
      }
      return Template;

    }

    public static string GetTemplateFile(string TemplateFile)
    {
      return CBO.GetCachedObject<string>(new CacheItemArgs(TemplateFile, DataCache.ResourceFilesCacheTimeOut, DataCache.ResourceFilesCachePriority), GetTemplateFileCallBack);
    }
    #endregion


  }
}