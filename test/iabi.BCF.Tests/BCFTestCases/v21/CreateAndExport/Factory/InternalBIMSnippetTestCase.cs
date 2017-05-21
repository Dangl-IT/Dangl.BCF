using System;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class InternalBimSnippetTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var container = new BCFv21Container();
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
                Guid = BcFv21TestCaseData.INTERNAL_BIM_SNIPPET_TOPIC_GUID,
                Index = 0,
                Title = "Internal BIM Snippet"
            };
            return markup;
        }
    }
}