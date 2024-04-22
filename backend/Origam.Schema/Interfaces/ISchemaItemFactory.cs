#region license
/*
Copyright 2005 - 2021 Advantage Solutions, s. r. o.

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

using System;
using System.Collections.Generic;

namespace Origam.Schema;

/// <summary>
/// Provides interface for generating new schema items.
/// </summary>
public interface ISchemaItemFactory
{
	/// <summary>
	/// Provides a new item of a given type.
	/// </summary>
	/// <param name="schemaExtensionId"></param>
	/// <param name="group"></param>
	/// <returns>New schema item.</returns>
	T NewItem<T>(Guid schemaExtensionId, SchemaItemGroup group) 
		where T : AbstractSchemaItem;
		
	/// <summary>
	/// Returns new valid group for this item.
	/// </summary>
	/// <param name="schemaExtensionId"></param>
	/// <param name="groupName"></param>
	/// <returns></returns>
	SchemaItemGroup NewGroup(Guid schemaExtensionId, string groupName="NewGroup");

	/// <summary>
	/// Gets all possible schema item types that can be asked for with NewItem method.
	/// </summary>
	Type[] NewItemTypes{get;}

	/// <summary>
	/// Gets optional list of names for new item types. This list will be used to generate a submenu
	/// for New > [Type] > [Name1] [Name2] ...
	/// </summary>
	IList<string> NewTypeNames {get;}

	/// <summary>
	/// Lists types for which the NewTypeNames can be used since not all NewItemTypes might
	/// need to be populated by predefined names.
	/// </summary>
	Type[] NameableTypes {get;}

	event Action<ISchemaItem> ItemCreated;
}