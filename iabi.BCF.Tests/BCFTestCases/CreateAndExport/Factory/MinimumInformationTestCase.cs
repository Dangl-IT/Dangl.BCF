using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using System;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory
{
    public static class MinimumInformationTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var Container = new BCFv2Container();
            Container.Topics.Add(new BCFTopic());
            Container.Topics[0].Markup = new Markup();
            Container.Topics[0].Markup.Topic.Guid = BCFTestCaseData.MinimumInformation_TopicGuid;
            Container.Topics[0].Markup.Topic.Title = "Minimum information BCFZip topic.";
            Container.Topics[0].Markup.Topic.CreationAuthor = "Developer@example.com";
            Container.Topics[0].Markup.Topic.CreationDate = new DateTime(2015, 07, 15, 13, 12, 42, DateTimeKind.Utc);
            return Container;
        }
    }
}