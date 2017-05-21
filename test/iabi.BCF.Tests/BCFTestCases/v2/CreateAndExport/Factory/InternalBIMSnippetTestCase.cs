using System;
using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v2.CreateAndExport.Factory
{
    public static class InternalBimSnippetTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var container = new BCFv2Container();
            container.Topics.Add(CreateTopic());
            return container;
        }

        public static BCFTopic CreateTopic()
        {
            var returnTopic = new BCFTopic();
            returnTopic.Markup = CreateMarkup();
            returnTopic.SnippetData = TestCaseResourceFactory.GetFileAttachment(FileAttachments.JsonElement);
            return returnTopic;
        }

        private static Markup CreateMarkup()
        {
            var markup = new Markup();
            markup.Topic = new Topic
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
                Guid = BcFv2TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID,
                Index = "0",
                Title = "Internal BIM Snippet"
            };
            return markup;
        }
    }
}