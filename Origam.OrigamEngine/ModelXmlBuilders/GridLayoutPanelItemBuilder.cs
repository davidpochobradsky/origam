#region license
/*
Copyright 2005 - 2019 Advantage Solutions, s. r. o.

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

using System.Xml;

namespace Origam.OrigamEngine.ModelXmlBuilders
{
	/// <summary>
	/// Summary description for GridLayoutPanelItemBuilder.
	/// </summary>
	public class GridLayoutPanelItemBuilder
	{
		public static void Build(XmlElement parentNode, UIElementRenderData renderData)
		{
			parentNode.SetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", "UIElement");
			parentNode.SetAttribute("Type", "GridLayoutPanelItem");
			parentNode.SetAttribute("Label", renderData.Text);
			parentNode.SetAttribute("TopCell", renderData.TopCell.ToString());
			parentNode.SetAttribute("LeftCell", renderData.LeftCell.ToString());
			parentNode.SetAttribute("HeightCells", renderData.HeightCells.ToString());
			parentNode.SetAttribute("WidthCells", renderData.WidthCells.ToString());
		}
	}
}
