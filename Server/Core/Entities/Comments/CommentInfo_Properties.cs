using System;
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

using System.Runtime.Serialization;

namespace DotNetNuke.Modules.Blog.Entities.Comments
{
  public partial class CommentInfo
  {

    #region  Private Members 
    #endregion

    #region  Constructors 
    public CommentInfo()
    {
    }
    #endregion

    #region  Public Properties 
    [DataMember()]
    public int CommentID { get; set; } = -1;
    [DataMember()]
    public int ContentItemId { get; set; } = -1;
    [DataMember()]
    public int ParentId { get; set; } = -1;
    [DataMember()]
    public string Comment { get; set; } = "";
    [DataMember()]
    public bool Approved { get; set; } = false;
    [DataMember()]
    public string Author { get; set; } = "";
    [DataMember()]
    public string Website { get; set; } = "";
    [DataMember()]
    public string Email { get; set; } = "";
    [DataMember()]
    public int CreatedByUserID { get; set; } = -1;
    [DataMember()]
    public DateTime CreatedOnDate { get; set; } = DateTime.MinValue;
    [DataMember()]
    public int LastModifiedByUserID { get; set; } = -1;
    [DataMember()]
    public DateTime LastModifiedOnDate { get; set; } = DateTime.MinValue;
    [DataMember()]
    public string Username { get; set; } = "";
    [DataMember()]
    public string DisplayName { get; set; } = "";
    [DataMember()]
    public int Likes { get; set; } = 0;
    [DataMember()]
    public int Dislikes { get; set; } = 0;
    [DataMember()]
    public int Reports { get; set; } = 0;
    [DataMember()]
    public int Liked { get; set; } = 0;
    [DataMember()]
    public int Disliked { get; set; } = 0;
    [DataMember()]
    public int Reported { get; set; } = 0;
    #endregion

  }
}