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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using Origam.DA.Service;
using Origam.DA.Service.MetaModelUpgrade;
using Origam.Extensions;

namespace Origam.DA.ServiceTests.MetaModelUpgraderTests
{
    [TestFixture]
    public class MetaModelUpGraderTests: MetaModelUpGradeTestBase
    {
        [Test]
        public void ShouldUpgradeByOneVersion()
        {
            XmlFileData xmlFileData = LoadFile("TestPersistedClassV6.0.1.origam");
            var sut = new MetaModelUpGrader(GetType().Assembly, new NullFileWriter());
            bool someFilesWereUpgraded = sut.TryUpgrade(
                new List<XmlFileData>{xmlFileData});

            XmlElement fileElement = xmlFileData.XmlDocument.FileElement;
            XmlNode classNode = fileElement.ChildNodes[0];
            Assert.True(classNode.Attributes["tpc:newProperty1"] != null); // assert the property was not removed
            Assert.True(classNode.Attributes["tpc:newProperty1"].Value == "5"); // assert the property value was not changed
            Assert.True(classNode.Attributes["tpc:newProperty2"] != null);
            Assert.True(classNode.Attributes["tbc:TestBaseClassProperty"] != null);
            Assert.True(classNode.Attributes["tbc:TestBaseClassProperty"].Value == "5");
            Assert.That(fileElement.Attributes["xmlns:tpc"]?.Value, Is.EqualTo("http://schemas.origam.com/Origam.DA.ServiceTests.TestPersistedClass/6.0.2")); 
            Assert.That(fileElement.Attributes["xmlns:tbc"]?.Value, Is.EqualTo("http://schemas.origam.com/Origam.DA.ServiceTests.TestBaseClass/6.0.1"));
        }             
        
        [Test]
        public void ShouldRemoveDeadClass()
        {
            XmlFileData xmlFileData = LoadFile("TestDeadClassV6.0.1.origam");
            var sut = new MetaModelUpGrader(GetType().Assembly, new NullFileWriter());
            sut.TryUpgrade(new List<XmlFileData>{xmlFileData});

            XmlNode fileNode = xmlFileData.XmlDocument.FileElement;
            bool classNamespacesPresent = xmlFileData.XmlDocument.Namespaces
                .Any(nameSpace => nameSpace.FullTypeName.Contains("TestDeadClass") ||
                                  nameSpace.FullTypeName.Contains("TestBaseClass"));
            Assert.False(classNamespacesPresent);
            Assert.That(fileNode.ChildNodes, Has.Count.EqualTo(0));
        }      
        
        [Test]
        public void ShouldUpgradeTwoVersions()
        {
            XmlFileData xmlFileData = LoadFile("TestPersistedClassV1.0.0.origam");
            var sut = new MetaModelUpGrader(GetType().Assembly, new NullFileWriter());
            bool someFilesWereUpgraded = sut.TryUpgrade(
                new List<XmlFileData>{xmlFileData});

            XmlNode classNode = xmlFileData.XmlDocument.ChildNodes[1].ChildNodes[0];
            Assert.True(classNode.Attributes["newProperty1"] != null);
            Assert.True(classNode.Attributes["newProperty2"] != null);
            Assert.True(classNode.Attributes["versions"] != null);
            StringAssert.Contains("Origam.DA.ServiceTests.TestPersistedClass 1.0.2", classNode.Attributes["versions"].Value);
            StringAssert.Contains("Origam.DA.ServiceTests.TestBaseClass 1.0.1", classNode.Attributes["versions"].Value);
        }

        [Test]
        public void ShouldThrowIfOneOfUpgradeScriptsIsMissing()
        {
            Assert.Throws<ClassUpgradeException>(() =>
            {
                XmlFileData xmlFileData = LoadFile("TestPersistedClass2V1.0.0.origam");
                var sut = new MetaModelUpGrader(GetType().Assembly,  new NullFileWriter());
                bool someFilesWereUpgraded = sut.TryUpgrade(
                    new List<XmlFileData>{xmlFileData});
            });
        }  
        
        [Test]
        public void ShouldThrowIfAttributeIsAlreadyPresent()
        {
            Assert.Throws<ClassUpgradeException>(() =>
            {
                XmlFileData xmlFileData = LoadFile("TestPersistedClassV1.0.1_WrongVersion.origam");
                var sut = new MetaModelUpGrader(GetType().Assembly,  new NullFileWriter());
                bool someFilesWereUpgraded = sut.TryUpgrade(
                    new List<XmlFileData>{xmlFileData});
            });
        }
    }
}