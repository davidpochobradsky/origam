﻿#region license
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
using System.Runtime.Serialization;

namespace Origam.Server
{
	class SessionExpiredException : Exception
	{
		public SessionExpiredException() : base(Properties.Resources.ErrorSessionExpired)
		{ 
		}

		public SessionExpiredException(string message) : base(message)
		{
		}

		public SessionExpiredException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SessionExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
