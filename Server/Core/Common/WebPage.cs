using System;
using System.IO;
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

using System.Net;

namespace DotNetNuke.Modules.Blog.Core.Common
{
  public class WebPage
  {

    #region  Properties 
    private readonly Uri url;
    public Uri Uri
    {
      get
      {
        return url;
      }
    }
    #endregion

    #region  Constructors 
    public WebPage(Uri filePath)
    {
      if (filePath is null)
      {
        throw new ArgumentNullException("filePath");
      }
      url = filePath;
    }
    #endregion

    #region  Public Methods 
    public WebResponse GetWebResponse()
    {
      var response = GetWebRequest().GetResponse();
      long contentLength = response.ContentLength;
      if (contentLength == -1)
      {
        string headerContentLength = response.Headers["Content-Length"];
        if (!string.IsNullOrEmpty(headerContentLength))
        {
          contentLength = long.Parse(headerContentLength);
        }
      }
      if (contentLength <= -1)
      {
        response.Close();
        return null;
      }
      return response;
    }

    private WebRequest _webRequest;

    public string GetFileAsString()
    {
      try
      {
        using (var response = GetWebResponse())
        {
          if (response is null)
          {
            return string.Empty;
          }
          using (var reader = new StreamReader(response.GetResponseStream()))
          {
            return reader.ReadToEnd();
          }
        }
      }
      catch (Exception ex)
      {
        DotNetNuke.Services.Exceptions.Exceptions.LogException(new Exception(string.Format("Track/Pingback Verification Request To '{0}' Failed", Uri.PathAndQuery), ex));
        return "";
      }
    }
    #endregion

    #region  Private Methods 
    private WebRequest GetWebRequest()
    {

      if (_webRequest is null)
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri);
        request.Headers["Accept-Encoding"] = "gzip";
        request.Headers["Accept-Language"] = "en-us";
        request.Credentials = CredentialCache.DefaultNetworkCredentials;
        request.AutomaticDecompression = DecompressionMethods.GZip;
        request.Timeout = 1000 * 30; // 30 secs timeout
        _webRequest = request;
      }
      return _webRequest;

    }
    #endregion

  }
}