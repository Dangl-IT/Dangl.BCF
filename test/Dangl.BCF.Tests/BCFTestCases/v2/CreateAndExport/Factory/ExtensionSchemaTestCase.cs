using System;
using Dangl.BCF.BCFv2;
using Dangl.BCF.BCFv2.Schemas;

namespace Dangl.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory
{
    public static class ExtensionSchemaTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var container = new BCFv2Container();
            container.ProjectExtensions = new ProjectExtensions();
            container.ProjectExtensions.Priority.Add("Low");
            container.ProjectExtensions.Priority.Add("Medium");
            container.ProjectExtensions.Priority.Add("High");
            container.ProjectExtensions.SnippetType.Add("IFC2X3");
            container.ProjectExtensions.SnippetType.Add("IFC4");
            container.ProjectExtensions.SnippetType.Add("JSON");
            container.ProjectExtensions.TopicLabel.Add("Development");
            container.ProjectExtensions.TopicLabel.Add("Architecture");
            container.ProjectExtensions.TopicLabel.Add("MEP");
            container.ProjectExtensions.TopicStatus.Add("Open");
            container.ProjectExtensions.TopicStatus.Add("Closed");
            container.ProjectExtensions.TopicStatus.Add("Reopened");
            container.ProjectExtensions.TopicType.Add("Information");
            container.ProjectExtensions.TopicType.Add("Warning");
            container.ProjectExtensions.TopicType.Add("Error");
            container.ProjectExtensions.TopicType.Add("Request");
            container.ProjectExtensions.UserIdType.Add("Architect@example.com");
            container.ProjectExtensions.UserIdType.Add("MEPEngineer@example.com");
            container.ProjectExtensions.UserIdType.Add("Developer@example.com");
            // Add a single topic
            container.Topics.Add(new BCFTopic());
            container.Topics[0].Markup = new Markup();
            container.Topics[0].Markup.Topic = new Topic();
            container.Topics[0].Markup.Topic.Guid = BcFv2TestCaseData.EXTENSION_SCHEMA_TOPIC_GUID;
            container.Topics[0].Markup.Topic.CreationAuthor = "Developer@example.com";
            container.Topics[0].Markup.Topic.CreationDate = DateTime.UtcNow;
            container.Topics[0].Markup.Topic.Title = "Test case for checking extension schema within the BCFZip container.";
            container.Topics[0].Markup.Topic.TopicStatus = "Open";
            container.Topics[0].Markup.Topic.TopicType = "Information";
            return container;
        }
    }
}