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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CSharpFunctionalExtensions;
using Origam.Extensions;
using Origam.Workbench.Services;

namespace Origam.DA.Service.MetaModelUpgrade
{
    public interface IMetaModelUpgradeService : IWorkbenchService
    {
        event EventHandler<UpgradeProgressInfo> UpgradeProgress;
        event EventHandler UpgradeStarted;
        event EventHandler UpgradeFinished;
        List<XmlFileData> Upgrade(List<XmlFileData> xmlFileData);
        void Cancel();
    }

    public class NullMetaModelUpgradeService : IMetaModelUpgradeService
    {
        public event EventHandler<UpgradeProgressInfo> UpgradeProgress;
        public event EventHandler UpgradeStarted;
        public event EventHandler UpgradeFinished;
        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }

        public List<XmlFileData> Upgrade(List<XmlFileData> xmlFileData)
        {
            return xmlFileData;
        }

        public void Cancel()
        {
        }
    }

    public class MetaModelUpgradeService: IMetaModelUpgradeService
    {
        private MetaModelUpGrader metaModelUpGrader;
        public event EventHandler<UpgradeProgressInfo> UpgradeProgress;
        public event EventHandler UpgradeStarted;
        public event EventHandler UpgradeFinished;
        private bool canceled;

        public List<XmlFileData> Upgrade(List<XmlFileData> xmlFileData)
        {
            metaModelUpGrader = new MetaModelUpGrader();

            int filesProcessed = 0;
            UpgradeStarted?.Invoke(null, EventArgs.Empty);
            List<XmlFileData> upgradedData = xmlFileData
                .AsParallel()
                .Where(x => !canceled)
                .Select(fileData => new XFileData(fileData))
                .Select(xFileData =>
                {
                    metaModelUpGrader.TryUpgrade(xFileData);
                    filesProcessed += 1;
                    UpgradeProgress?.Invoke(
                        null,
                        new UpgradeProgressInfo(xmlFileData.Count,
                            filesProcessed));
                    return xFileData;
                })
                .Select(fileData => new XmlFileData(fileData))
                .ToList();
            UpgradeFinished?.Invoke(null, EventArgs.Empty);
            
            return canceled 
                ? new List<XmlFileData>()
                : upgradedData;
        }

        public void Cancel()
        {
            canceled = true;
        }

        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }
    }

    public class XFileData
    {
        public OrigamXDocument Document { get; }
        public FileInfo File { get; }


        public XFileData(XmlFileData xmlFileData)
            : this(xmlFileData.XmlDocument, xmlFileData.FileInfo)
        {
        }

        public XFileData(OrigamXmlDocument xmlDocument, FileInfo file)
        {
            Document = new OrigamXDocument(xmlDocument);
            File = file;
        }

        public XFileData(OrigamXDocument document, FileInfo file)
        {
            Document = document;
            File = file;
        }
    }
}