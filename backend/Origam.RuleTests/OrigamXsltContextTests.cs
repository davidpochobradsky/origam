﻿#region license

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Moq;
using NUnit.Framework;
using Origam.DA;
using Origam.Schema;
using Origam.Workbench.Services;
using Assert = NUnit.Framework.Assert;

namespace Origam.Rule.Tests
{
    [TestFixture]
    public class OrigamXsltContextTests
    {
        RuleEngine ruleEngine;
        private Mock<IBusinessServicesService> businessServiceMock;
        private Mock<IParameterService> parameterServiceMock;
        private Mock<IDataLookupService> lookupServiceMock;
        private Mock<IOrigamAuthorizationProvider> authorizationProvider;
        private Mock<Func<UserProfile>> userProfileGetterMock;
        [SetUp]
        public void Init()
        {
            lookupServiceMock = new Mock<IDataLookupService>();
            businessServiceMock = new Mock<IBusinessServicesService>();
            businessServiceMock
                .Setup(service => service.XslFunctionProviderServiceAgents)
                .Returns(new  List<IServiceAgent>());
            var stateMachineServiceMock = new Mock<IStateMachineService>();
            var tracingServiceMock = new Mock<ITracingService>();
            var documentationServiceMock = new Mock<IDocumentationService>();
            parameterServiceMock = new Mock<IParameterService>();
            authorizationProvider = new Mock<IOrigamAuthorizationProvider>();
            userProfileGetterMock = new Mock<Func<UserProfile>>();

            ruleEngine = new RuleEngine(
                new Hashtable(), 
                null,
                new NullPersistenceService(),
                lookupServiceMock.Object,
                parameterServiceMock.Object,
                businessServiceMock.Object,
                stateMachineServiceMock.Object,
                tracingServiceMock.Object,
                documentationServiceMock.Object,
                authorizationProvider.Object,
                userProfileGetterMock.Object
            );
        }
        
        [Test]
        public void ShouldGetConstant()
        {
            string expectedResult = "constant1_value";
            string xpath = "AS:GetConstant('constant1')";
            
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            parameterServiceMock
                .Setup(service => service.GetParameterValue("constant1", OrigamDataType.String, null))
                .Returns(expectedResult);
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        } 
        
        [TestCase("string1", new object[0], "string1_value")]
        [TestCase("string1 {0} {1}", new object[]{"1", "2"}, "string1_value 1 2")]
        [TestCase("string1 {0} {1} {2} {3}", new object[]{"1", "2", "3", "4"}, "string1_value 1 2 3 4")]
        public void ShouldGetString(string stringName, object[] args, string expectedResult)
        {
            string argsString = string.Join(
                ", ", 
                args.Cast<string>().Select(arg => $"'{arg}'"));
            if (argsString != "")
            {
                argsString = ", " + argsString;
            }

            string xpath = $"AS:GetString('{stringName}'{argsString})";
            
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);

            parameterServiceMock
                .Setup(service => service.GetString(stringName, true, args))
                .Returns(expectedResult);
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        
        [Test]
        public void ShouldRunNumberOperand()
        {
            string xpath = "AS:NumberOperand('1', '1', 'PLUS')";
            object expectedResult = "2" ;
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);

            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        } 
        
        [TestCase("Plus", "1", "1", "2")]
        [TestCase("Minus", "1", "1", "0")]
        [TestCase("Div", "4", "2", "2")]
        [TestCase("Mul", "2", "2", "4")]
        [TestCase("Mod", "5", "2", "1")]
        public void ShouldRunMathFunctions(string functionName, string parameter1,
            string parameter2, string expectedResult)
        {
            string xpath = $"AS:{functionName}('{parameter1}', '{parameter2}')";

            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);

            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        
        [Test]
        public void ShouldFormatNumber()
        {
            string xpath = "AS:FormatNumber('1.54876', 'E')";
            object expectedResult = "1,548760E+000" ;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }   
        
        [TestCase("AS:LookupValue('{0}', '{1}')", new []{"lookupValue"})]
        [TestCase("AS:LookupValue('{0}', '{1}', '{2}', '{3}', '{4}')", new []{"par1", "val1", "par2", "val2"})]
        [TestCase("AS:LookupValue('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", new []{"par1", "val1", "par2", "val2", "par3", "val3"})]
        public void ShouldLookupValue(string xpathTemplate, string[] parameters)
        {
            string lookupId = "45b07cce-3d02-448c-afca-1b6f1eb158b5";
            var formatArguments = new List<object> { lookupId };
            formatArguments.AddRange(parameters);
            string xpath = string.Format(xpathTemplate, formatArguments.ToArray());
            object expectedResult = "lookupResult" ;

            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);

