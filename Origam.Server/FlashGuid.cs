#region license
/*
Copyright 2005 - 2017 Advantage Solutions, s. r. o.

This file is part of ORIGAM.

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with ORIGAM.  If not, see<http://www.gnu.org/licenses/>.
*/
#endregion

using System;

namespace Origam.Server
{
    class FlashGuid
    {
        private byte[] _bytes;

        public FlashGuid()
        {
        }

        public FlashGuid(Guid guid)
        {
            _bytes = guid.ToByteArray();
        }

        public byte[] Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        public Guid ToGuid()
        {
            return new Guid(_bytes);
        }

        public override string ToString()
        {
            return this.ToGuid().ToString();
        }
    }
}
