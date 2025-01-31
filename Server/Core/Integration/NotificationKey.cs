
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

namespace DotNetNuke.Modules.Blog.Integration
{
  public class NotificationKey
  {

    public string ID = "";
    public int ModuleId = -1;
    public int BlogId = -1;
    public int ContentItemId = -1;
    public int CommentId = -1;

    public NotificationKey(string key)
    {
      string[] keyParts = key.Split(':');
      if (keyParts.Length < 5)
        return;
      ID = keyParts[0];
      ModuleId = int.Parse(keyParts[1]);
      BlogId = int.Parse(keyParts[2]);
      ContentItemId = int.Parse(keyParts[3]);
      CommentId = int.Parse(keyParts[4]);
    }

    public NotificationKey(string id, int moduleId, int blogId, int contentItemId, int commentId)
    {
      ID = id;
      ModuleId = moduleId;
      BlogId = blogId;
      ContentItemId = contentItemId;
      CommentId = commentId;
    }

    public new string ToString()
    {
      return string.Format("{0}:{1}:{2}:{3}:{4}", ID, ModuleId, BlogId, ContentItemId, CommentId);
    }

  }
}