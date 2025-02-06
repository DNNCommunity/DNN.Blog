using System;
using System.Collections;
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

namespace DotNetNuke.Modules.Blog.Core.Security.Permissions
{

  [Serializable()]
  public class PermissionCollection : IDictionary<string, PermissionInfo>
  {

    private Dictionary<string, PermissionInfo> _permissions = new Dictionary<string, PermissionInfo>();
    private SortedDictionary<int, string> _keys = new SortedDictionary<int, string>();

    public PermissionCollection()
    {
      Add("ADD", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.ADD, PermissionKey = "ADD" });
      Add("EDIT", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.EDIT, PermissionKey = "EDIT" });
      Add("APPROVE", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.APPROVE, PermissionKey = "APPROVE" });
      Add("VIEWCOMMENT", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.VIEWCOMMENT, PermissionKey = "VIEWCOMMENT" });
      Add("ADDCOMMENT", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.ADDCOMMENT, PermissionKey = "ADDCOMMENT" });
      Add("APPROVECOMMENT", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.APPROVECOMMENT, PermissionKey = "APPROVECOMMENT" });
      Add("AUTOAPPROVECOMMENT", new PermissionInfo() { PermissionId = (int)BlogPermissionTypes.AUTOAPPROVECOMMENT, PermissionKey = "AUTOAPPROVECOMMENT" });
    }

    public PermissionInfo GetById(int id)
    {
      switch (id)
      {
        case (int)BlogPermissionTypes.ADD:
          {
            return this["ADD"];
          }
        case (int)BlogPermissionTypes.EDIT:
          {
            return this["EDIT"];
          }
        case (int)BlogPermissionTypes.APPROVE:
          {
            return this["APPROVE"];
          }
        case (int)BlogPermissionTypes.ADDCOMMENT:
          {
            return this["ADDCOMMENT"];
          }
        case (int)BlogPermissionTypes.APPROVECOMMENT:
          {
            return this["APPROVECOMMENT"];
          }
        case (int)BlogPermissionTypes.AUTOAPPROVECOMMENT:
          {
            return this["AUTOAPPROVECOMMENT"];
          }
        case (int)BlogPermissionTypes.VIEWCOMMENT:
          {
            return this["VIEWCOMMENT"];
          }
      }
      return null;
    }

    public void Add(KeyValuePair<string, PermissionInfo> item)
    {
      if (!ContainsKey(item.Key))
      {
        _permissions.Add(item.Key, item.Value);
        _keys.Add(_permissions.Count - 1, item.Key);
      }
    }

    public void Clear()
    {
      _permissions.Clear();
      _keys.Clear();
    }

    public bool Contains(KeyValuePair<string, PermissionInfo> item)
    {
      return _permissions.ContainsValue(item.Value);
    }

    public void CopyTo(KeyValuePair<string, PermissionInfo>[] array, int arrayIndex)
    {
      // todo
    }

    public int Count
    {
      get
      {
        return _permissions.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool Remove(KeyValuePair<string, PermissionInfo> item)
    {
      if (_permissions.ContainsKey(item.Key))
      {
        _permissions.Remove(item.Key);
        return true;
      }
      return false;
    }

    public void Add(string key, PermissionInfo value)
    {
      if (!_permissions.ContainsKey(key))
      {
        _permissions.Add(key, value);
        _keys.Add(_permissions.Count - 1, key);
      }
    }

    public bool ContainsKey(string key)
    {
      return _permissions.ContainsKey(key);
    }

    public PermissionInfo this[string key]
    {
      get
      {
        return _permissions[key];
      }
      set
      {
        _permissions[key] = value;
      }
    }

    public PermissionInfo this[int index]
    {
      get
      {
        return this[_keys[index]];
      }
      set
      {
        this[_keys[index]] = value;
      }
    }

    public ICollection<string> Keys
    {
      get
      {
        return _keys.Values;
      }
    }

    public bool Remove(string key)
    {
      if (_permissions.ContainsKey(key))
      {
        _permissions.Remove(key);
        return true;
      }
      return false;
    }

    public bool TryGetValue(string key, out PermissionInfo value)
    {
      return _permissions.TryGetValue(key, out value);
    }

    public ICollection<PermissionInfo> Values
    {
      get
      {
        return _permissions.Values;
      }
    }

    public IEnumerator<KeyValuePair<string, PermissionInfo>> GetEnumerator()
    {
      return null;
    }

    public IEnumerator GetEnumerator1()
    {
      return null;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator1();

  }
}