using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Modules.Blog.Common;
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

using DotNetNuke.Modules.Blog.Entities.Posts;

namespace DotNetNuke.Modules.Blog.Services
{
  public class TrackAndPingBackController
  {

    #region  Private Members 
    private static readonly Regex TrackbackLinkRegex = new Regex("trackback:ping=\"([^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex UrlsRegex = new Regex("<a.*?href=[\"'](?<url>.*?)[\"'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private PostInfo Post { get; set; } = null;
    private DotNetNuke.Entities.Portals.PortalSettings PortalSettings { get; set; } = null;
    #endregion

    #region  TrackbackMessage 
    public struct TrackbackMessage
    {

      #region  Properties 
      public string BlogName { get; set; }
      public string Excerpt { get; set; }
      public Uri PostUrl { get; set; }
      public string Title { get; set; }
      public Uri UrlToNotifyTrackback { get; set; }
      #endregion

      #region  Constructors 
      public TrackbackMessage(PostInfo post, Uri urlToNotifyTrackback, DotNetNuke.Entities.Portals.PortalSettings portalSettings)
      {
        Title = post.Title;
        PostUrl = new Uri(post.PermaLink(portalSettings));
        Excerpt = post.Summary;
        BlogName = post.Blog.Title;
        UrlToNotifyTrackback = urlToNotifyTrackback;
      }
      #endregion

      #region  Public Methods 
      public override string ToString()
      {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "title={0}&url={1}&excerpt={2}&blog_name={3}", Title, PostUrl, Excerpt, BlogName);
      }
      #endregion

    }
    #endregion

    #region  Constructors 
    public TrackAndPingBackController(PostInfo post)
    {
      Post = post;
      PortalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
    }
    #endregion

    #region  Public Methods 
    public void SendTrackAndPingBacks()
    {

      if (!Post.Blog.EnableTrackBackSend & !Post.Blog.EnablePingBackSend)
        return;

      foreach (Uri url in GetUrlsFromContent(HttpUtility.HtmlDecode(Post.Content)))
      {

        bool trackbackSent = false;
        if (Post.Blog.EnableTrackBackSend)
        {
          var remoteFile = new WebPage(url);
          string pageContent = remoteFile.GetFileAsString();
          var trackbackUrl = GetTrackBackUrlFromPage(pageContent);
          if (trackbackUrl is not null)
          {
            var message = new TrackbackMessage(Post, trackbackUrl, PortalSettings);
            trackbackSent = SendTrackback(message);
          }
        }
        if (!trackbackSent && Post.Blog.EnablePingBackSend)
        {
          SendPingback(new Uri(Post.PermaLink(PortalSettings)), url);
        }

      }

    }
    #endregion

    #region  Trackback 
    public bool SendTrackback(TrackbackMessage message)
    {

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(message.UrlToNotifyTrackback);

      request.Credentials = CredentialCache.DefaultNetworkCredentials;
      request.Method = "POST";
      request.ContentLength = message.ToString().Length;
      request.ContentType = "application/x-www-form-urlencoded";
      request.KeepAlive = false;
      request.Timeout = 30000;

      using (var writer = new System.IO.StreamWriter(request.GetRequestStream()))
      {
        writer.Write(message.ToString());
      }

      bool result = false;
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse)request.GetResponse();
        string answer;
        using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
        {
          answer = sr.ReadToEnd();
        }
        result = response.StatusCode == HttpStatusCode.OK && answer.Contains("<error>0</error>");
      }
      catch (Exception ex)
      {
      }

      return result;

    }
    #endregion

    #region  Pingback 
    public void SendPingback(Uri sourceUrl, Uri targetUrl)
    {

      try
      {

        string pingUrl = null;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUrl);
        request.Credentials = CredentialCache.DefaultNetworkCredentials;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
          foreach (string k in response.Headers.AllKeys)
          {
            if (k.Equals("x-pingback", StringComparison.OrdinalIgnoreCase) || k.Equals("pingback", StringComparison.OrdinalIgnoreCase))
            {
              pingUrl = response.Headers[k];
            }
          }
          if (string.IsNullOrEmpty(pingUrl))
          {
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
              string content = reader.ReadToEnd();
              var m = Regex.Match(content, "<link[^>]*rel=\"pingback\"[^>]*>|<link[^>]*rel='pingback'[^>]*>");
              if (m.Success)
              {
                string link = m.Value;
                var m2 = Regex.Match(link, "href=\"(?<link>[^\"]*)\"|href='(?<link>[^']*)'");
                if (m2.Success)
                {
                  pingUrl = m2.Groups["link"].Value.Replace("&amp;", "&");
                }
              }
            }
          }
        }

        Uri url = null;
        if (!string.IsNullOrEmpty(pingUrl) && Uri.TryCreate(pingUrl, UriKind.Absolute, out url))
        {
          request = (HttpWebRequest)WebRequest.Create(url);
          request.Method = "POST";
          request.Timeout = 30000;
          request.ContentType = "text/xml";
          request.ProtocolVersion = HttpVersion.Version11;
          request.Headers["Accept-Language"] = "en-us";
          AddXmlToRequest(sourceUrl, targetUrl, request);
          using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
          {
          }
        }
      }

      catch (Exception ex)
      {
        ex = new Exception();
      }

    }
    #endregion

    #region  Private Methods 
    private static Uri GetTrackBackUrlFromPage(string input)
    {
      string url = TrackbackLinkRegex.Match(input).Groups[1].ToString().Trim();
      Uri uri = null;
      return Uri.TryCreate(url, UriKind.Absolute, out uri) ? uri : null;
    }

    private static IEnumerable<Uri> GetUrlsFromContent(string content)
    {
      var urlsList = new List<Uri>();
      foreach (Match m in UrlsRegex.Matches(content))
      {
        string url = m.Groups["url"].ToString().Trim();
        Uri uri = null;
        if (Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
          urlsList.Add(uri);
        }
      }
      return urlsList;
    }

    private static void AddXmlToRequest(Uri sourceUrl, Uri targetUrl, HttpWebRequest webreqPing)
    {
      using (var stream = webreqPing.GetRequestStream())
      {
        using (var writer = new System.Xml.XmlTextWriter(stream, Encoding.ASCII))
        {
          writer.WriteStartDocument(true);
          writer.WriteStartElement("methodCall");
          writer.WriteElementString("methodName", "pingback.ping");
          writer.WriteStartElement("params");

          writer.WriteStartElement("param");
          writer.WriteStartElement("value");
          writer.WriteElementString("string", sourceUrl.ToString());
          writer.WriteEndElement();
          writer.WriteEndElement();

          writer.WriteStartElement("param");
          writer.WriteStartElement("value");
          writer.WriteElementString("string", targetUrl.ToString());
          writer.WriteEndElement();
          writer.WriteEndElement();

          writer.WriteEndElement();
          writer.WriteEndElement();
        }
      }
    }
    #endregion

  }
}