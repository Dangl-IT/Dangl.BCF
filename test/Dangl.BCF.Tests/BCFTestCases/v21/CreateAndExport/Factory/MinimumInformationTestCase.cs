using System;
using Dangl.BCF.BCFv21;
using Dangl.BCF.BCFv21.Schemas;

namespace Dangl.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class MinimumInformationTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var container = new BCFv21Container();
            container.Topics.Add(new BCFTopic());
            container.Topics[0].Markup = new Markup();
            container.Topics[0].Markup.Topic.Guid = BcFv21TestCaseData.MINIMUM_INFORMATION_TOPIC_GUID;
            container.Topics[0].Markup.Topic.Title = "Minimum information BCFZip topic.";
            container.Topics[0].Markup.Topic.CreationAuthor = "Developer@example.com";
            container.Topics[0].Markup.Topic.CreationDate = new DateTime(2015, 07, 15, 13, 12, 42, DateTimeKind.Utc);
            return container;
        }
    }
}