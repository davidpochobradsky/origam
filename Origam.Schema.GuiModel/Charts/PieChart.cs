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


namespace Origam.Schema.GuiModel
{
	[SchemaItemDescription("Pie Chart", "icon_pie-chart.png")]
    [HelpTopic("Pie+Charts")]
    [ClassMetaVersion("1.0.0")]
	public class PieChart : AbstractChart
	{
		public PieChart() : base() {Init();}
		public PieChart(Guid schemaExtensionId) : base(schemaExtensionId) {Init();}
		public PieChart(Key primaryKey) : base(primaryKey) {Init();}

		private void Init()
		{
			this.ChildItemTypes.Add(typeof(PieSeries));
		}
	}
}