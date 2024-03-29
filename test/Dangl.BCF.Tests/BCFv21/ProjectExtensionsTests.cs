using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Dangl.BCF.BCFv21;
using System.Linq;

namespace Dangl.BCF.Tests.BCFv21
{
    public class ProjectExtensionsTests
    {
        [Fact]
        public void CanReadSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.NotNull(extensions);
        }

        [Fact]
        public void ReadsCorrectTopicTypeForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("Information", extensions.TopicType);
            Assert.Contains("Warning", extensions.TopicType);
            Assert.Contains("Error", extensions.TopicType);
            Assert.Contains("Request", extensions.TopicType);
        }

        [Fact]
        public void ReadsCorrectTopicStatusForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("Open", extensions.TopicStatus);
            Assert.Contains("Closed", extensions.TopicStatus);
            Assert.Contains("Reopened", extensions.TopicStatus);
        }

        [Fact]
        public void ReadsCorrectTopicLabelForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("Development", extensions.TopicLabel);
            Assert.Contains("Architecture", extensions.TopicLabel);
            Assert.Contains("MEP", extensions.TopicLabel);
        }

        [Fact]
        public void ReadsCorrectSnippetTypeForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("IFC2X3", extensions.SnippetType);
            Assert.Contains("IFC4", extensions.SnippetType);
            Assert.Contains("JSON", extensions.SnippetType);
        }

        [Fact]
        public void ReadsCorrectPriorityForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("Low", extensions.Priority);
            Assert.Contains("Medium", extensions.Priority);
            Assert.Contains("High", extensions.Priority);
        }

        [Fact]
        public void ReadsCorrectUserIdTypeForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("Architect@example.com", extensions.UserIdType);
            Assert.Contains("MEPEngineer@example.com", extensions.UserIdType);
            Assert.Contains("Developer@example.com", extensions.UserIdType);
        }

        [Fact]
        public void ReadsCorrectStageForSchemaWithValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithAllValues());
            Assert.Contains("Draft", extensions.Stage);
            Assert.Contains("Tendering", extensions.Stage);
        }

        [Fact]
        public void CanReadSchemaWithNoValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithNoValues());
            Assert.NotNull(extensions);
        }

        [Fact]
        public void AllValuesEmptyWhenReadSchemaWithNoValues()
        {
            var extensions = new ProjectExtensions(GetExtensionsStringWithNoValues());
            Assert.False(extensions.Priority.Any());
            Assert.False(extensions.SnippetType.Any());
            Assert.False(extensions.Stage.Any());
            Assert.False(extensions.TopicLabel.Any());
            Assert.False(extensions.TopicStatus.Any());
            Assert.False(extensions.TopicType.Any());
            Assert.False(extensions.UserIdType.Any());
        }

        private string GetExtensionsStringWithAllValues()
        {
            var extensionsString = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<!--Created with the Dangl.BCF library, V1.1.0 at 22.05.2017 09:51. Visit http://www.dangl-it.com to find out more.-->
<schema xmlns=""http://www.w3.org/2001/XMLSchema"">
  <redefine schemaLocation=""markup.xsd"">
    <simpleType name=""TopicType"">
      <restriction base=""TopicType"">
        <enumeration value=""Information"" />
        <enumeration value=""Warning"" />
        <enumeration value=""Error"" />
        <enumeration value=""Request"" />
      </restriction>
    </simpleType>
    <simpleType name=""TopicStatus"">
      <restriction base=""TopicStatus"">
        <enumeration value=""Open"" />
        <enumeration value=""Closed"" />
        <enumeration value=""Reopened"" />
      </restriction>
    </simpleType>
    <simpleType name=""TopicLabel"">
      <restriction base=""TopicLabel"">
        <enumeration value=""Development"" />
        <enumeration value=""Architecture"" />
        <enumeration value=""MEP"" />
      </restriction>
    </simpleType>
    <simpleType name=""SnippetType"">
      <restriction base=""SnippetType"">
        <enumeration value=""IFC2X3"" />
        <enumeration value=""IFC4"" />
        <enumeration value=""JSON"" />
      </restriction>
    </simpleType>
    <simpleType name=""Priority"">
      <restriction base=""Priority"">
        <enumeration value=""Low"" />
        <enumeration value=""Medium"" />
        <enumeration value=""High"" />
      </restriction>
    </simpleType>
    <simpleType name=""UserIdType"">
      <restriction base=""UserIdType"">
        <enumeration value=""Architect@example.com"" />
        <enumeration value=""MEPEngineer@example.com"" />
        <enumeration value=""Developer@example.com"" />
      </restriction>
    </simpleType>
    <simpleType name=""Stage"">
      <restriction base=""Stage"">
        <enumeration value=""Draft"" />
        <enumeration value=""Tendering"" />
      </restriction>
    </simpleType>
  </redefine>
</schema>";
            return extensionsString;
        }

        private string GetExtensionsStringWithNoValues()
        {
            var extensionsString = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<!--Created with the Dangl.BCF library, V1.1.0 at 22.05.2017 09:51. Visit http://www.dangl-it.com to find out more.-->
<schema xmlns=""http://www.w3.org/2001/XMLSchema"">
  <redefine schemaLocation=""markup.xsd"">
  </redefine>
</schema>";
            return extensionsString;
        }
    }
}
