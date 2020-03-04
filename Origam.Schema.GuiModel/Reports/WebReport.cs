#region license
/*
Copyright 2005 - 2020 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using Origam.DA.Common;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Origam.DA.ObjectPersistence;

namespace Origam.Schema.GuiModel
{
	/// <summary>
	/// Summary description for CrystalReport.
	/// </summary>
	[SchemaItemDescription("Web Report", "icon_web-report.png")]
    [HelpTopic("Web+Report")]
    [ClassMetaVersion("6.0.0")]
	public class WebReport : AbstractReport
	{	
		public WebReport() : base(){}
		
		public WebReport(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public WebReport(Key primaryKey) : base(primaryKey)	{}

		private string _url;
		private string _externalUrlScheme;
		private bool _forceExternalUrl;
		private bool _isUrlEscaped = false;
		private WebPageOpenMethod _openMethod = WebPageOpenMethod.OrigamTab;
		
		[EntityColumn("LS01")]
        [XmlAttribute("url")]
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		[EntityColumn("SS03")]
        [XmlAttribute("externalUrlScheme")]
        public string ExternalUrlScheme
		{
			get
			{
				return _externalUrlScheme;
			}
			set
			{
				_externalUrlScheme = value;
			}
		}

		[EntityColumn("B01")]
        [XmlAttribute("forceExternalUrl")]
        public bool ForceExternalUrl
		{
			get
			{
				return _forceExternalUrl;
			}
			set
			{
				_forceExternalUrl = value;
			}
		}

		[EntityColumn("B02")] 
        [XmlAttribute("isUrlEscaped")]
		public bool IsUrlEscaped
		{
			get
			{
				return _isUrlEscaped;
			}
			set
			{
				_isUrlEscaped = value;
			}
		}

		[EntityColumn("I03")]
		[DefaultValue(WebPageOpenMethod.OrigamTab)]
        [XmlAttribute("openMethod")]
        public WebPageOpenMethod OpenMethod
		{
			get
			{
				return _openMethod;
			}
			set
			{
				_openMethod = value;
			}
		}
	}
}
