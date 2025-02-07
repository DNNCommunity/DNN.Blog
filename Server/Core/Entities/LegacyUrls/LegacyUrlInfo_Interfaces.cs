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


using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using System;
using System.Data;

namespace DotNetNuke.Modules.Blog.Core.Entities.LegacyUrls
{
    public partial class LegacyUrlInfo : IHydratable
    {
        #region  IHydratable Implementation 
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fill hydrates the object from a Datareader
        /// </summary>
        /// <remarks>The Fill method is used by the CBO method to hydrtae the object
        /// rather than using the more expensive Refection methods.</remarks>
        /// <history>
        /// [pdonker] 05/23/2013 Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void Fill(IDataReader dr)
        {

            ContentItemId = Convert.ToInt32(Null.SetNull(dr["ContentItemId"], ContentItemId));
            EntryId = Convert.ToInt32(Null.SetNull(dr["EntryId"], EntryId));
            Url = Convert.ToString(Null.SetNull(dr["Url"], Url));

        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Key ID
        /// </summary>
        /// <remarks>The KeyID property is part of the IHydratble interface. It is used
        /// as the key property when creating a Dictionary</remarks>
        /// <history>
        /// [pdonker] 05/23/2013 Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int KeyID
        {
            get
            {
                return default;
            }
            set
            {
            }
        }
        #endregion
    }
}