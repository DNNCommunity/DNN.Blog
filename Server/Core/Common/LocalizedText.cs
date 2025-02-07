using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DotNetNuke.Modules.Blog.Core.Common
{
    [Serializable()]
    public class LocalizedText : IXmlSerializable
    {
        private Dictionary<string, string> _texts = new Dictionary<string, string>();
        public List<string> Locales
        {
            get
            {
                var res = new List<string>();
                foreach (string k in _texts.Keys)
                    res.Add(k);
                return res;
            }
        }
        public string this[string key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return _texts[key];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                _texts[key] = value;
            }
        }

        public Dictionary<string, string> GetDictionary()
        {
            return _texts;
        }
        public bool ContainsKey(string key)
        {
            return _texts.ContainsKey(key);
        }
        public void Add(string key, string value)
        {
            _texts.Add(key, value);
        }
        public bool Remove(string key)
        {
            return _texts.Remove(key);
        }

        public LocalizedText() : base()
        {
        }

        public LocalizedText(IDataReader ir, string FieldName) : base()
        {
            while (ir.Read())
            {
                if (!ReferenceEquals(ir[FieldName], DBNull.Value))
                {
                    _texts.Add(Convert.ToString(ir["Locale"]), Convert.ToString(ir[FieldName]));
                }
            }
            ir.Close();
            ir.Dispose();
        }

        public string ToJSONArray(string DefaultLocale, string DefaultText)
        {
            string res = "";
            foreach (string localeCode in _texts.Keys)
            {
                res += ", \"" + localeCode.Replace("-", "_") + "\": \"";
                res += _texts[localeCode];
                res += "\"";
            }
            res += ", \"" + DefaultLocale.Replace("-", "_") + "\": \"";
            res += DefaultText;
            res += "\"";
            return res;
        }

        public override string ToString()
        {
            var res = new StringBuilder();
            var xw = XmlWriter.Create(res);
            xw.WriteStartElement("MLText");
            foreach (string l in _texts.Keys)
            {
                xw.WriteStartElement("Text");
                xw.WriteAttributeString("Locale", l);
                xw.WriteString(_texts[l]);
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
            xw.Flush();
            return res.ToString();
        }

        public void Deserialize(string xml)
        {
            var str = new System.IO.StringReader(xml);
            var xr = XmlReader.Create(str);
            xr.MoveToContent();
            while (xr.ReadToFollowing("Text"))
            {
                if (xr.MoveToAttribute("Locale"))
                {
                    string l = xr.ReadContentAsString();
                    xr.MoveToContent();
                    string s = xr.ReadElementContentAsString();
                    _texts.Add(l, s);
                }
            }
        }

        public void FromXml(XmlNode xml)
        {
            if (xml is null)
                return;

        }

        public string ToConcatenatedString()
        {
            var res = new StringBuilder();
            foreach (string l in _texts.Keys)
            {
                res.Append(_texts[l]);
                res.Append(" ");
            }
            return res.ToString();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        private string readElement(XmlReader reader, string ElementName)
        {
            if (!(reader.NodeType == XmlNodeType.Element) || (reader.Name ?? "") != (ElementName ?? ""))
            {
                reader.ReadToFollowing(ElementName);
            }
            if (reader.NodeType == XmlNodeType.Element)
            {
                return reader.ReadElementContentAsString();
            }
            else
            {
                return "";
            }
        }

        private string readAttribute(XmlReader reader, string AttributeName)
        {
            if (reader.HasAttributes)
            {
                reader.MoveToAttribute(AttributeName);
                return reader.Value;
            }
            else
            {
                return "";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ReadXml fills the object (de-serializes it) from the XmlReader passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="reader">The XmlReader that contains the xml for the object</param>
        /// <history>
        /// 	[pdonker]	05/21/2008  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ReadXml(XmlReader reader)
        {
            while (reader.Name == "Text")
            {
                reader.ReadStartElement("Text");
                string loc = readAttribute(reader, "Locale");
                if (!string.IsNullOrEmpty(loc))
                {
                    string txt = reader.ReadElementContentAsString();
                    Add(loc, txt);
                }
                reader.ReadEndElement(); // Text
            }
        }

        public void ReadXml(XmlNode xMLText)
        {
            if (xMLText is null)
                return;
            foreach (XmlNode xText in xMLText.ChildNodes)
            {
                string locale = xText.Attributes["Locale"].InnerText;
                _texts.Add(locale, xText.InnerText);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// WriteXml converts the object to Xml (serializes it) and writes it using the XmlWriter passed
        /// </summary>
        /// <remarks></remarks>
        /// <param name="writer">The XmlWriter that contains the xml for the object</param>
        /// <history>
        /// 	[pdonker]	05/21/2008  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void WriteXml(XmlWriter writer)
        {
            foreach (string locale in _texts.Keys)
            {
                writer.WriteStartElement("Text");
                writer.WriteAttributeString("Locale", locale);
                writer.WriteCData(_texts[locale]);
                writer.WriteEndElement();
            }
        }

    }
}