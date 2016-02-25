using System;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.CreateAndExport.Factory
{
    public static class ExtensionSchemaTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var Container = new BCFv2Container();
            Container.ProjectExtensions = new Extensions_XSD();
            Container.ProjectExtensions.Priority.Add("Low");
            Container.ProjectExtensions.Priority.Add("Medium");
            Container.ProjectExtensions.Priority.Add("High");
            Container.ProjectExtensions.SnippetType.Add("IFC2X3");
            Container.ProjectExtensions.SnippetType.Add("IFC4");
            Container.ProjectExtensions.SnippetType.Add("JSON");
            Container.ProjectExtensions.TopicLabel.Add("Development");
            Container.ProjectExtensions.TopicLabel.Add("Architecture");
            Container.ProjectExtensions.TopicLabel.Add("MEP");
            Container.ProjectExtensions.TopicStatus.Add("Open");
            Container.ProjectExtensions.TopicStatus.Add("Closed");
            Container.ProjectExtensions.TopicStatus.Add("Reopened");
            Container.ProjectExtensions.TopicType.Add("Information");
            Container.ProjectExtensions.TopicType.Add("Warning");
            Container.ProjectExtensions.TopicType.Add("Error");
            Container.ProjectExtensions.TopicType.Add("Request");
            Container.ProjectExtensions.UserIdType.Add("Architect@example.com");
            Container.ProjectExtensions.UserIdType.Add("MEPEngineer@example.com");
            Container.ProjectExtensions.UserIdType.Add("Developer@example.com");
            // Add a single topic
            Container.Topics.Add(new BCFTopic());
            Container.Topics[0].Markup = new Markup();
            Container.Topics[0].Markup.Topic = new Topic();
            Container.Topics[0].Markup.Topic.Guid = BCFTestCaseData.ExtensionSchema_TopicGuid;
            Container.Topics[0].Markup.Topic.CreationAuthor = "Developer@example.com";
            Container.Topics[0].Markup.Topic.CreationDate = DateTime.UtcNow;
            Container.Topics[0].Markup.Topic.Title = "Test case for checking extension schema within the BCFZip container.";
            Container.Topics[0].Markup.Topic.TopicStatus = "Open";
            Container.Topics[0].Markup.Topic.TopicType = "Information";
            return Container;
        }
    }
}