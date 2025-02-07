using System.Collections;
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2015
// by DotNetNuke Corporation
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
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Blog.Core.Entities.Posts;

namespace DotNetNuke.Modules.Blog.Core.Integration
{

  public class FileController
  {

    #region Shared methods

    public static ArrayList getFileList(string pModulPath, PostInfo pPost)
    {

      var myList = new ArrayList();
      if (Directory.Exists(getPostDir(pModulPath, pPost)))
      {
        string[] fileList = Directory.GetFiles(getPostDir(pModulPath, pPost));
        foreach (string s in fileList)
          myList.Add(Path.GetFileName(s));
      }
      return myList;

    }

    public static string getPostDir(string pModulPath, PostInfo pPost)
    {
      string getPostDirRet = default;

      PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
      if (pPost is null)
      {
        getPostDirRet = HttpContext.Current.Request.MapPath(pModulPath) + @"Files\AnonymousBlogAttachments\";
      }
      else
      {
        getPostDirRet = HttpContext.Current.Request.MapPath(pModulPath) + @"Files\" + pPost.BlogID.ToString() + @"\" + pPost.ContentItemId.ToString() + @"\";
      }

      return getPostDirRet;

    }

    public static string createFileDirectory(string filePath)
    {

      string newFolderPath = filePath.Substring(0, filePath.LastIndexOf(@"\"));
      if (!Directory.Exists(newFolderPath))
        Directory.CreateDirectory(newFolderPath);
      return newFolderPath;

    }

    public static string getVirtualFileName(string pModulPath, string pFullPath)
    {
      string getVirtualFileNameRet = default;
      string strReturn;
      strReturn = pFullPath.Replace(HttpContext.Current.Request.MapPath(pModulPath), "");
      strReturn = strReturn.Replace(@"\", "/");
      strReturn = pModulPath + strReturn;
      getVirtualFileNameRet = DotNetNuke.Common.Globals.ResolveUrl(strReturn);
      return getVirtualFileNameRet;
    }
    #endregion

  }

}