            if (parameters.Length == 1)
            {
                lookupServiceMock
                    .Setup(service => service.GetDisplayText(Guid.Parse(lookupId), parameters[0],false, false, null))
                    .Returns(expectedResult);
            }
            else if (parameters.Length == 4)
            {
                Hashtable paramTable = new Hashtable(3);
                paramTable[parameters[0]] = parameters[1];
                paramTable[parameters[2]] = parameters[3];
                lookupServiceMock
                    .Setup(service => service.GetDisplayText(Guid.Parse(lookupId), paramTable,false, false, null))
                    .Returns(expectedResult);

                object displayText = lookupServiceMock.Object.GetDisplayText(Guid.Parse(lookupId),
                    paramTable, false, false, null);
            }            
            else if (parameters.Length == 6)
            {
                Hashtable paramTable = new Hashtable(3);
                paramTable[parameters[0]] = parameters[1];
                paramTable[parameters[2]] = parameters[3];
                paramTable[parameters[4]] = parameters[5];
                lookupServiceMock
                    .Setup(service => service.GetDisplayText(Guid.Parse(lookupId), paramTable,false, false, null))
                    .Returns(expectedResult);
            }
            else
            {
                throw new Exception("Wrong number of test parameters.");
            }

            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        } 
                
        [Test]
        public void ShouldRoundNumber()
        {
            string xpath = "AS:NormalRound(1.54876, 2)";
            object expectedResult = "1.55" ;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }   
        
        [Test]
        public void ShouldConvertImage()
        {
            // white square 6x6, png format.
            var image = new byte[]{137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 
                72, 68, 82, 0, 0, 0, 6, 0, 0, 0, 6, 8, 2, 0, 0, 0, 111, 174, 120,
                31, 0, 0, 0, 1, 115, 82, 71, 66, 0, 174, 206, 28, 233, 0, 0, 0,
                4, 103, 65, 77, 65, 0, 0, 177, 143, 11, 252, 97, 5, 0, 0, 0, 9,
                112, 72, 89, 115, 0, 0, 22, 37, 0, 0, 22, 37, 1, 73, 82, 36, 240,
                0, 0, 0, 23, 73, 68, 65, 84, 24, 87, 99, 252, 255, 255, 63, 3, 42, 
                96, 130, 210, 72, 128, 122, 66, 12, 12, 0, 81, 221, 3, 9, 217, 
                253, 155, 178, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130};
                
            string xpath = $"AS:ResizeImage('{Convert.ToBase64String(image)}', '4', '4')";
            object expectedResult = 
                "iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAAXNSR0IArs4c6QA" +
                "AAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAmSURBVBhXY/" +
                "j3798KID4PwyCBm/+RAEjgIpQNBiCBy1A2EPz/DwAwcj5F+ErgqwAAAABJRU5Er" +
                "kJgggAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAA==" ;
            
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }  
        
        [Test]
        public void ShouldDoOrigamRound()
        {
            string xpath = $"AS:OrigamRound('1,569456', 'rounding1')";
            object expectedResult = "2" ;

            lookupServiceMock
                .Setup(service => service.GetDisplayText(Guid.Parse("7d3d6933-648b-42cb-8947-0d2cb700152b"), "rounding1", null))
                .Returns(1m);            
            lookupServiceMock
                .Setup(service => service.GetDisplayText(Guid.Parse("994608ad-9634-439b-975a-484067f5b5a6"), "rounding1", false, false, null))
                .Returns("9ecc0d91-f4bd-411e-936d-e4a8066b38dd");

            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }    
        
        [Test]
        public void ShouldExecureiif()
        {
            string xpath = $"AS:iif('true', 1, 0)";
            object expectedResult = "1";

            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }   
        
        [TestCase(new object[]{null, "1"})]
        [TestCase(new object[]{null, null, "1"})]
        [TestCase(new object[]{null, null, null, "1"})]
        public void ShouldExecureisnull(object[] arguments)
        {
            var strArguments =  arguments.Select(x=> x == null ? "null" : $"'{x}'");
            string xpath = $"AS:isnull({string.Join(", ", strArguments)})";
            object expectedResult = arguments[arguments.Length - 1];

            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }    

        [Test]
        public void ShouldEncodeDataForUri()
        {
            string xpath = $"AS:EncodeDataForUri('http://test?p=193')";
            string expectedResult = "http%3A%2F%2Ftest%3Fp%3D193";

            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }  
        
        [Test]
        public void ShouldDecodeDataFromUri()
        {
            string xpath = "AS:DecodeDataFromUri('http%3A%2F%2Ftest%3Fp%3D193')";
            string expectedResult = $"http://test?p=193";

            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }  
        
        [TestCase("AS:AddMinutes('2022-07-13', 10)", "2022-07-13T00:10:00.0000000+02:00")]
        [TestCase("AS:AddHours('2022-07-13', 1)", "2022-07-13T01:00:00.0000000+02:00")]
        [TestCase("AS:AddDays('2022-07-13', 1)", "2022-07-14T00:00:00.0000000+02:00")]
        [TestCase("AS:AddMonths('2022-07-13', 1)", "2022-08-13T00:00:00.0000000+02:00")]
        [TestCase("AS:AddYears('2022-07-13', 1)", "2023-07-13T00:00:00.0000000+02:00")]
        public void ShouldAddTime(string xpath, string expectedResult)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }             
        
        [TestCase("AS:DifferenceInDays('2022-07-13', '2022-07-14')", 1.0)]
        [TestCase("AS:DifferenceInMinutes('2022-07-13', '2022-07-14')", 1440.0)]
        [TestCase("AS:DifferenceInSeconds('2022-07-13', '2022-07-14')", 86_400.0)]
        public void ShouldGetTimeDifference(string xpath, double expectedResult)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }           
        [Test]
        public void ShouldGetUTCDateTime()
        {
            DateTime.Now;
            string xpath = "AS:UTCDateTime()";
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            XPathNavigator nav = new XmlDocument().CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            
            OrigamXsltContext sut = new OrigamXsltContext(
                new NameTable(), 
                ruleEngine, 
                businessServiceMock.Object
            );
            expr.SetContext(sut);
            object result = nav.Evaluate(expr);
            Assert.That(result, Is.EqualTo(expectedResult));
        }   
        
    }
}
