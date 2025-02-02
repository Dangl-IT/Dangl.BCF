﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Dangl.BCF.Tests
{
    public class BrandingCommentFactoryTests
    {
        public class GetBrandingComment
        {
            [Fact]
            public void HasCorrectVersion()
            {
                var commentText = BrandingCommentFactory.GetBrandingComment();
                var expectedVersionString = FileVersionProvider.NuGetVersion;
                Assert.Contains(expectedVersionString, commentText);
            }

            [Fact]
            public void HasCorrectTimestamp()
            {
                var commentText = BrandingCommentFactory.GetBrandingComment();
                var currentDateTime = DateTime.UtcNow;
                var regexDateTime = @"\d\d\.\d\d\.\d\d\d\d \d\d:\d\d";
                var regexMatch = Regex.Match(commentText, regexDateTime);
                Assert.True(regexMatch.Success, $"Failed to find a timestamp in: \"{commentText}\"");
                var extractedDateTime = regexMatch.Value;
                var day = int.Parse(extractedDateTime.Substring(0, 2));
                var month = int.Parse(extractedDateTime.Substring(3, 2));
                var year = int.Parse(extractedDateTime.Substring(6, 4));
                var hour = int.Parse(extractedDateTime.Substring(11, 2));
                var minute = int.Parse(extractedDateTime.Substring(14, 2));
                var parsedDateTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
                var timeDifference = Math.Abs((currentDateTime - parsedDateTime).TotalSeconds);
                Assert.True(timeDifference < 600, $"Time difference is {timeDifference} seconds, current time: {currentDateTime:dd.MM.yyyy HH:mm}, actual: {parsedDateTime:dd.MM.yyyy HH:mm}");  // should be within ten minutes of eachother, accounts for slow CI server environments
                                                    // and the fact that the actual date is only given with minutes and has no seconds part
            }

            [Fact]
            public void HasCorrectUrl()
            {
                var commentText = BrandingCommentFactory.GetBrandingComment();
                Assert.Contains(BrandingCommentFactory.BRANDING_URL, commentText);
            }

            [Fact]
            public void ConstUrlIsValidUrl()
            {
                Assert.True(Uri.IsWellFormedUriString(BrandingCommentFactory.BRANDING_URL, UriKind.Absolute));
            }

            [Fact]
            public void HasCorrectText()
            {
                var commentText = BrandingCommentFactory.GetBrandingComment();
                Assert.StartsWith("Created with the Dangl.BCF library, V", commentText);
                Assert.EndsWith($". Visit {BrandingCommentFactory.BRANDING_URL} to find out more.", commentText);
            }
        }

        public class AppendBrandingCommentToTopLevelXml
        {
            [Fact]
            public void WriteToSimpleXml()
            {
                var xmlInput = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SomeElement>
  Hello
</SomeElement>";

                var inputSplitInLines = Regex.Split(xmlInput, "\r\n?|\n");
                var expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);

                var actual = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(xmlInput);

                if (actual != expected)
                {
                    // Generating it again due to a possible difference in the timestamp (minute / hour might have changed)
                    expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);
                }
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WriteToVersionXml()
            {
                var xmlInput = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Markup xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Topic Guid=""5019D939-62A4-45D9-B205-FAB602C98FE8"">
    <Title>Referenced topic</Title>
    <CreationDate>2016-11-22T13:20:03.0814453Z</CreationDate>
    <Description>This is just an empty topic that acts as a referenced topic.</Description>
  </Topic>
</Markup>";

                var inputSplitInLines = Regex.Split(xmlInput, "\r\n?|\n");
                var expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);

                var actual = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(xmlInput);

                if (actual != expected)
                {
                    // Generating it again due to a possible difference in the timestamp (minute / hour might have changed)
                    expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);
                }
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WriteToMarkupXml()
            {
                var xmlInput = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Markup xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Topic Guid=""5019D939-62A4-45D9-B205-FAB602C98FE8"">
    <Title>Referenced topic</Title>
    <CreationDate>2016-11-22T13:20:03.0814453Z</CreationDate>
    <Description>This is just an empty topic that acts as a referenced topic.</Description>
  </Topic>
</Markup>";

                var inputSplitInLines = Regex.Split(xmlInput, "\r\n?|\n");
                var expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);

                var actual = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(xmlInput);

                if (actual != expected)
                {
                    // Generating it again due to a possible difference in the timestamp (minute / hour might have changed)
                    expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);
                }
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WriteToProjectXml()
            {
                var xmlInput = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ProjectExtension xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Project ProjectId=""F338B6F0-A93E-40FF-A4D6-6117CD21EC2A"">
    <Name>BCF API Implementation</Name>
  </Project>
  <ExtensionSchema>extensions.xsd</ExtensionSchema>
</ProjectExtension>";

                var inputSplitInLines = Regex.Split(xmlInput, "\r\n?|\n");
                var expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);

                var actual = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(xmlInput);

                if (actual != expected)
                {
                    // Generating it again due to a possible difference in the timestamp (minute / hour might have changed)
                    expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);
                }
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WriteToViewpointXml()
            {
                var xmlInput = @"<?xml version=""1.0"" encoding=""utf-8""?>
<VisualizationInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" Guid=""81daa431-bf01-4a49-80a2-1ab07c177717"">
  <Components DefaultVisibilitySpaces=""false"" DefaultVisibilitySpaceBoundaries=""false"">
    <Component IfcGuid=""0fdpeZZEX3FwJ7x0ox5kzF"" Visible=""false"" />
    <Component IfcGuid=""23Zwlpd71EyvHlH6OZ77nK"" Visible=""false"" />
    <Component IfcGuid=""1OpjQ1Nlv4sQuTxfUC_8zS"" Visible=""false"" />
    <Component IfcGuid=""0cSRUx$EX1NRjqiKcYQ$a0"" Visible=""false"" />
  </Components>
  <PerspectiveCamera>
    <CameraViewPoint>
      <X>-30.0807178226062</X>
      <Y>17.1180195726065</Y>
      <Z>11.4769701040657</Z>
    </CameraViewPoint>
    <CameraDirection>
      <X>0.520915589917324</X>
      <Y>-0.661777065303802</Y>
      <Z>-0.539164240672219</Z>
    </CameraDirection>
    <CameraUpVector>
      <X>-1.13083372396512E-08</X>
      <Y>-8.90132135110878E-09</Y>
      <Z>1</Z>
    </CameraUpVector>
    <FieldOfView>60</FieldOfView>
  </PerspectiveCamera>
</VisualizationInfo>";

                var inputSplitInLines = Regex.Split(xmlInput, "\r\n?|\n");
                var expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);

                var actual = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(xmlInput);

                if (actual != expected)
                {
                    // Generating it again due to a possible difference in the timestamp (minute / hour might have changed)
                    expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);
                }
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WriteToExtensionsSchemaXsd()
            {
                var xmlInput = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:redefine schemaLocation=""markup.xsd"">
    <xs:simpleType name=""TopicType"">
      <xs:restriction>
        <xs:enumeration value=""Architecture"" />
        <xs:enumeration value=""Hidden Type"" />
        <xs:enumeration value=""Structural"" />
      </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name=""TopicStatus"">
      <xs:restriction>
        <xs:enumeration value=""Finished status"" />
        <xs:enumeration value=""Open"" />
        <xs:enumeration value=""Closed"" />
      </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name=""TopicLabel"">
      <xs:restriction>
        <xs:enumeration value=""Architecture"" />
        <xs:enumeration value=""IT Development"" />
        <xs:enumeration value=""Management"" />
        <xs:enumeration value=""Mechanical"" />
        <xs:enumeration value=""Structural"" />
      </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name=""SnippetType"">
      <xs:restriction>
        <xs:enumeration value=""IFC2X3"" />
        <xs:enumeration value=""PDF"" />
        <xs:enumeration value=""XLSX"" />
      </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name=""Priority"">
      <xs:restriction>
        <xs:enumeration value=""Low"" />
        <xs:enumeration value=""High"" />
        <xs:enumeration value=""Medium"" />
      </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name=""UserIdType"">
      <xs:restriction>
        <xs:enumeration value=""dangl@iabi.eu"" />
        <xs:enumeration value=""linhard@iabi.eu"" />
      </xs:restriction>
    </xs:simpleType>
    <xs:simpleType name=""Stage"">
      <xs:restriction>
        <xs:enumeration value=""dangl@iabi.eu"" />
        <xs:enumeration value=""linhard@iabi.eu"" />
      </xs:restriction>
    </xs:simpleType>
  </xs:redefine>
</xs:schema>";

                var inputSplitInLines = Regex.Split(xmlInput, "\r\n?|\n");
                var expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);

                var actual = BrandingCommentFactory.AppendBrandingCommentToTopLevelXml(xmlInput);

                if (actual != expected)
                {
                    // Generating it again due to a possible difference in the timestamp (minute / hour might have changed)
                    expected = inputSplitInLines[0] + Environment.NewLine
                               + $"<!--{BrandingCommentFactory.GetBrandingComment()}-->" + Environment.NewLine
                               + inputSplitInLines.Skip(1).Aggregate((current, next) => current + Environment.NewLine + next);
                }
                Assert.Equal(expected, actual);
            }
        }
    }
}
