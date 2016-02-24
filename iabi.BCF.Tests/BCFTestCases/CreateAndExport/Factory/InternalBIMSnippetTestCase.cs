using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using System;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory
{
    public static class InternalBIMSnippetTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var Container = new BCFv2Container();
            Container.Topics.Add(CreateTopic());
            return Container;
        }

        public static BCFTopic CreateTopic()
        {
            var ReturnTopic = new BCFTopic();
            ReturnTopic.Markup = CreateMarkup();
            ReturnTopic.SnippetData = BCFTestCaseData.JsonElement;
            return ReturnTopic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();
            Markup.Topic = new Topic
            {
                BimSnippet = new BimSnippet
                {
                    isExternal = false,
                    Reference = "JsonElement.json",
                    ReferenceSchema = "http://json-schema.org",
                    SnippetType = "JSON"
                },
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 06, 21, 12, 00, 00, DateTimeKind.Utc),
                Description = "This topic has an internal BIM Snippet reference",
                Guid = BCFTestCaseData.InternalBIMSnippet_TopicGuid,
                Index = "0",
                Title = "Internal BIM Snippet"
            };
            return Markup;
        }
    }
}
