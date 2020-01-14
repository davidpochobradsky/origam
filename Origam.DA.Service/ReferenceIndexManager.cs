﻿#region license
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Origam.DA.ObjectPersistence;
using Origam.Schema;

namespace Origam.DA.Service
{
    public static class ReferenceIndexManager
    {
        private static readonly List<IPersistent> temporaryAction = new List<IPersistent>();
        private static object obj = new object();
        public static bool UseIndex { get; private set; } = false;
        private static bool blockAddTemporaryAction = false;
        private static readonly List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>> referenceIndex
            = new List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>>();
        internal static List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>> GetReferenceIndex()
        {
            return referenceIndex;
        }
        public static void ClearReferenceIndex(bool fullClear)
        {
            UseIndex = false;
            blockAddTemporaryAction = false;
            if (fullClear) 
            { 
                temporaryAction.Clear();
            }
            referenceIndex.Clear();
        }
        private static void Remove(IPersistent sender)
        {
            List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>> ListForDelete
               = referenceIndex.Where(x => x.Value.Key == sender.Id).ToList();
            foreach (var items in ListForDelete)
            {
                referenceIndex.Remove(items);
            }
        }
        private static void Add(IPersistent sender)
        {
            if (sender is AbstractSchemaItem)
            {
                List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>> newReferenceIndex = new List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>>();
                Addreference((AbstractSchemaItem)sender, newReferenceIndex);
                referenceIndex.AddRange(newReferenceIndex);
            }
        }
        internal static void UpdateReferenceIndex(IPersistent sender)
        {
            DoTemporaryAction(sender);
            if (blockAddTemporaryAction)
            {
                UpdateIndex(sender);
            }
        }
        private static void UpdateIndex(IPersistent sender)
        {
            Remove(sender);
            if (!sender.IsDeleted)
            {
                Add(sender);
            }
        }
        private static void DoTemporaryAction(IPersistent sender)
        {
            if (!blockAddTemporaryAction)
            {
                lock (temporaryAction)
                {
                    if (!blockAddTemporaryAction)
                    {
                        if (sender == null)
                        {
                            foreach (IPersistent persistent in temporaryAction)
                            {
                                UpdateIndex(persistent);
                            }
                            temporaryAction.Clear();
                            blockAddTemporaryAction = true;
                        }
                        else
                        {
                            temporaryAction.Add(sender);
                        }
                    }
                }
            }
        }
        private static void Addreference(AbstractSchemaItem retrievedObj, List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>> referenceIndex)
        {
            CheckDependencies(retrievedObj, referenceIndex);
            foreach (AbstractSchemaItem item in retrievedObj.ChildItems)
            {
                CheckDependencies(item, referenceIndex);
            }
        }
        private static void CheckDependencies(AbstractSchemaItem item, List<KeyValuePair<Guid, KeyValuePair<Guid, Type>>> referenceIndex)
        {
                ArrayList dependencies = item.GetDependencies(false);
                foreach (AbstractSchemaItem item1 in dependencies)
                {
                    if (item1 != null)
                    {
                        lock (obj)
                        {
                            referenceIndex.Add
                                (new KeyValuePair<Guid, KeyValuePair<Guid, Type>>(item1.Id, new KeyValuePair<Guid, Type>(item.Id, item.GetType())));
                        }
                    }
                }
        }
        public static void AddToBuildIndex(IFilePersistent item)
        {
            AbstractSchemaItem schemaItem = (AbstractSchemaItem)item;
            Addreference(schemaItem, referenceIndex);
        }
        public static void ActivateReferenceIndex()
        {
            DoTemporaryAction(null);
            UseIndex = true;
        }
    }
}
